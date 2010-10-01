using System.ComponentModel;

namespace DatDigger
{
    /// <summary>Header for a SEDB section</summary>
    public class SectionHeader
    {
        public long Magic { get; set; }   // 8-byte magic number in the form 'SEDBxxxx'
        public int Version { get; set; }
        public int Unknown2 { get; set; }
        public int SectionLength { get; set; }  // This includes the 48 bytes for the SectionHeader

        [Browsable(false)]
        public byte[] Junk { get; set; }        // 28 bytes of nothing! (unless I'm missing something)
    }
}
