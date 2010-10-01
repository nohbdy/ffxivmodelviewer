using System.Collections.Generic;

namespace DatDigger.Sections.Animation
{
    public class CibtData
    {
        public string Name { get; set; }
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
        public byte Unk4 { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class CibtSection : CibSection
    {
        private const int bytesPerBlock = 0x14;

        public List<CibtData> CibtData { get; set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.CibtData = new List<CibtData>();
            int numCibtBlocks = (this.SectionLength - 4) / bytesPerBlock;
            for (var i = 0; i < numCibtBlocks; i++)
            {
                var cibt = new CibtData();
                cibt.Name = reader.ReadFixedLengthString(16).Trim();
                cibt.Unk1 = reader.ReadByte();
                cibt.Unk2 = reader.ReadByte();
                cibt.Unk3 = reader.ReadByte();
                cibt.Unk4 = reader.ReadByte();
                this.CibtData.Add(cibt);
            }
        }
    }
}
