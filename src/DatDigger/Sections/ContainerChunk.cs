using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections
{
    public class ContainerChunk : Chunk
    {
        public int NumChildren { get; protected set; }
        public int Unknown1 { get; protected set; }
        public int Unknown2 { get; protected set; }
        public int Unknown3 { get; protected set; }

        [Browsable(false)]
        public List<Chunk> SubChunks { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.NumChildren = reader.ReadInt32(Endianness.BigEndian);
            this.Unknown1 = reader.ReadInt32(Endianness.BigEndian);
            this.Unknown2 = reader.ReadInt32(Endianness.BigEndian);
            this.Unknown3 = reader.ReadInt32(Endianness.BigEndian);

            this.LoadAdditionalData(reader);
            this.LoadChildren(reader);
        }

        /// <summary>Give Container nodes the opportunity to load any additional data between the end of the Container header and beginning of child chunks</summary>
        /// <param name="reader">A BinaryReader positioned immediately after container header</param>
        protected virtual void LoadAdditionalData(BinaryReaderEx reader) { }

        /// <summary>Read the chunks that are child to this container chunk</summary>
        /// <param name="reader">A BinaryReader positioned at the start of the first child chunk</param>
        protected virtual void LoadChildren(BinaryReaderEx reader)
        {
            this.SubChunks = new List<Chunk>(this.NumChildren);
            this.Children = new List<INavigable>(this.NumChildren);

            for (var i = 0; i < this.NumChildren; i++)
            {
                long offset = reader.BaseStream.Position;
                Chunk childChunk = ChunkLoader.LoadChunk(this.SectionType, reader, this);
                this.SubChunks.Add(childChunk);
                this.Children.Add(childChunk);

                reader.BaseStream.Position = offset + childChunk.ChunkHeader.NextChunkOffset;
            }
        }
    }
}
