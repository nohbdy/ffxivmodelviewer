using System;
using System.Collections.Generic;
using System.Text;

namespace DatDigger.Sections
{
    public delegate Chunk ChunkFactory(SectionType sectionType, Chunk parent);

    public static class ChunkLoader
    {
        private static Dictionary<int, Dictionary<int, ChunkFactory>> chunkFactories = new Dictionary<int, Dictionary<int, ChunkFactory>>();

        private static Chunk GenericChunkFactory<T>(SectionType sectionType, Chunk parent) where T : Chunk, new()
        {
            Chunk result = new T();
            result.SectionType = sectionType;
            return result;
        }

        private static void RegisterChunk<T>(SectionType sectionType, int chunkType) where T : Chunk, new()
        {
            var dict = chunkFactories[(int)sectionType];
            dict.Add(chunkType, GenericChunkFactory<T>);
        }

        private static Chunk AabbChunkFactory(SectionType sectionType, Chunk parent)
        {
            Chunk result;
            if (parent is Model.BoundingBoxContainerChunk)
            {
                result = new Model.BoundingBoxChunk();
            }
            else
            {
                result = new Model.BoundingBoxContainerChunk();
            }
            result.SectionType = sectionType;
            return result;
        }

        private static Chunk HeaderChunkFactory(SectionType sectionType, Chunk parent)
        {
            Chunk result;
            if (parent is Model.MeshChunk)
            {
                result = new Model.MeshHeaderChunk();
            }
            else
            {
                result = new Model.HeaderChunk();
            }
            result.SectionType = sectionType;
            return result;
        }

        static ChunkLoader()
        {
            Dictionary<int, ChunkFactory> modelFactories = new Dictionary<int, ChunkFactory>();
            chunkFactories.Add((int)SectionType.wrb, modelFactories);
            chunkFactories.Add((int)SectionType.sdrb, new Dictionary<int, ChunkFactory>());

            // Shader Chunks
            RegisterChunk<Shader.ShaderChunk>(SectionType.sdrb, (int)Shader.ChunkType.SHD);
            RegisterChunk<Shader.FileChunk>(SectionType.sdrb, (int)Shader.ChunkType.FILE);
            RegisterChunk<Shader.VcapChunk>(SectionType.sdrb, (int)Shader.ChunkType.VCAP);
            RegisterChunk<Shader.PcapChunk>(SectionType.sdrb, (int)Shader.ChunkType.PCAP);
            RegisterChunk<Shader.PramChunk>(SectionType.sdrb, (int)Shader.ChunkType.PRAM);

            // Model Chunks
            RegisterChunk<Model.WrbChunk>(SectionType.wrb, (int)Model.ChunkType.WRB);
            RegisterChunk<Model.ModelContainerChunk>(SectionType.wrb, (int)Model.ChunkType.MDLC);
            RegisterChunk<Model.ModelChunk>(SectionType.wrb, (int)Model.ChunkType.MDL);
            RegisterChunk<Model.MeshChunk>(SectionType.wrb, (int)Model.ChunkType.MESH);
            RegisterChunk<Model.StreamChunk>(SectionType.wrb, (int)Model.ChunkType.STMS);
            RegisterChunk<Model.NameChunk>(SectionType.wrb, (int)Model.ChunkType.NAME);
            RegisterChunk<Model.StringChunk>(SectionType.wrb, (int)Model.ChunkType.STR);
            RegisterChunk<Model.RsidChunk>(SectionType.wrb, (int)Model.ChunkType.RSID);
            RegisterChunk<Model.RstpChunk>(SectionType.wrb, (int)Model.ChunkType.RSTP);
            RegisterChunk<Model.PridChunk>(SectionType.wrb, (int)Model.ChunkType.PRID);
            RegisterChunk<Model.PrtpChunk>(SectionType.wrb, (int)Model.ChunkType.PRTP);
            RegisterChunk<Model.EnvdChunk>(SectionType.wrb, (int)Model.ChunkType.ENVD);
            RegisterChunk<Model.LtcdChunk>(SectionType.wrb, (int)Model.ChunkType.LTCD);
            RegisterChunk<Model.MictChunk>(SectionType.wrb, (int)Model.ChunkType.MICT);
            RegisterChunk<Model.MinsChunk>(SectionType.wrb, (int)Model.ChunkType.MINS);
            RegisterChunk<Model.CompChunk>(SectionType.wrb, (int)Model.ChunkType.COMP);
            RegisterChunk<Model.PgrpChunk>(SectionType.wrb, (int)Model.ChunkType.PGRP);
            RegisterChunk<Model.ShapChunk>(SectionType.wrb, (int)Model.ChunkType.SHAP);

            // The following are special cases - they can appear in multiple contexts
            modelFactories.Add((int)Model.ChunkType.AABB, AabbChunkFactory);
            modelFactories.Add((int)Model.ChunkType.HEAD, HeaderChunkFactory);
        }

        /// <summary>Create an empty Chunk</summary>
        /// <param name="magicNumber">The magic number used to identify the chunk</param>
        /// <returns>A newly created chunk of the requested type or null if no Chunk types are registered to the given magic number</returns>
        private static Chunk CreateChunk(SectionType sectionType, int magicNumber, Chunk parent)
        {
            Dictionary<int, ChunkFactory> factories;
            if (!chunkFactories.TryGetValue((int)sectionType, out factories))
            {
                throw new InvalidOperationException("No factories exist for SectionType " + sectionType);
            }

            ChunkFactory factory;
            if (factories.TryGetValue(magicNumber, out factory))
            {
                return factory(sectionType, parent);
            }

            throw new InvalidOperationException(String.Format("Cannot create Chunk of type {0}", Encoding.ASCII.GetString(BitConverter.GetBytes(magicNumber))));
        }

        /// <summary>Create a Chunk by determining which type is next in the stream</summary>
        /// <param name="reader">BinaryReader pointed at the start of a chunk</param>
        /// <returns>A newly created chunk of the correct type or null if something went horribly wrong</returns>
        private static Chunk CreateChunk(SectionType sectionType, BinaryReaderEx reader, Chunk parent)
        {
            long position = reader.BaseStream.Position;
            int type = reader.ReadInt32();
            reader.BaseStream.Position = position;

            return CreateChunk(sectionType, type, parent);
        }

        /// <summary>Load a chunk from the data stream</summary>
        /// <param name="reader">BinaryReader pointed at the start of a chunk</param>
        /// <returns>A newly created and loaded chunk or null if something went horribly wrong</returns>
        public static Chunk LoadChunk(SectionType sectionType, BinaryReaderEx reader, INavigable parent)
        {
            Chunk parentChunk = parent as Chunk;
            Chunk result = CreateChunk(sectionType, reader, parentChunk);
            if (result != null)
            {
                result.Parent = parent;
                result.LoadData(reader);
            }

            return result;
        }
    }
}
