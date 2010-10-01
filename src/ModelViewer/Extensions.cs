using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelViewer
{
    public static class Extensions
    {
        public static IntPtr Increment(this IntPtr ptr, int cbSize)
        {
            return new IntPtr(ptr.ToInt64() + cbSize);
        }

        public static bool DirectoryContains(this string path, string file)
        {
            var files = System.IO.Directory.GetFiles(path);
            var filePath = System.IO.Path.Combine(path, file);
            return files.Contains(filePath);
        }

        public static bool IsNullOrWhiteSpace(this System.String str)
        {
            if (str == null)
            {
                return true;
            }

            return (str.Trim() == String.Empty);
        }

        public static void CopyTo(this System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }

        public static string GetResourceAsString(this System.Reflection.Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error retrieving from Resources. Tried '" + resourceName + "'\r\n" + e.ToString());
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T thing in enumerable)
            {
                action(thing);
            }
        }
    }
}