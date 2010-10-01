using System;
using System.Collections.Generic;

namespace DatDigger.Sections.Shader
{
    public class PramChunk : Chunk
    {
        public short Unknown1 { get; protected set; } // 0x0818, 0x081B, 0x081A, 0x0809
	    public short Unknown2 { get; protected set; } // 0x00FF, 0x0005
	    public float Unknown3 { get; protected set; } // 0 (+0f), 0x3f000000 (0.5f)
	    public int NumParameters { get; protected set; } // number of parameters
	    public int Unknown4 { get; protected set; } // 0x20 - header size??
	    public int ParameterNameStart { get; protected set; } // Offset to start of list of offsets to parameter names
	    public int NumSamplers { get; protected set; } // number of samplers
	    public int SamplerDataStart { get; protected set; } // Offset to start of list of offsets to sampler data
        public int SamplerNameStart { get; protected set; } // Offset to start of list of offsets to sampler names

        public long BasePosition { get; protected set; }

        public List<uint> ParameterDataOffsets { get; protected set; }
        public List<uint> ParameterNameOffsets { get; protected set; }
        public List<uint> SamplerDataOffsets { get; protected set; }
        public List<uint> SamplerNameOffsets { get; protected set; }

        public List<ParameterData> Parameters { get; protected set; }
        public List<SamplerData> Samplers { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.BasePosition = reader.BaseStream.Position;

            this.DisplayName = String.Format("PRAM [0x{0:X}]", this.ChunkStart);
            this.Unknown1 = reader.ReadInt16();
            this.Unknown2 = reader.ReadInt16();
            this.Unknown3 = reader.ReadSingle();
            this.NumParameters = reader.ReadInt32();
            this.Unknown4 = reader.ReadInt32();
            this.ParameterNameStart = reader.ReadInt32();
            this.NumSamplers = reader.ReadInt32();
            this.SamplerDataStart = reader.ReadInt32();
            this.SamplerNameStart = reader.ReadInt32();

            this.ParameterDataOffsets = ReadOffsets(reader, this.NumParameters);
            this.ParameterNameOffsets = ReadOffsets(reader, this.NumParameters);
            this.SamplerDataOffsets = ReadOffsets(reader, this.NumSamplers);
            this.SamplerNameOffsets = ReadOffsets(reader, this.NumSamplers);

            this.Children = new List<INavigable>();
            this.ReadParameterData(reader);
            this.ReadSamplerData(reader);
        }

        private List<uint> ReadOffsets(BinaryReaderEx reader, int count)
        {
            List<uint> results = new List<uint>();
            for (var i = 0; i < count; i++)
            {
                results.Add(reader.ReadUInt32());
            }

            return results;
        }

        private void ReadParameterData(BinaryReaderEx reader)
        {
            this.Parameters = new List<ParameterData>(this.NumParameters);

            foreach (ushort offset in this.ParameterDataOffsets)
            {
                reader.BaseStream.Position = this.BasePosition + offset;
                ParameterData data = new ParameterData();
                data.Parent = this;
                data.IsPixelShaderParameter = (reader.ReadByte() == 0x01); // If the value is 1 it is used in pixel shaders, if 0 it is for vertex shader
                data.Unknown2 = reader.ReadByte();
                data.NumValues = reader.ReadByte();
                data.Unknown3 = reader.ReadByte();
                data.Unknown4 = reader.ReadInt32();
                data.Unknown5 = reader.ReadInt32();
                data.Unknown6 = reader.ReadInt32();
                data.Defaults.X = reader.ReadSingle();
                data.Defaults.Y = reader.ReadSingle();
                data.Defaults.Z = reader.ReadSingle();
                data.Defaults.W = reader.ReadSingle();

                this.Parameters.Add(data);
                this.Children.Add(data);
            }

            ReadParameterNames(reader);
        }

        private void ReadParameterNames(BinaryReaderEx reader)
        {
            for (var i = 0; i < this.NumParameters; i++)
            {
                reader.BaseStream.Position = this.BasePosition + this.ParameterNameOffsets[i];
                this.Parameters[i].Name = reader.ReadNullTerminatedString();
                this.Parameters[i].EffectHandle = new SlimDX.Direct3D9.EffectHandle(this.Parameters[i].Name);
            }
        }

        private void ReadSamplerData(BinaryReaderEx reader)
        {
            this.Samplers = new List<SamplerData>(this.NumSamplers);

            foreach (ushort offset in this.SamplerDataOffsets)
            {
                reader.BaseStream.Position = this.BasePosition + offset;
                SamplerData data = new SamplerData();
                data.Parent = this;
                data.Magic = reader.ReadInt16();
                data.StringTableLength = reader.ReadByte();
                data.Unknown1 = reader.ReadByte();
                data.Unknown2 = reader.ReadInt32();
                data.Unknown3 = reader.ReadInt32();
                data.Unknown4 = reader.ReadInt32();
                data.Unknown5 = reader.ReadInt32();
                data.Unknown6 = reader.ReadInt32();
                data.Unknown7 = reader.ReadInt32();
                data.Unknown8 = reader.ReadInt32();

                while (reader.PeekChar() != 0)
                {
                    data.StringTable.Add(reader.ReadNullTerminatedString());
                }

                this.Samplers.Add(data);
                this.Children.Add(data);
            }

            ReadSamplerNames(reader);
        }

        private void ReadSamplerNames(BinaryReaderEx reader)
        {
            for (var i = 0; i < this.NumSamplers; i++)
            {
                reader.BaseStream.Position = this.BasePosition + this.SamplerNameOffsets[i];
                this.Samplers[i].Name = reader.ReadNullTerminatedString();
            }
        }
    }
}
