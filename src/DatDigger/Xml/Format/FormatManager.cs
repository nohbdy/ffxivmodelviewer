using System;
using System.Collections.Generic;

namespace DatDigger.Xml.Format
{
    public static class FormatManager
    {
        private static IFormatCell defaultFormatter = new IdentityFormatter();
        private static Dictionary<string, IFormatCell> formatLookup = new Dictionary<string, IFormatCell>()
        {
            { "Icon", new IconFormatter() },
            { "Model", new ModelFormatter() }
        };

        public static IFormatCell DefaultFormatter { get { return defaultFormatter; } }

        public static IFormatCell GetFormatter(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return defaultFormatter;
            }

            IFormatCell result;
            if (formatLookup.TryGetValue(id, out result))
            {
                return result;
            }

            return defaultFormatter;
        }
    }
}
