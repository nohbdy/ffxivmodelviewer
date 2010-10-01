using System;

namespace DatDigger
{
    public static class Utilities
    {
        public static string DataFileIdToPath(int fileNumber)
        {
            byte b0 = (byte)(fileNumber & 0xFF);            // LSB
            byte b1 = (byte)((fileNumber >> 8) & 0xFF);
            byte b2 = (byte)((fileNumber >> 16) & 0xFF);
            byte b3 = (byte)((fileNumber >> 24) & 0xFF);    // MSB

            return String.Format("./data/{0:X2}/{1:X2}/{2:X2}/{3:X2}.DAT", b3, b2, b1, b0);
        }

        public static string DataFileIdToRelativePath(int fileNumber)
        {
            byte b0 = (byte)(fileNumber & 0xFF);            // LSB
            byte b1 = (byte)((fileNumber >> 8) & 0xFF);
            byte b2 = (byte)((fileNumber >> 16) & 0xFF);
            byte b3 = (byte)((fileNumber >> 24) & 0xFF);    // MSB

            return String.Format("data/{0:X2}/{1:X2}/{2:X2}/{3:X2}.DAT", b3, b2, b1, b0);
        }
    }
}
