﻿using System;
using System.Runtime.InteropServices;

namespace DatDigger
{
    /// <summary>
    /// Helper class for Half conversions and some low level operations.
    /// This class is internally used in the Half class.
    /// </summary>
    /// <remarks>
    /// References:
    ///     - Fast Half Float Conversions, Jeroen van der Zijp, link: http://www.fox-toolkit.org/ftp/fasthalffloatconversion.pdf
    /// </remarks>
    [ComVisible(false)]
    public static class HalfHelper
    {
        private static uint[] mantissaTable = GenerateMantissaTable();
        private static uint[] exponentTable = GenerateExponentTable();
        private static ushort[] offsetTable = GenerateOffsetTable();

        // Transforms the subnormal representation to a normalized one. 
        private static uint ConvertMantissa(int i)
        {
            uint m = (uint)(i << 13); // Zero pad mantissa bits
            uint e = 0; // Zero exponent

            // While not normalized
            while ((m & 0x00800000) == 0)
            {
                e -= 0x00800000; // Decrement exponent (1<<23)
                m <<= 1; // Shift mantissa                
            }
            m &= unchecked((uint)~0x00800000); // Clear leading 1 bit
            e += 0x38800000; // Adjust bias ((127-14)<<23)
            return m | e; // Return combined number
        }

        private static uint[] GenerateMantissaTable()
        {
            uint[] mantissaTable = new uint[2048];
            mantissaTable[0] = 0;
            for (int i = 1; i < 1024; i++)
            {
                mantissaTable[i] = ConvertMantissa(i);
            }
            for (int i = 1024; i < 2048; i++)
            {
                mantissaTable[i] = (uint)(0x38000000 + ((i - 1024) << 13));
            }

            return mantissaTable;
        }
        private static uint[] GenerateExponentTable()
        {
            uint[] exponentTable = new uint[64];
            exponentTable[0] = 0;
            for (int i = 1; i < 31; i++)
            {
                exponentTable[i] = (uint)(i << 23);
            }
            exponentTable[31] = 0x47800000;
            exponentTable[32] = 0x80000000;
            for (int i = 33; i < 63; i++)
            {
                exponentTable[i] = (uint)(0x80000000 + ((i - 32) << 23));
            }
            exponentTable[63] = 0xc7800000;

            return exponentTable;
        }
        private static ushort[] GenerateOffsetTable()
        {
            ushort[] offsetTable = new ushort[64];
            offsetTable[0] = 0;
            for (int i = 1; i < 32; i++)
            {
                offsetTable[i] = 1024;
            }
            offsetTable[32] = 0;
            for (int i = 33; i < 64; i++)
            {
                offsetTable[i] = 1024;
            }

            return offsetTable;
        }

        public static float HalfToSingle(ushort half)
        {
            uint result = mantissaTable[offsetTable[half >> 10] + (half & 0x3ff)] + exponentTable[half >> 10];
            return BitConverter.ToSingle(BitConverter.GetBytes(result), 0); // Unsafe Equivalent: return *((float*)&result);
        }
    }
}