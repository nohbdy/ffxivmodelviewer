using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections
{
    public class Chunk : INavigable
    {
        public Chunk()
        {
            this.DisplayName = "??Chunk??";
        }

        public SectionType SectionType { get; set; }
        public ChunkHeader ChunkHeader { get; protected set; }

        /// <summary>Offset in the stream where this chunk begins</summary>
        public long ChunkStart { get; protected set; }

        [Browsable(false)]
        public virtual string DisplayName { get; protected set; }

        [Browsable(false)]
        public INavigable Parent { get; internal set; }

        [Browsable(false)]
        public virtual List<INavigable> Children { get; protected set; }

        public virtual void LoadData(BinaryReaderEx reader)
        {
            this.ChunkStart = reader.BaseStream.Position;

            this.ChunkHeader = new ChunkHeader();
            this.ChunkHeader.ChunkType = reader.ReadInt32();
            this.ChunkHeader.Unknown1 = reader.ReadInt32(Endianness.BigEndian);
            this.ChunkHeader.DataSize = reader.ReadInt32(Endianness.BigEndian);
            this.ChunkHeader.NextChunkOffset = reader.ReadInt32(Endianness.BigEndian);
        }
    }
}
