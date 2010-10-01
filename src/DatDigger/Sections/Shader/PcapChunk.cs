using System;

namespace DatDigger.Sections.Shader
{
    public class PcapChunk : Chunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("PCAP [0x{0:X}]", this.ChunkStart);
        }
    }
}
