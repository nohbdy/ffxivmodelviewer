using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Texture
{
    public class VtexHeader
    {
        public int Unknown1 { get; set; }//* 00 00 00 00
        public int DataLength { get; set; }//* B4 40 00 00 
        public short Unknown2 { get; set; }//* 01 00 
        public short Unknown3 { get; set; }//* 01 00 
        public short Unknown4 { get; set; }//* 02 00
        public short Unknown5 { get; set; }//* 02 00
        public int Unknown6 { get; set; }//* 1C 00 00 00 
        public int Unknown7 { get; set; }//* 18 00 00 00 
        public int Unknown8 { get; set; }//* 3C 00 00 00 
        public int Unknown9 { get; set; }//* 30 00 00 00
        public int Unknown10 { get; set; }//* 2C 00 00 00
        public int Unknown11 { get; set; }//* 01 00 00 00
        public int Unknown12 { get; set; }//* 01 00 00 00
        public int Unknown13 { get; set; }//* 1C 00 00 00
        public int Unknown14 { get; set; }//* 3C 00 00 00
        public int Unknown15 { get; set; }//* 01 00 00 00
        public int Unknown16 { get; set; }//* 28 00 00 00
        public string Name { get; set; }//* 30 45 30 49 31 77 68 69 74 30 5F 66 30 32 74 00
        public string Extension { get; set; }//* 78 65 74 76 00 00 00 00
        public int Unknown17 { get; set; }//* 01 00 00 00
        public int GtexOffset { get; set; } // 64 00 00 00
        public int Unknown18 { get; set; }// 20 40 00 00
        public int Unknown19 { get; set; }// 00 00 00 00
    }

    public class VtexSection : SectionBase
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VtexHeader VtexHeader { get; protected set; }
        public GtexData Gtex { get; protected set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.VtexHeader = new VtexHeader();
            this.VtexHeader.Unknown1 = reader.ReadInt32();
            this.VtexHeader.DataLength = reader.ReadInt32();
            this.VtexHeader.Unknown2 = reader.ReadInt16();
            this.VtexHeader.Unknown3 = reader.ReadInt16();
            this.VtexHeader.Unknown4 = reader.ReadInt16();
            this.VtexHeader.Unknown5 = reader.ReadInt16();
            this.VtexHeader.Unknown6 = reader.ReadInt32();
            this.VtexHeader.Unknown7 = reader.ReadInt32();
            this.VtexHeader.Unknown8 = reader.ReadInt32();
            this.VtexHeader.Unknown9 = reader.ReadInt32();
            this.VtexHeader.Unknown10 = reader.ReadInt32();
            this.VtexHeader.Unknown11 = reader.ReadInt32();
            this.VtexHeader.Unknown12 = reader.ReadInt32();
            this.VtexHeader.Unknown13 = reader.ReadInt32();
            this.VtexHeader.Unknown14 = reader.ReadInt32();
            this.VtexHeader.Unknown15 = reader.ReadInt32();
            this.VtexHeader.Unknown16 = reader.ReadInt32();
            this.VtexHeader.Name = reader.ReadFixedLengthString(16).TrimEnd('\0');
            this.VtexHeader.Extension = reader.ReadFixedLengthString(8).TrimEnd('\0');
            this.VtexHeader.Unknown17 = reader.ReadInt32();
            this.VtexHeader.GtexOffset = reader.ReadInt32();
            this.VtexHeader.Unknown18 = reader.ReadInt32();
            this.VtexHeader.Unknown19 = reader.ReadInt32();

            reader.BaseStream.Position = this.SectionStart + 0x30 + this.VtexHeader.GtexOffset;

            // Load GTEX
            Gtex = new GtexData();
            Gtex.Parent = this;
            Gtex.LoadSection(reader);

            this.Children = new List<INavigable>();
            this.Children.Add(this.Gtex);
        }
    }
}
