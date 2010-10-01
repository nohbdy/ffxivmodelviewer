using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatDigger.Sections.Script
{
    public static class NameDecoder
    {
        private static Dictionary<char, char> lut = new Dictionary<char,char>(26) {
            { '9', 'a' },
            { '8', 'b' },
            { '7', 'c' },
            { '6', 'd' },
            { '5', 'e' },
            { '4', 'f' },
            { '3', 'g' },
            { '2', 'h' },
            { '1', 'i' },
            { '0', 'j' },
            { 'z', 'k' },
            { 'y', 'l' },
            { 'x', 'm' },
            { 'w', 'n' },
            { 'v', 'o' },
            { 'u', 'p' },
            { 't', 'q' },
            { 's', 'r' },
            { 'r', 's' },
            { 'q', 't' },
            { 'p', 'u' },
            { 'o', 'v' },
            { 'n', 'w' },
            { 'm', 'x' },
            { 'l', 'y' },
            { 'k', 'z' },
            { 'j', '0' },
            { 'i', '1' },
            { 'h', '2' },
            { 'g', '3' },
            { 'f', '4' },
            { 'e', '5' },
            { 'd', '6' },
            { 'c', '7' },
            { 'b', '8' },
            { 'a', '9' },
            { '_', '_' }
        };

        public static string Decode(string fileName)
        {
            int dot = fileName.IndexOf('.');
            string ext = (dot < 0) ? String.Empty : fileName.Substring(dot);
            char[] src = (dot < 0) ? fileName.ToCharArray() : fileName.Substring(0, dot).ToCharArray();
            return src.Aggregate(new StringBuilder(fileName.Length), (a, b) => a.Append(lut[b]), a => a.Append(ext).ToString());
        }
    }
}
