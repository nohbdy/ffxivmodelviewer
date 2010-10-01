using System.Collections.Generic;

namespace DatDigger.Sections.Skeleton
{
    public class ElementOffsetData
    {
        public int BoneNameOffset { get; set; }
        public int NameOffset { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
        public int Unused1 { get; set; }
        public int Unused2 { get; set; }
        public int Unused3 { get; set; }

        public string BoneName { get; set; }
        public string Name { get; set; }
    }

    public class ElbSection : SectionBase
    {
        public int NumElements { get; set; }
        public List<ElementOffsetData> ElementOffsets { get; protected set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.NumElements = reader.ReadInt32();
            reader.BaseStream.Seek(12, System.IO.SeekOrigin.Current); // Skip 12 bytes

            this.ElementOffsets = new List<ElementOffsetData>();

            for (var i = 0; i < this.NumElements; i++)
            {
                var thing = new ElementOffsetData();
                thing.BoneNameOffset = reader.ReadInt32();
                thing.NameOffset = reader.ReadInt32();
                thing.OffsetX = reader.ReadSingle();
                thing.OffsetY = reader.ReadSingle();
                thing.OffsetZ = reader.ReadSingle();
                thing.Unused1 = reader.ReadInt32();
                thing.Unused2 = reader.ReadInt32();
                thing.Unused3 = reader.ReadInt32();
                this.ElementOffsets.Add(thing);
            }

            foreach (var el in this.ElementOffsets)
            {
                reader.BaseStream.Position = this.SectionStart + el.BoneNameOffset;
                el.BoneName = reader.ReadNullTerminatedString();
                reader.BaseStream.Position = this.SectionStart + el.NameOffset;
                el.Name = reader.ReadNullTerminatedString();
            }
        }
    }
}
