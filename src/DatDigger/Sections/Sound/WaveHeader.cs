namespace DatDigger.Sections.Sound
{
    public class WaveHeader
    {
        public int DataLength { get; set; } // Does not include VoiceHeader or FormatHeader
        public int NumChannels { get; set; }
        public int SamplingRate { get; set; }
        public WaveCompressionFormat Format { get; set; }
        public int NumSamples { get; set; }
        public int F { get; set; }
        public int FormatHeaderLength { get; set; }
        public int H { get; set; }
    }
}
