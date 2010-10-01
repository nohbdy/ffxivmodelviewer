using System.ComponentModel;

namespace DatDigger.Sections.Model
{
    public class PgrpHeader
    {
        public int NumTriangles { get; set; }
        public int TrianglesOffset { get; set; }
        public int NameOffset { get; set; }
    }

    public class PgrpChunk : Chunk
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PgrpHeader PgrpHeader { get; protected set; }
        public short[] Triangles { get; protected set; }
        public string PgrpName { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            long headerStart = reader.BaseStream.Position;

            this.PgrpHeader = new PgrpHeader();
            this.PgrpHeader.NumTriangles = reader.ReadInt32(Endianness.BigEndian);
            this.PgrpHeader.TrianglesOffset = reader.ReadInt32(Endianness.BigEndian);
            this.PgrpHeader.NameOffset = reader.ReadInt32(Endianness.BigEndian);

            //Read PGRP data
            reader.BaseStream.Position = headerStart + this.PgrpHeader.TrianglesOffset;
            this.Triangles = new short[this.PgrpHeader.NumTriangles];
            for (var i = 0; i < this.PgrpHeader.NumTriangles; i++)
            {
                this.Triangles[i] = reader.ReadInt16(Endianness.BigEndian);
            }

            // Read Name
            reader.BaseStream.Position = headerStart + this.PgrpHeader.NameOffset;
            this.PgrpName = reader.ReadNullTerminatedString();

            this.DisplayName = "PGRP [" + this.PgrpName + "]";
        }
    }
}
