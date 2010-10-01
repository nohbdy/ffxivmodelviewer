using System.Collections.Generic;

namespace DatDigger.Xml
{
    public class Sheet
    {
        public Sheet()
        {
            InfoFile = 0;
            FileBlocks = new List<FileBlock>();
            Parameters = new List<Parameter>();
        }

        public string Name { get; set; }
        public SheetMode Mode { get; set; }
        public int ColumnMax { get; set; }
        public int ColumnCount { get; set; }
        public bool Cache { get; set; }
        public SheetType Type { get; set; }
        public Language Language { get; set; }
        public int InfoFile { get; set; }

        /// <summary>Determine whether this sheet is simply an external reference to another file</summary>
        public bool IsReference
        {
            get
            {
                return (InfoFile != 0);
            }
        }

        public List<FileBlock> FileBlocks { get; private set; }
        public List<Parameter> Parameters { get; private set; }
    }
}
