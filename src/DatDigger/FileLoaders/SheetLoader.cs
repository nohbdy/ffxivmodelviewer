using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using DatDigger.Xml;

namespace DatDigger.FileLoaders
{
    public class SheetLoader : FileLoaderBase
    {
        private string ns = String.Empty;

        public List<Sheet> Sheets { get; private set; }

        public SheetLoader()
        {
            this.Sheets = new List<Sheet>();
        }

        public override bool ReadFile(System.IO.BinaryReader reader)
        {
            return ReadFile(reader.BaseStream);
        }

        public bool ReadFile(System.IO.Stream stream, bool encoded = true)
        {
            byte[] dstBuffer;
            if (encoded)
            {
                byte[] xmlFileData;
                int fileLen = (int)stream.Length;
                xmlFileData = new byte[fileLen];
                stream.Read(xmlFileData, 0, fileLen);

                dstBuffer = DatDigger.Crypto.ShuffleString.Decode(xmlFileData, fileLen);
                string fileData = Encoding.UTF8.GetString(dstBuffer);
                fileData = null;
            }
            else
            {
                dstBuffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(dstBuffer, 0, (int)stream.Length);
            }

            try
            {
                using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(dstBuffer))
                {
                    var reader = XmlReader.Create(memStream);
                    var doc = new XmlDocument();
                    doc.Load(reader);

                    var nav = doc.DocumentElement.CreateNavigator();
                    var sheetElements = nav.SelectChildren("sheet", ns);

                    foreach (XPathNavigator sheet in sheetElements)
                    {
                        Sheet result = ReadSheet(sheet);
                        if (result != null)
                        {
                            this.Sheets.Add(result);
                        }
                    }
                }

                if (this.Sheets.Count == 0) { return false; }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private int ReadIntAttribute(XPathNavigator nav, string name, int defaultValue = 0)
        {
            var clone = nav.Clone();
            try
            {
                if (clone.MoveToAttribute(name, ns))
                {
                    return clone.ValueAsInt;
                }
            }
            catch
            {
                return defaultValue;
            }

            return defaultValue;
        }

        private string ReadStringAttribute(XPathNavigator nav, string name, string defaultValue = null)
        {
            var clone = nav.Clone();
            if (clone.MoveToAttribute(name, ns))
            {
                return clone.Value;
            }

            return defaultValue;
        }

        public static Language GetLanguage(string langStr)
        {
            switch (langStr)
            {
                case "ja":
                    return Language.Japanese;
                case "en":
                    return Language.English;
                case "fr":
                    return Language.French;
                case "de":
                    return Language.German;
                default:
                    return Language.Other;
            }
        }

        private Language ReadLanguageAttribute(XPathNavigator nav, string name, Language defaultValue = Language.None)
        {
            var clone = nav.Clone();
            if (clone.MoveToAttribute(name, ns))
            {
                return GetLanguage(clone.Value);
            }

            return defaultValue;
        }

        private Sheet ReadSheet(XPathNavigator nav)
        {
            Sheet result = new Sheet();
            var sheetElement = nav.Clone();
            result.Name = ReadStringAttribute(nav, "name", String.Empty);
            result.InfoFile = ReadIntAttribute(nav, "infofile");
            result.ColumnCount = ReadIntAttribute(nav, "column_count");
            result.ColumnMax = ReadIntAttribute(nav, "column_max");
            result.Language = ReadLanguageAttribute(nav, "lang");

            if (result.InfoFile == 0)
            {
                // Try to load parameter and fileblock information for this sheet
                var typeParamList = sheetElement.Select("./type/param");
                var idxParamList = sheetElement.Select("./index/param");
                var blockList = sheetElement.Select("./block/file");

                int pIdx = 1;
                foreach (XPathNavigator p in typeParamList)
                {
                    idxParamList.MoveNext();

                    Parameter param = new Parameter();
                    param.Order = pIdx;
                    param.Type = p.Value.ParseDataType();
                    param.Index = idxParamList.Current.ValueAsInt;
                    result.Parameters.Add(param);

                    pIdx++;
                }

                foreach (XPathNavigator block in blockList)
                {
                    result.FileBlocks.Add(ReadFileBlock(block));
                }
            }

            return result;
        }

        private FileBlock ReadFileBlock(XPathNavigator nav)
        {
            FileBlock result = new FileBlock();
            result.Begin = ReadIntAttribute(nav, "begin");
            result.Count = ReadIntAttribute(nav, "count");
            result.EnableFileId = ReadIntAttribute(nav, "enable");
            result.OffsetFileId = ReadIntAttribute(nav, "offset");
            result.FileId = nav.ValueAsInt;

            return result;
        }
    }
}
