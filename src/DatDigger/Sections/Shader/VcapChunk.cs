using System;

namespace DatDigger.Sections.Shader
{
    public class VcapChunk : Chunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("VCAP [0x{0:X}]", this.ChunkStart);
        }
    }
}
