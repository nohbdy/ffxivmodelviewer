using System;

namespace DatDigger.Crypto
{
    public static class ShuffleString
    {
        public static byte[] Decode(byte[] srcBuffer, int srcLen)
        {
            byte[] dstBuffer;
            if (srcBuffer[srcLen - 1] != 0xF1)
            {
                dstBuffer = new byte[srcLen];
                srcBuffer.CopyTo(dstBuffer, 0);
                return dstBuffer;
            }

            dstBuffer = new byte[srcLen - 1];
            int dstLen = srcLen - 1;

            unsafe
            {
                int encodedLen = srcLen - 1;
                if (encodedLen > dstLen)
                {
                    throw new InvalidOperationException("Destination buffer of insufficient size");
                }

                // Initialize dstBuffer to be the same as srcBuffer
                Array.Copy(srcBuffer, dstBuffer, encodedLen);

                fixed (byte* dst = dstBuffer)
                fixed (byte* src = srcBuffer)
                {
                    short xA;
                    short xB;

                    ScrambleBuffer(dst, encodedLen);
                    xA = (short)(7 * encodedLen);

                    // This is kind of a hack!
                    // The following relies on the decrypted file beginning with (UTF8::BOF)<?xml
                    // Therefore, this only works for XML encrypted using this method
                    xB = (short)((int)*(short*)(dst + 6) ^ 0x6C6D);

                    short* ptr = (short*)dst;
                    byte* dstEnd = &dst[encodedLen];
                    do
                    {
                        *ptr ^= xA;
                        ptr += 2;
                    } while (ptr < dstEnd);

                    ptr = (short*)(dst + 2);
                    do
                    {
                        *ptr ^= xB;
                        ptr += 2;
                    } while (ptr < dstEnd);

                    // This may need to be *((byte*)xB + 1) or something
                    // if the last byte ever ends up breaking
                    if ((encodedLen & 1) != 0)
                        *(dstEnd - 1) = (byte)(*(dstEnd - 1) ^ (byte)(xB & 0xFF));
                }
            }

            return dstBuffer;
        }

        private unsafe static void ScrambleBuffer(byte* dst, int len)
        {
            byte tmp;
            byte* ptr = dst;
            byte* end = &dst[len - 1];
            if (ptr < end)
            {
                do
                {
                    tmp = *end;
                    *end = *ptr;
                    *ptr = tmp;
                    ptr += 2;
                    end -= 2;
                } while (ptr < end);
            }
        }
    }
}
