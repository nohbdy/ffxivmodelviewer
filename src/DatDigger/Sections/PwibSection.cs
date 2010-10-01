using System.Collections.Generic;

namespace DatDigger.Sections
{
    public class PwibSection : INavigable
    {
        public uint FileSize { get; protected set; }
        public int Unknown { get; protected set; }
        public uint DataOffset { get; protected set; }
        public string FilePath { get; internal set; }

        public SectionBase Section { get; protected set; }

        public void LoadData(BinaryReaderEx reader)
        {
            reader.ReadInt32(); // Remove PWIB bytes
            this.FileSize = reader.ReadUInt32(Endianness.BigEndian);
            this.Unknown = reader.ReadInt32(Endianness.BigEndian);
            this.DataOffset = reader.ReadUInt32(Endianness.BigEndian);

            this.LoadSection(reader);
        }

        private void LoadSection(BinaryReaderEx reader)
        {
            this.Children = new List<INavigable>();
            this.Section = SectionLoader.LoadSection(reader, this);
            this.Children.Add(this.Section);
        }

        public string DisplayName { get { return "PWIB"; } }
        public INavigable Parent { get; protected set; }
        public List<INavigable> Children { get; protected set; }
    }
}
