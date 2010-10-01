using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;

namespace DatDigger.Sections.UI
{
    public class SqexFile : INavigable
    {
        public string FileName { get; protected set; }
        public string EncryptionKey { get; protected set; }
        public string DisplayName { get { return "SQEX - " + FileName; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; protected set; }
        BlowfishEngine crypto = new BlowfishEngine();

        private byte[] encodedData;
        private byte[] decodedData;

        public byte[] EncodedData { get { return encodedData; } }
        public byte[] DecodedData { get { return decodedData; } }

        public string Contents { get; protected set; }

        public SqexFile(string fileName, string key)
        {
            this.FileName = fileName;
            this.EncryptionKey = key;
        }

        public void LoadFile(BinaryReaderEx reader)
        {
            reader.BaseStream.Seek(8, System.IO.SeekOrigin.Begin);
            
            this.ReadEncodedData(reader);
            this.DecodeData();

            this.Contents = Encoding.UTF8.GetString(this.decodedData);
        }

        public void ReadEncodedData(BinaryReaderEx reader)
        {
            long encodedDataLen = reader.BaseStream.Length - 8;
            this.encodedData = new byte[encodedDataLen];
            this.decodedData = new byte[encodedDataLen];
            reader.Read(encodedData, 0, (int)encodedDataLen);
        }

        public void DecodeData()
        {
            byte[] key = Encoding.UTF8.GetBytes(this.EncryptionKey);
            crypto.Init(false, key);

            int dataLength = this.encodedData.Length;
            crypto.ProcessBuffer(this.encodedData, 0, this.decodedData, 0, dataLength);
        }
    }
}
