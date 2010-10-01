namespace DatDigger.Sections.Sound
{
    public class SscfHeader
    {
        public short Unknown0 { get; set; } // 0x1
        public short NumTracks { get; set; }
        public short NumWaves { get; set; }
        public short Unknown1 { get; set; } // 0x100F, 0x1389, 0x138A  -- possible flags, format?
        public int OffsetA { get; set; }
        public int OffsetB { get; set; } // offset to list of wave offsets
        public long OffsetC { get; set; }
        public long OffsetD { get; set; }
        public long OffsetE { get; set; }
        public long Unused { get; set; } // 0x0
    }
}
