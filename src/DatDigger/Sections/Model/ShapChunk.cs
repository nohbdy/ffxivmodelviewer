using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Model
{
    public class ShapHeader
    {
        public int NumThings { get; set; }
        public int ThingOffset { get; set; }
        public int NumThings2 { get; set; }
        public int Thing2Offset { get; set; }
        public int NameOffset { get; set; }
        public int Unknown { get; set; }
    }

    public class ShapThing1
    {
        public short Unk1 { get; set; }
        public short Unk2 { get; set; }
    }

    public class ShapThing2
    {
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
    }

    public class ShapChunk : Chunk
    {
        private long shapOffset;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ShapHeader ShapHeader { get; protected set; }
        public string Name { get; protected set; }
        public List<ShapThing1> Things { get; protected set; }
        public List<ShapThing2> Things2 { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            shapOffset = reader.BaseStream.Position;
            this.ShapHeader = new ShapHeader();
            this.ShapHeader.NumThings = reader.ReadInt32(Endianness.BigEndian);
            this.ShapHeader.ThingOffset = reader.ReadInt32(Endianness.BigEndian);
            this.ShapHeader.NumThings2 = reader.ReadInt32(Endianness.BigEndian);
            this.ShapHeader.Thing2Offset = reader.ReadInt32(Endianness.BigEndian);
            this.ShapHeader.NameOffset = reader.ReadInt32(Endianness.BigEndian);
            this.ShapHeader.Unknown = reader.ReadInt32(Endianness.BigEndian);

            reader.BaseStream.Position = shapOffset + this.ShapHeader.ThingOffset;
            this.Things = new List<ShapThing1>();
            for (var i = 0; i < this.ShapHeader.NumThings; i++)
            {
                ShapThing1 t = new ShapThing1();
                t.Unk1 = reader.ReadInt16(Endianness.BigEndian);
                t.Unk2 = reader.ReadInt16(Endianness.BigEndian);
                this.Things.Add(t);
            }

            reader.BaseStream.Position = shapOffset + this.ShapHeader.Thing2Offset;
            this.Things2 = new List<ShapThing2>();
            for (var j = 0; j < this.ShapHeader.NumThings2; j++)
            {
                ShapThing2 t = new ShapThing2();
                t.Unk1 = reader.ReadSingle(Endianness.BigEndian);
                t.Unk2 = reader.ReadSingle(Endianness.BigEndian);
                t.Unk3 = reader.ReadSingle(Endianness.BigEndian);
                this.Things2.Add(t);
            }

            reader.BaseStream.Position = shapOffset + this.ShapHeader.NameOffset;
            this.Name = reader.ReadNullTerminatedString();

            this.DisplayName = String.Format("SHAP [{0}]", this.Name);
        }
    }
}
