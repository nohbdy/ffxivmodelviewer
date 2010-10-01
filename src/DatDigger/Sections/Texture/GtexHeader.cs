namespace DatDigger.Sections.Texture
{
    public class GtexHeader
    {
        public int Magic { get; set; } // = 'GTEX'
		public byte Unknown1 { get; set; }
		public byte Unknown2 { get; set; }
		public byte Format { get; set; }
		public byte MipMapCount { get; set; }
		public byte Unknown3 { get; set; }
        public bool IsCubeMap { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public short Depth { get; set; }
		public int Unknown5 { get; set; }
        public uint DataOffset { get; set; }
    }
}
