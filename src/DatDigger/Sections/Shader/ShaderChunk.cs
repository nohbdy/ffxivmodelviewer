using System;

namespace DatDigger.Sections.Shader
{
    public class ShaderChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("Shader [0x{0:X}]", this.ChunkStart);
        }
    }
}
