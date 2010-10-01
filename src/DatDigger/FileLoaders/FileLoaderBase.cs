using System.IO;

namespace DatDigger.FileLoaders
{
    public abstract class FileLoaderBase
    {
        public FileLoaderBase()
        {
            this.Name = this.GetType().Name;
        }

        public string Name { get; set; }

        public abstract bool ReadFile(BinaryReader reader);
    }
}
