using System;
using System.IO;
using System.Text;

namespace DatDigger
{
    public static class DataTypeExtensions
    {
        private const byte xorValue = 0x73;
        private static Encoding encoding = Encoding.UTF8;

        public static Type GetRealType(this DataType type)
        {
            switch (type)
            {
                case DataType.String:
                    return typeof(String);
                case DataType.SignedByte:
                    return typeof(SByte);
                case DataType.Float:
                    return typeof(Single);
                case DataType.HalfFloat:
                    return typeof(Single);
                case DataType.Int:
                    return typeof(Int32);
                case DataType.UnsignedInt:
                    return typeof(UInt32);
                case DataType.UnsignedByte:
                    return typeof(Byte);
                case DataType.Boolean:
                    return typeof(Boolean);
                case DataType.Short:
                    return typeof(Int16);
                case DataType.UnsignedShort:
                    return typeof(UInt16);
                default:
                    throw new InvalidOperationException("Cannot determine real type of data type: " + type);
            }
        }

        private static void DecryptString(byte[] stringData)
        {
            for (int i = 0; i < stringData.Length; i++)
            {
                stringData[i] = (byte)(stringData[i] ^ xorValue);
            }
        }

        private static string ReadString(BinaryReader reader)
        {
            int strlen = (int)reader.ReadInt16();
            byte unknown = reader.ReadByte(); // 0xFF
            byte[] stringData = reader.ReadBytes(strlen - 1);

            DecryptString(stringData);
            return encoding.GetString(stringData);
        }

        public static object ReadData(this DataType type, BinaryReader reader)
        {
            switch (type)
            {
                case DataType.Boolean:
                    byte b = reader.ReadByte();
                    return (b == 0) ? false : true;
                case DataType.Float:
                    return reader.ReadSingle();
                case DataType.HalfFloat:
                    {
                        ushort val = reader.ReadUInt16();
                        return HalfHelper.HalfToSingle(val);
                    }
                case DataType.Int:
                    return reader.ReadInt32();
                case DataType.UnsignedInt:
                    return reader.ReadUInt32();
                case DataType.Short:
                    return reader.ReadInt16();
                case DataType.SignedByte:
                    return reader.ReadSByte();
                case DataType.String:
                    return ReadString(reader);
                case DataType.UnsignedByte:
                    return reader.ReadByte();
                case DataType.UnsignedShort:
                    return reader.ReadUInt16();
                default:
                    throw new InvalidOperationException("Unknown data type " + type);
            }
        }

        public static DataType ParseDataType(this string p)
        {
            switch (p)
            {
                case "str":
                    return DataType.String;
                case "s8":
                    return DataType.SignedByte;
                case "float":
                    return DataType.Float;
                case "f16":
                    return DataType.HalfFloat;
                case "s32":
                    return DataType.Int;
                case "u32":
                    return DataType.UnsignedInt;
                case "u8":
                    return DataType.UnsignedByte;
                case "bool":
                    return DataType.Boolean;
                case "s16":
                    return DataType.Short;
                case "u16":
                    return DataType.UnsignedShort;
                default:
                    throw new InvalidOperationException("Found unexpected parameter type: " + p);
            }
        }
    }

    public enum DataType
    {
        String,
        SignedByte,
        UnsignedByte,
        Int,
        UnsignedInt,
        Float,
        HalfFloat,
        Boolean,
        Short,
        UnsignedShort
    }
}
