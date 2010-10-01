using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Texture
{
    public class GtexData : INavigable
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GtexHeader Header { get; protected set; }

        public string DisplayName { get { return "GTEX"; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; protected set; }

        /// <summary>Should probably rename this as it can also hold layer information for cubemaps</summary>
        public List<MipMapData> MipMapData { get; protected set; }

        public uint DataOffset { get; protected set; }
        public List<byte[]> TextureData { get; protected set; }
        public int NumLayers { get; protected set; }

        public SlimDX.Direct3D9.Format Format
        {
            get
            {
                switch (this.Header.Format)
                {
                    case 0x03: return SlimDX.Direct3D9.Format.A8R8G8B8;
                    case 0x04: return SlimDX.Direct3D9.Format.X8R8G8B8;
                    case 0x18: return SlimDX.Direct3D9.Format.Dxt1;
                    case 0x19: return SlimDX.Direct3D9.Format.Dxt3;
                    case 0x1A: return SlimDX.Direct3D9.Format.Dxt5;
                }
                throw new InvalidOperationException("Unknown Texture Format: " + this.Header.Format);
            }
        }

        public void LoadSection(BinaryReaderEx reader)
        {
            this.Header = new GtexHeader();
            this.Header.Magic = reader.ReadInt32();
            this.Header.Unknown1 = reader.ReadByte();
            this.Header.Unknown2 = reader.ReadByte();
            this.Header.Format = reader.ReadByte();
            this.Header.MipMapCount = reader.ReadByte();
            this.Header.Unknown3 = reader.ReadByte();
            this.Header.IsCubeMap = reader.ReadByte() == 0x1;
            this.Header.Width = reader.ReadUInt16(Endianness.BigEndian);
            this.Header.Height = reader.ReadUInt16(Endianness.BigEndian);
            this.Header.Depth = reader.ReadInt16(Endianness.BigEndian);
            this.Header.Unknown5 = reader.ReadInt32(Endianness.BigEndian);
            this.Header.DataOffset = reader.ReadUInt32(Endianness.BigEndian);

            if (this.Header.IsCubeMap) { this.NumLayers = 6; }
            else { this.NumLayers = this.Header.MipMapCount; }

            this.LoadMipMapData(reader);
            this.GetDataOffset();
            this.LoadTextureData(reader);
        }

        public void LoadMipMapData(BinaryReaderEx reader)
        {
            this.MipMapData = new List<MipMapData>(this.Header.MipMapCount);

            for (var i = 0; i < this.NumLayers; i++)
            {
                MipMapData mipData = new MipMapData();
                mipData.Offset = reader.ReadUInt32(Endianness.BigEndian);
                mipData.Length = reader.ReadInt32(Endianness.BigEndian);

                this.MipMapData.Add(mipData);
            }
        }

        public void LoadTextureData(BinaryReaderEx reader)
        {
            this.TextureData = new List<byte[]>(this.Header.MipMapCount);

            foreach (MipMapData mipData in this.MipMapData)
            {
                reader.BaseStream.Position = this.DataOffset + mipData.Offset;
                this.TextureData.Add(reader.ReadBytes(mipData.Length));
            }
        }

        public void GetDataOffset()
        {
            if (this.Header.DataOffset != 0)
            {
                this.DataOffset = this.Header.DataOffset;
                return;
            }

            INavigable parent = this.Parent;
            while (parent != null)
            {
                var pwib = parent as PwibSection;
                if (pwib != null)
                {
                    this.DataOffset = pwib.DataOffset;
                    return;
                }

                parent = parent.Parent;
            }

            // No PWIB parent and not set in GTEX header?? Gotta calculate the offset ourself :(
            const uint headerSize = 24;
            uint offset = headerSize + (uint)this.NumLayers * 8;

            // Round to next highest multiple of 16
            if (offset % 16 == 0)
            {
                this.DataOffset = offset;
            }
            else
            {
                this.DataOffset = ((offset >> 5) + 1) << 5;
            }
        }
    }
}
