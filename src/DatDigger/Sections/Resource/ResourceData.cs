namespace DatDigger.Sections.Resource
{
    public class ResourceData
    {
        public int Index { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public int Type { get; set; }

        public SectionBase Section { get; set; }
    }
}
