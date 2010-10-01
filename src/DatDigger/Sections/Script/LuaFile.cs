using System.Collections.Generic;
using System.Text;

namespace DatDigger.Sections.Script
{
    public class LuaFile : INavigable
    {
        public string DataAsString { get; protected set; }
        public byte[] Data { get; protected set; }
        public string DisplayName { get; protected set; }
        public INavigable Parent { get; set; }
        public List<INavigable> Children { get; protected set; }

        public int Unknown { get; protected set; }
        public int ContentLength { get; protected set; }
        public bool IsEncoded { get; protected set; }

        public LuaFile()
        {
            this.DisplayName = "Lua File";
        }

        public void LoadFile(BinaryReaderEx reader)
        {
            reader.BaseStream.Position = 4;
            this.Unknown = reader.ReadInt32();
            this.ContentLength = reader.ReadInt32();
            this.IsEncoded = (reader.ReadByte() == 0xFF);

            byte[] data = new byte[this.ContentLength];

            reader.Read(data, 0, this.ContentLength);

            if (this.IsEncoded)
            {
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] ^= 0x73;
                }
            }

            this.Data = data;
            this.DataAsString = Encoding.UTF8.GetString(data);
        }
    }
}
