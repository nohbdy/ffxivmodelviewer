namespace DatDigger.Sections.Skeleton
{
    public class Header
    {
        public int Unknown1 { get; set; }

        /// <summary>Offset from end of header to beginning of StringTable</summary>
        public int StringTableOffset { get; set; }

        /// <summary>Number of entries in the string table</summary>
        public int NumStrings { get; set; }         // Number of strings in the string table

        /// <summary>The value 'lks\0'</summary>
        public int Lks { get; set; }                // 'lks\0'

        /// <summary>(Possibly) Index into string table to use as a name</summary>
        public int StringIndex { get; set; }

        public int Unknown2 { get; set; }

        /// <summary>Length in bytes of the Bone data section</summary>
        public int BoneDataLength { get; set; }

        public int Unknown3 { get; set; }
    }
}
