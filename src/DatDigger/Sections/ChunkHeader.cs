namespace DatDigger.Sections
{
    public class ChunkHeader
    {
        /// <summary>The Type of chunk this header preceeds</summary>
        public int ChunkType { get; set; }

        public int Unknown1 { get; set; }

        /// <summary>Size in bytes (including header) of this chunk</summary>
        public int DataSize { get; set; }

        /// <summary>Offset from beginning of this chunk to beginning of next chunk, or 0 if this is the last chunk</summary>
        public int NextChunkOffset { get; set; }
    }
}
