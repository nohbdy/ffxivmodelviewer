using System;

namespace DatDigger.Sections.Model
{
    public class ModelChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("Model Chunk [0x{0:X}]", this.ChunkStart);
        }
    }
}
