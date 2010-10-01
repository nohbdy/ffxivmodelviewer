using System;

namespace DatDigger.Sections.Model
{
    public class ModelContainerChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("MDLC Chunk [0x{0:X}]", this.ChunkStart);
        }
    }
}
