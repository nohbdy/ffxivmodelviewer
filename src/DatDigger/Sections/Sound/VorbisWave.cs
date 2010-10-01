using System.Collections.Generic;

namespace DatDigger.Sections.Sound
{
    /// <summary>Voice containing Ogg Vorbis data</summary>
    public class VorbisWave : IWave, DatDigger.Sound.IPlayableStream
    {
        public string DataFormat { get { return "Vorbis"; } }
        public string DisplayName { get { return "Vorbis Sound"; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; protected set; }
        public WaveHeader WaveHeader { get; set; }
        public SlimDX.Multimedia.WaveFormat WaveFormat { get; protected set; }

        [System.ComponentModel.Browsable(false)]
        public byte[] WaveData { get { return null; } }
        public int NumChannels { get; protected set; }
        public int NumStreams { get; protected set; }
        public float TimeTotal { get; protected set; }
        public long PcmSamples { get; protected set; }
        public int StreamLength { get; protected set; }
        public int SamplingRate { get; protected set; }
        public int BitRate { get; protected set; }

        private System.IO.MemoryStream oggDataStream;
        private csvorbis.VorbisFile vorbisFile;
        private object mutex = new object();

        public void LoadData(BinaryReaderEx reader)
        {
            reader.BaseStream.Seek(this.WaveHeader.FormatHeaderLength, System.IO.SeekOrigin.Current);
            DecodeWave(reader);
        }

        public void DecodeWave(BinaryReaderEx reader)
        {
            byte[] oggData = new byte[this.WaveHeader.DataLength];
            reader.Read(oggData, 0, this.WaveHeader.DataLength);

            oggDataStream = new System.IO.MemoryStream(oggData, false);
            
            vorbisFile = new csvorbis.VorbisFile(oggDataStream, null, 0);
            csvorbis.Info vorbisInfo = vorbisFile.getInfo(0);

            this.WaveFormat = new SlimDX.Multimedia.WaveFormat();
            this.WaveFormat.FormatTag = SlimDX.Multimedia.WaveFormatTag.Pcm;
            this.WaveFormat.Channels = (short)vorbisInfo.channels;
            this.WaveFormat.BitsPerSample = 16;
            this.WaveFormat.SamplesPerSecond = vorbisInfo.rate;
            this.WaveFormat.BlockAlignment = (short)(this.WaveFormat.Channels * 2); // 16 bits * number of channels
            this.WaveFormat.AverageBytesPerSecond = this.WaveFormat.SamplesPerSecond * this.WaveFormat.BlockAlignment;

            this.NumChannels = this.WaveFormat.Channels;
            this.SamplingRate = vorbisInfo.rate;
            this.NumStreams = vorbisFile.streams();
            this.TimeTotal = vorbisFile.time_total(0);
            this.PcmSamples = vorbisFile.pcm_total(0);
            this.BitRate = vorbisFile.bitrate(0);
            this.StreamLength = (int)(this.PcmSamples * this.WaveFormat.BlockAlignment); // numSamples * numChannels * bytes per sample
        }

        public int GetWaveData(byte[] buffer, int length)
        {
            if (oggDataStream == null) { return -1; }

            lock (mutex)
            {
                if (oggDataStream == null) { return -1; }
                return vorbisFile.read(buffer, length, 0, 0, 2, 1, null);
            }
        }

        public void Dispose()
        {
            if (oggDataStream != null)
            {
                oggDataStream.Dispose();
                oggDataStream = null;
            }
        }
    }
}
