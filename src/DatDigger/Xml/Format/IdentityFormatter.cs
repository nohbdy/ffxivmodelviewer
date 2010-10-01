using System;

namespace DatDigger.Xml.Format
{
    public class IdentityFormatter : IFormatCell
    {
        public Type GetStorageType(Type sourceType)
        {
            return sourceType;
        }

        public object Format(object val)
        {
            return val;
        }
    }
}
