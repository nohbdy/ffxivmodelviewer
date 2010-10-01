using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DatDigger.FileLoaders
{
    public class SheetDataLoader
    {
        private const byte xorValue = 0x73;
        private static Encoding encoding = Encoding.UTF8;

        public DataTable Data { get; private set; }

        public bool ReadSheet(Xml.Sheet sheet)
        {
            try
            {
                this.Data = new DataTable();
                BuildColumns(sheet);

                List<Xml.Format.IFormatCell> formatters = new List<Xml.Format.IFormatCell>(sheet.Parameters.Count + 1);
                formatters.Add(Xml.Format.FormatManager.DefaultFormatter);
                foreach (Xml.Parameter p in sheet.Parameters)
                {
                    string format = Xml.KnownColumnData.GetFormat(sheet.Name, p.Index);
                    formatters.Add(Xml.Format.FormatManager.GetFormatter(format));
                }

                foreach (Xml.FileBlock block in sheet.FileBlocks)
                {
                    using (FileStream enableStream = File.Open(Utilities.DataFileIdToPath(block.EnableFileId), FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (FileStream dataStream = File.Open(Utilities.DataFileIdToPath(block.FileId), FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        BinaryReader enableReader = new BinaryReader(enableStream);
                        BinaryReader dataReader = new BinaryReader(dataStream);

                        while (enableStream.Position != enableStream.Length)
                        {
                            int startId = enableReader.ReadInt32();
                            int numIds = enableReader.ReadInt32();
                            for (var i = 0; i < numIds; i++)
                            {
                                DataRow row = this.Data.NewRow();
                                row[0] = startId + i;
                                foreach (Xml.Parameter p in sheet.Parameters)
                                {
                                    row[p.Order] = formatters[p.Order].Format(p.Type.ReadData(dataReader));
                                }
                                this.Data.Rows.Add(row);
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                this.Data = null;
                return false;
            }
        }

        private void BuildColumns(Xml.Sheet sheet)
        {
            List<Xml.Format.IFormatCell> formatters = new List<Xml.Format.IFormatCell>(sheet.Parameters.Count + 1);
            formatters.Add(Xml.Format.FormatManager.GetFormatter(Xml.KnownColumnData.GetFormat(sheet.Name, 0)));

            foreach (Xml.Parameter p in sheet.Parameters)
            {
                string format = Xml.KnownColumnData.GetFormat(sheet.Name, p.Index);
                formatters.Add(Xml.Format.FormatManager.GetFormatter(format));
            }

            var idType = formatters[0].GetStorageType(typeof(int));
            var idName = Xml.KnownColumnData.GetName(sheet.Name, -1);
            this.Data.Columns.Add(idName, idType);
            foreach (Xml.Parameter p in sheet.Parameters)
            {
                var type = formatters[p.Order].GetStorageType(p.Type.GetRealType());
                string columnName = Xml.KnownColumnData.GetName(sheet.Name, p.Index);
                var col = this.Data.Columns.Add(columnName, type);
            }
        }

        public static DataTable LoadSingleSheet(int fileId)
        {
            string xmlPath = DatDigger.Utilities.DataFileIdToPath(fileId);

            List<DatDigger.Xml.Sheet> sheets;
            using (FileStream stream = File.Open(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loader = new DatDigger.FileLoaders.SheetLoader();
                if (!loader.ReadFile(stream))
                {
                    throw new InvalidOperationException("Unable to load XML from " + xmlPath);
                }

                sheets = loader.Sheets;
            }

            if (sheets.Count != 1)
            {
                throw new InvalidOperationException("Unexpected number of sheets - expected 1, found " + sheets.Count.ToString());
            }

            var r = new DatDigger.FileLoaders.SheetDataLoader();
            if (!r.ReadSheet(sheets[0]))
            {
                throw new InvalidOperationException("Unable to load data from sheet");
            }

            return r.Data;
        }

        public static DataTable LoadLanguageSheet(int fileId, Language lang)
        {
            string xmlPath = DatDigger.Utilities.DataFileIdToPath(fileId);

            List<DatDigger.Xml.Sheet> sheets;
            using (FileStream stream = File.Open(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loader = new DatDigger.FileLoaders.SheetLoader();
                if (!loader.ReadFile(stream))
                {
                    throw new InvalidOperationException("Unable to load XML from " + xmlPath);
                }

                sheets = loader.Sheets;
            }

            Xml.Sheet sheet = null;
            foreach (var s in sheets)
            {
                if (s.Language == lang)
                {
                    sheet = s;
                    break;
                }
            }

            if (sheet == null)
            {
                throw new InvalidOperationException("Could not find sheet with language " + lang + " in XML " + xmlPath);
            }

            var r = new DatDigger.FileLoaders.SheetDataLoader();
            if (!r.ReadSheet(sheet))
            {
                throw new InvalidOperationException("Unable to load data from sheet");
            }

            return r.Data;
        }
    }
}
