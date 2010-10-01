using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace DatDigger.Sections.Model
{
    public class StreamChunk : Chunk
    {
        /// <summary>Number of elements per vertex in this stream</summary>
        public int NumElements { get; protected set; }

        /// <summary>Number of Vertices in this stream</summary>
        public int VertexCount { get; protected set; }

        /// <summary>Size in bytes of each vertex in the compressed version of this stream</summary>
        public int CompressedBytesPerVertex { get; protected set; }

        public int Unknown1 { get; protected set; }

        public List<StreamElement> Elements { get; protected set; }

        /// <summary>Size in Bytes of each vertex in this stream</summary>
        public int VertexSize { get; protected set; }

        private byte[] vertexData;

        [Browsable(false)]
        public byte[] VertexData { get { return vertexData; } }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("STMS [0x{0:X}]", this.ChunkStart);

            this.NumElements = reader.ReadInt32(Endianness.BigEndian);
            this.VertexCount = reader.ReadInt32(Endianness.BigEndian);
            this.CompressedBytesPerVertex = reader.ReadInt32(Endianness.BigEndian);
            this.Unknown1 = reader.ReadInt32(Endianness.BigEndian);

            this.LoadElements(reader);
            this.DecompressStream(reader);
        }

        private void LoadElements(BinaryReaderEx reader)
        {
            this.Elements = new List<StreamElement>(this.NumElements);

            for (var i = 0; i < this.NumElements; i++)
            {
                StreamElement element = new StreamElement();
                element.Unknown1 = reader.ReadInt16(Endianness.BigEndian);
                element.OffsetInVertex = reader.ReadInt16(Endianness.BigEndian);
                element.CompressionType = (StreamCompressionType)reader.ReadInt32(Endianness.BigEndian);
                element.NumElements = reader.ReadInt32(Endianness.BigEndian);
                element.DataType = StreamDataType.GetStreamDataType(reader.ReadInt16(Endianness.BigEndian));
                element.Unknown2 = reader.ReadInt16(Endianness.BigEndian);

                this.Elements.Add(element);
            }
        }

        private int CalculateVertexSize()
        {
            int result = 0;
            foreach (StreamElement element in this.Elements)
            {
                element.UncompressedOffsetInVertex = result;
                if (element.DataType.Id == (short)StreamDataTypeId.BoneIndex || element.DataType.Id == (short)StreamDataTypeId.BoneWeight)
                {
                    // Number of bones per vert may vary so calculate the real size used (4 bytes per bone weight or bone index)
                    result += Math.Min(element.NumElements, StreamDataType.MaxBonesPerVert) * 4;
                }
                else
                {
                    result += element.DataType.DataSize;
                }
            }

            return result;
        }

        private void DecompressStream(BinaryReaderEx reader)
        {
            this.VertexSize = CalculateVertexSize();
            this.vertexData = new byte[this.VertexSize * this.VertexCount];

            long srcVertStart;

            using (MemoryStream dstStream = new MemoryStream(this.VertexData))
            using (BinaryWriter writer = new BinaryWriter(dstStream))
            {
                for (var vertexIndex = 0; vertexIndex < this.VertexCount; vertexIndex++)
                {
                    srcVertStart = reader.BaseStream.Position; // Store position to beginning of this vertex
                    foreach (StreamElement element in this.Elements)
                    {
                        reader.BaseStream.Position = srcVertStart + element.OffsetInVertex; // Move to correct offset in source vertex
                        element.DataType.Decompress(reader, writer, element);
                    }

                    reader.BaseStream.Position = srcVertStart + this.CompressedBytesPerVertex; // Move to next vertex
                }
            }
        }
    }
}
