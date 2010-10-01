using System;
using System.Collections.Generic;

namespace DatDigger.Sections.Sound
{
    /// <summary>Voice encoded as ADPCM</summary>
    public class AdpcmWave : IWave, DatDigger.Sound.IPlayable
    {
        public string DataFormat { get { return "ADPCM"; } }
        public string DisplayName { get { return "ADPCM Sound"; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; protected set; }
        public WaveHeader WaveHeader { get; set; }
        public DatDigger.Sound.AdpcmWaveFormat Header { get; protected set; }
        SlimDX.Multimedia.WaveFormat DatDigger.Sound.IPlayable.WaveFormat { get { return Header; } }

        [System.ComponentModel.Browsable(false)]
        public byte[] WaveData { get; protected set; }

        public void LoadData(BinaryReaderEx reader)
        {
            long pos = reader.BaseStream.Position;

            ReadHeader(reader);

            reader.BaseStream.Position = pos + this.WaveHeader.FormatHeaderLength;
            this.WaveData = reader.ReadBytes(this.WaveHeader.DataLength);
        }

        private void ReadHeader(BinaryReaderEx reader)
        {
            this.Header = new DatDigger.Sound.AdpcmWaveFormat();
            this.Header.FormatTag = (SlimDX.Multimedia.WaveFormatTag)reader.ReadInt16();
            this.Header.Channels = reader.ReadInt16();
            this.Header.SamplesPerSecond = reader.ReadInt32();
            this.Header.AverageBytesPerSecond = reader.ReadInt32();
            this.Header.BlockAlignment = reader.ReadInt16();
            this.Header.BitsPerSample = reader.ReadInt16();
            short cbSize = reader.ReadInt16();
            if (cbSize != 0x20)
            {
                throw new InvalidOperationException(String.Format("Unexpected value for ADPCMWAVEFORMAT cbSize 0x{0}.  Expected 0x20", cbSize));
            }

            this.Header.SamplesPerBlock = reader.ReadInt16();
            this.Header.NumCoef = reader.ReadInt16();
            this.Header.Coefficients = new List<DatDigger.Sound.AdpcmCoefficient>(this.Header.NumCoef);
            for (var i = 0; i < this.Header.NumCoef; i++)
            {
                DatDigger.Sound.AdpcmCoefficient coef = new DatDigger.Sound.AdpcmCoefficient();
                coef.A = reader.ReadInt16();
                coef.B = reader.ReadInt16();
                this.Header.Coefficients.Add(coef);
            }
        }
    }
}
