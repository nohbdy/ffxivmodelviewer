using System;

namespace DatDigger.Xml.Format
{
    public interface IFormatCell
    {
        Type GetStorageType(Type sourceType);
        object Format(object val);
    }
}
