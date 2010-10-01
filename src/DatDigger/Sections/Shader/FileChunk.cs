using System;

namespace DatDigger.Sections.Shader
{
    public class FileChunk : Chunk
    {
        public int BlockSize { get; protected set; }
        public int StringOffset { get; protected set; }
        public int ShaderOffset { get; protected set; }

        public byte[] UnknownData { get; protected set; }

        public string Name { get; protected set; }

        public int CompiledShaderLength { get; protected set; }

        public int Unknown4 { get; protected set; } // Varies, could be important for something?

        public byte[] CompiledShader { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            long pos = reader.BaseStream.Position;

            this.BlockSize = reader.ReadInt32(Endianness.BigEndian);
            this.StringOffset = reader.ReadInt32(Endianness.BigEndian);
            this.ShaderOffset = reader.ReadInt32(Endianness.BigEndian);
            this.Name = reader.ReadNullTerminatedString();

            reader.BaseStream.Position = pos + this.ShaderOffset;

            this.UnknownData = reader.ReadBytes(16);
            this.CompiledShaderLength = reader.ReadInt32(Endianness.BigEndian);
            this.Unknown4 = reader.ReadInt32(Endianness.BigEndian);

            reader.BaseStream.Position = pos + this.ShaderOffset + 0x20;

            this.CompiledShader = reader.ReadBytes(this.CompiledShaderLength);

            this.DisplayName = String.Format("File [0x{0:X}]", this.ChunkStart);
        }
    }
}
