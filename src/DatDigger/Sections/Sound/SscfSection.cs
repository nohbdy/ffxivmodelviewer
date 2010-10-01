using System;
using System.Collections.Generic;

namespace DatDigger.Sections.Sound
{
    public class SscfSection : SectionBase
    {
        private long startHeaderOffset;

        public SscfHeader SscfHeader { get; private set; }
        public List<int> VoiceOffsets { get; protected set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);
            this.LoadHeader(reader);
            this.LoadVoices(reader);
        }

        private void LoadHeader(BinaryReaderEx reader)
        {
            this.startHeaderOffset = reader.BaseStream.Position;

            this.SscfHeader = new SscfHeader();
            this.SscfHeader.Unknown0 = reader.ReadInt16();
            this.SscfHeader.NumTracks = reader.ReadInt16();
            this.SscfHeader.NumWaves = reader.ReadInt16();
            this.SscfHeader.Unknown1 = reader.ReadInt16();
            this.SscfHeader.OffsetA = reader.ReadInt32();
            this.SscfHeader.OffsetB = reader.ReadInt32();
            this.SscfHeader.OffsetC = reader.ReadInt64();
            this.SscfHeader.OffsetD = reader.ReadInt64();
            this.SscfHeader.OffsetE = reader.ReadInt64();
            this.SscfHeader.Unused = reader.ReadInt64();
        }

        private void LoadVoices(BinaryReaderEx reader)
        {
            if (this.SscfHeader.NumWaves <= 0)
            {
                return;
            }

            reader.BaseStream.Position = this.SscfHeader.OffsetB;
            this.VoiceOffsets = new List<int>(this.SscfHeader.NumWaves);
            this.Children = new List<INavigable>(this.SscfHeader.NumWaves);

            for (var i = 0; i < this.SscfHeader.NumWaves; i++)
            {
                this.VoiceOffsets.Add(reader.ReadInt32());
            }

            foreach (int offset in this.VoiceOffsets)
            {
                reader.BaseStream.Position = offset;
                WaveHeader waveHeader = ReadWaveHeader(reader);
                IWave voice = null;
                switch (waveHeader.Format)
                {
                    case WaveCompressionFormat.PCM:
                        voice = new PcmWave() { Parent = this, WaveHeader = waveHeader };
                        break;
                    case WaveCompressionFormat.Vorbis:
                        voice = new VorbisWave() { Parent = this, WaveHeader = waveHeader };
                        break;
                    case WaveCompressionFormat.ADPCM:
                        voice = new AdpcmWave() { Parent = this, WaveHeader = waveHeader };
                        break;
                    case WaveCompressionFormat.ATRAC3:
                    case WaveCompressionFormat.ATRAC3_Too:
                    case WaveCompressionFormat.XMA:
                        throw new InvalidOperationException("Unsupported Wave Format: " + waveHeader.Format);
                    default:
                        throw new InvalidOperationException("Unknown Wave Format: 0x" + ((int)waveHeader.Format).ToString("X"));
                }

                this.Children.Add(voice);
                voice.LoadData(reader);
            }
        }

        private WaveHeader ReadWaveHeader(BinaryReaderEx reader)
        {
            WaveHeader result = new WaveHeader();
            result.DataLength = reader.ReadInt32();
            result.NumChannels = reader.ReadInt32();
            result.SamplingRate = reader.ReadInt32();
            result.Format = (WaveCompressionFormat)reader.ReadInt32();
            result.NumSamples = reader.ReadInt32();
            result.F = reader.ReadInt32();
            result.FormatHeaderLength = reader.ReadInt32();
            result.H = reader.ReadInt32();

            return result;
        }
    }
}
