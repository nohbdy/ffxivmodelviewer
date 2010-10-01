namespace DatDigger.Sections.Resource
{
    public class ResourceHeader
    {
        /// <summary>Number of Resources contained in this section</summary>
        public int NumResources { get; set; }

        /// <summary>Offset from end of Resource list to beginning of string table</summary>
        public int StringTableOffset { get; set; }

        /// <summary>Number of strings that are contained in the string table</summary>
        public int NumStrings { get; set; }

        public SectionType ResourceType { get; set; }
    }
}
