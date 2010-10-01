using System.Collections.Generic;

namespace DatDigger.Sections.Shader
{
    public class SamplerData : INavigable
    {
        public SamplerData()
        {
            this.StringTable = new List<string>();
        }

        /// <summary>Magic Number? Seems to always be 0x102</summary>
        public short Magic { get; set; }

        /// <summary>Length in bytes of the string table</summary>
        public byte StringTableLength { get; set; }

        /// <summary>Value varies, seems it could mean something</summary>
        public byte Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        public int Unknown5 { get; set; }

        public int Unknown6 { get; set; }

        public int Unknown7 { get; set; }

        public int Unknown8 { get; set; }

        public string Name { get; set; }

        public List<string> StringTable { get; set; }

        public string DisplayName { get { return Name; } }

        public INavigable Parent { get; set; }

        public List<INavigable> Children { get { return null; } }
    }
}
