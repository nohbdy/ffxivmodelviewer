using System;
using System.ComponentModel;

namespace DatDigger.Sections.Model
{
    public class EnvdHeader
    {
        /// <summary>Offset to the bone name</summary>
        public short NameOffset { get; set; }

        /// <summary>Number of vertices that reference this bone</summary>
        public ushort NumVerts { get; set; }

        /// <summary>Offset to the beginning of the vertex index list</summary>
        public short VertsOffset { get; set; }

        /// <summary>Offset to the beginning of the weights list</summary>
        public short WeightsOffset { get; set; }

        /// <summary>Number of weights in this ENVD Chunk</summary>
        public ushort NumWeights { get; set; }

        /// <summary>Duplicate of VertsOffset?</summary>
        public short Unk1 { get; set; }

        /// <summary>Duplicate of WeightsOffset?</summary>
        public short Unk2 { get; set; }

        /// <summary>Unknown, seen 0x0000</summary>
        public short Unk3 { get; set; }
    }

    public class EnvdChunk : Chunk
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EnvdHeader EnvdHeader { get; protected set; }
        public string BoneName { get; protected set; }
        public short[] VertexIndices { get; protected set; }
        public byte[] VertexWeights { get; protected set; }
        private long headerOffset;

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.headerOffset = reader.BaseStream.Position;

            this.EnvdHeader = new EnvdHeader();
            this.EnvdHeader.NameOffset = reader.ReadInt16(Endianness.BigEndian);
            this.EnvdHeader.NumVerts = reader.ReadUInt16(Endianness.BigEndian);
            this.EnvdHeader.VertsOffset = reader.ReadInt16(Endianness.BigEndian);
            this.EnvdHeader.WeightsOffset = reader.ReadInt16(Endianness.BigEndian);
            this.EnvdHeader.NumWeights = reader.ReadUInt16(Endianness.BigEndian);
            this.EnvdHeader.Unk1 = reader.ReadInt16(Endianness.BigEndian);
            this.EnvdHeader.Unk2 = reader.ReadInt16(Endianness.BigEndian);
            this.EnvdHeader.Unk3 = reader.ReadInt16(Endianness.BigEndian);
            
            // Read String
            reader.BaseStream.Position = this.headerOffset + this.EnvdHeader.NameOffset;
            this.BoneName = reader.ReadNullTerminatedString();

            // Read list of vertices that reference this bone
	        reader.BaseStream.Position = this.headerOffset + this.EnvdHeader.VertsOffset;
            this.VertexIndices = new short[this.EnvdHeader.NumVerts];
            for (var i = 0; i < this.EnvdHeader.NumVerts; i++)
            {
                this.VertexIndices[i] = reader.ReadInt16(Endianness.BigEndian);
            }

	        // Read list of vertex weights (stored as bytes)
            reader.BaseStream.Position = this.headerOffset + this.EnvdHeader.WeightsOffset;
            this.VertexWeights = new byte[this.EnvdHeader.NumWeights];
            for (var i = 0; i < this.EnvdHeader.NumWeights; i++)
            {
                this.VertexWeights[i] = reader.ReadByte();
            }

            this.DisplayName = String.Format("ENVD [{0}]", this.BoneName);
        }
    }
}
