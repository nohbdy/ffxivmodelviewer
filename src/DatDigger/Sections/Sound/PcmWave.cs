using System.Collections.Generic;

namespace DatDigger.Sections.Sound
{
    public class PcmWave : IWave, DatDigger.Sound.IPlayable
    {
        public string DataFormat { get { return "PCM"; } }
        public string DisplayName { get { return "PCM Sound"; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; protected set; }
        public WaveHeader WaveHeader { get; set; }
        public SlimDX.Multimedia.WaveFormat WaveFormat { get; protected set; }

        [System.ComponentModel.Browsable(false)]
        public byte[] WaveData { get; protected set; }

        public void LoadData(BinaryReaderEx reader)
        {
            long pos = reader.BaseStream.Position;

            BuildFormat();

            reader.BaseStream.Position = pos + this.WaveHeader.FormatHeaderLength;
            this.WaveData = reader.ReadBytes(this.WaveHeader.DataLength);
        }

        private void BuildFormat()
        {
            this.WaveFormat = new SlimDX.Multimedia.WaveFormat();
            this.WaveFormat.FormatTag = SlimDX.Multimedia.WaveFormatTag.Pcm;
            this.WaveFormat.SamplesPerSecond = this.WaveHeader.SamplingRate;
            this.WaveFormat.Channels = (short)this.WaveHeader.NumChannels;
            this.WaveFormat.BitsPerSample = 16;
            this.WaveFormat.BlockAlignment = (short)(this.WaveFormat.Channels * 2);
            this.WaveFormat.AverageBytesPerSecond = this.WaveFormat.SamplesPerSecond * this.WaveFormat.BlockAlignment;
        }
    }
}
