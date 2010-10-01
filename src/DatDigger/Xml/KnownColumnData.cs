using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace DatDigger.Xml
{
    public class KnownColumn
    {
        public string Table { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string LinkTo { get; set; }
        public string Format { get; set; }
    }

    public static class KnownColumnData
    {
        private static Dictionary<string, Dictionary<string, KnownColumn>> nameLookup = new Dictionary<string, Dictionary<string, KnownColumn>>();
        private static Dictionary<string, Dictionary<string, KnownColumn>> idLookup = new Dictionary<string, Dictionary<string, KnownColumn>>();
        private static string ns = String.Empty;

        public static string GetName(string file, int columnId)
        {
            string idAsStr;
            if (columnId == -1) { idAsStr = "ID"; }
            else { idAsStr = columnId.ToString(); }

            var data = GetById(file, idAsStr);
            if (data == null)
            {
                return idAsStr;
            }

            return data.Name;
        }

        public static string GetFormat(string file, int columnId)
        {
            string idAsStr;
            if (columnId == -1) { idAsStr = "ID"; }
            else { idAsStr = columnId.ToString(); }

            var data = GetById(file, idAsStr);
            if (data == null)
            {
                return null;
            }

            return data.Format;
        }

        public static string GetLinkTo(string file, string columnName)
        {
            var col = GetByName(file, columnName);
            if (col == null) { return null; }

            return col.LinkTo;
        }

        private static KnownColumn GetById(string file, string columnId)
        {
            Dictionary<string, KnownColumn> idToData;
            if (!idLookup.TryGetValue(file, out idToData))
            {
                return null;
            }

            KnownColumn result;
            if (idToData.TryGetValue(columnId, out result))
            {
                return result;
            }

            return null;
        }

        private static KnownColumn GetByName(string file, string columnName)
        {
            Dictionary<string, KnownColumn> nameToData;
            if (!nameLookup.TryGetValue(file, out nameToData))
            {
                return null;
            }

            KnownColumn result;
            if (nameToData.TryGetValue(columnName, out result))
            {
                return result;
            }

            return null;
        }

        public static void ReadData(string filePath)
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                var nav = doc.DocumentElement.CreateNavigator();
                var columns = nav.SelectChildren("column", ns);

                foreach (XPathNavigator column in columns)
                {
                    var data = new KnownColumn();
                    data.Name = column.GetAttribute("column_name", ns);
                    data.Id = column.GetAttribute("column_id", ns);
                    data.Table = column.GetAttribute("table", ns);
                    data.LinkTo = column.GetAttribute("link_to", ns);
                    data.Format = column.GetAttribute("format", ns);

                    Dictionary<string, KnownColumn> subLookup;
                    if (idLookup.TryGetValue(data.Table, out subLookup))
                    {
                        subLookup[data.Id] = data;
                    }
                    else
                    {
                        subLookup = new Dictionary<string, KnownColumn>();
                        subLookup[data.Id] = data;
                        idLookup[data.Table] = subLookup;
                    }

                    if (nameLookup.TryGetValue(data.Table, out subLookup))
                    {
                        subLookup[data.Name] = data;
                    }
                    else
                    {
                        subLookup = new Dictionary<string, KnownColumn>();
                        subLookup[data.Name] = data;
                        nameLookup[data.Table] = subLookup;
                    }
                }
            }
        }
    }
}
