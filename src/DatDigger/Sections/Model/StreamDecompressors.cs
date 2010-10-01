using System.Collections.Generic;

namespace DatDigger.Sections.Model
{
    public delegate float FloatStreamDecompressor(BinaryReaderEx src);
    public delegate ushort UInt16StreamDecompressor(BinaryReaderEx src);
    public delegate int Int32StreamDecompressor(BinaryReaderEx src);

    public static class StreamDecompressors
    {
        private static Dictionary<StreamCompressionType, FloatStreamDecompressor> floatDecompressors = new Dictionary<StreamCompressionType, FloatStreamDecompressor>();
        private static Dictionary<StreamCompressionType, UInt16StreamDecompressor> ushortDecompressors = new Dictionary<StreamCompressionType, UInt16StreamDecompressor>();
        private static Dictionary<StreamCompressionType, Int32StreamDecompressor> intDecompressors = new Dictionary<StreamCompressionType, Int32StreamDecompressor>();

        private static float FloatToFloat(BinaryReaderEx src)
        {
            return src.ReadSingle(Endianness.BigEndian);
        }

        private static float SByteToFloat(BinaryReaderEx src)
        {
            sbyte val = src.ReadSByte();
            return val / 127f;
        }

        private static float ByteToFloat(BinaryReaderEx src)
        {
            byte val = src.ReadByte();
            return val / 255f;
        }

        private static float ShortToFloat(BinaryReaderEx src)
        {
            short val = src.ReadInt16(Endianness.BigEndian);
            return val / 32768.0f;
        }

        private static float UShortToFloat(BinaryReaderEx src)
        {
            ushort val = src.ReadUInt16(Endianness.BigEndian);
            return val / 65535.0f;
        }

        private static float HalfToFloat(BinaryReaderEx src)
        {
            ushort val = src.ReadUInt16(Endianness.BigEndian);
            return HalfHelper.HalfToSingle(val);
        }

        private static ushort UShortToShort(BinaryReaderEx src)
        {
            return src.ReadUInt16(Endianness.BigEndian);
        }

        private static int SByteToInt(BinaryReaderEx src)
        {
            return (int)src.ReadSByte();
        }

        static StreamDecompressors()
        {
            floatDecompressors.Add(StreamCompressionType.Float, FloatToFloat);
            floatDecompressors.Add(StreamCompressionType.SByte, SByteToFloat);
            floatDecompressors.Add(StreamCompressionType.Byte, ByteToFloat);
            floatDecompressors.Add(StreamCompressionType.Int16, ShortToFloat);
            floatDecompressors.Add(StreamCompressionType.Half, HalfToFloat);
            floatDecompressors.Add(StreamCompressionType.UInt16, UShortToFloat);

            intDecompressors.Add(StreamCompressionType.SByte, SByteToInt);

            ushortDecompressors.Add(StreamCompressionType.UInt16, UShortToShort);
        }

        public static FloatStreamDecompressor GetFloatDecompressor(StreamCompressionType compressedType)
        {
            return floatDecompressors[compressedType];
        }

        public static UInt16StreamDecompressor GetUShortDecompressor(StreamCompressionType compressedType)
        {
            return ushortDecompressors[compressedType];
        }

        public static Int32StreamDecompressor GetIntDecompressor(StreamCompressionType compressedType)
        {
            return intDecompressors[compressedType];
        }
    }
}
