using System;

namespace DatDigger.Sections.Model
{
    public class WrbChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("WRB Chunk [0x{0:X}]", this.ChunkStart);
        }
    }
}
