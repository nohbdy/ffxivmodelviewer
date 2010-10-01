using System;

namespace DatDigger.Xml.Format
{
    public enum IconType
    {
        IconType0 = 0,
        IconType1 = 1,
        IconType2 = 2,
        Ability = 3,
        IconType4 = 4,
        IconType5 = 5,
        Item = 6,
        Weapon = 7,
        Armor = 8,
        IconType9 = 9
    }

    public class IconValue
    {
        public int RealValue { get; set; }
        public int IconFile { get; set; }
        public IconType IconType { get; set; }

        public override string ToString()
        {
            return String.Format("1C/{0:X2}/{1:X2}/{2:X2}.DAT", 0x59 + (int)IconType, IconFile >> 8, IconFile & 0xFF);
        }
    }

    public class IconFormatter : IFormatCell
    {
        public Type GetStorageType(Type srcType)
        {
            return typeof(IconValue);
        }

        public object Format(object val)
        {
            int ival = (int)val;
            IconValue result = new IconValue()
            {
                RealValue = ival,
                IconFile = ival % 10000,
                IconType = (IconType)(ival / 10000)
            };

            return result;
        }
    }
}
