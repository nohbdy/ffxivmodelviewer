namespace DatDigger.Sections.Model
{
    public class MeshHeaderChunk : Chunk
    {
        public byte Unknown0 { get; private set; } //Bit Field
        public byte PgrpCount { get; private set; } // Number of PGRP chunks
        public byte StmsCount { get; private set; } // Number of STMS chunks
        public byte EnvdCount { get; private set; } // Number of ENVD chunks
        public int Unknown1 { get; private set; }
        public byte Unknown2 { get; private set; }
        public byte Unknown3 { get; private set; }
        public byte Unknown4 { get; private set; }
        public byte Unknown5 { get; private set; }
        public byte ShapCount { get; private set; } // Number of SHAP chunks
        public byte MimsCount { get; private set; } // Number of MIMS chunks
        public byte Unknown6 { get; private set; }
        public byte Unknown7 { get; private set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "Header";

            this.Unknown0 = reader.ReadByte();
            this.PgrpCount = reader.ReadByte();
            this.StmsCount = reader.ReadByte();
            this.EnvdCount = reader.ReadByte();
            this.Unknown1 = reader.ReadInt32(Endianness.BigEndian);
            this.Unknown2 = reader.ReadByte();
            this.Unknown3 = reader.ReadByte();
            this.Unknown4 = reader.ReadByte();
            this.Unknown5 = reader.ReadByte();
            this.ShapCount = reader.ReadByte();
            this.MimsCount = reader.ReadByte();
            this.Unknown6 = reader.ReadByte();
            this.Unknown7 = reader.ReadByte();
        }
    }
}
