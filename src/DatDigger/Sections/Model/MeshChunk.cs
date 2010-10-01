using System;

namespace DatDigger.Sections.Model
{
    public class MeshChunk : ContainerChunk
    {
        public MeshHeaderChunk Header { get; private set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = String.Format("MESH [0x{0:X}]", this.ChunkStart);
        }

        protected override void LoadChildren(BinaryReaderEx reader)
        {
            base.LoadChildren(reader);

            this.Header = this.FindChild<MeshHeaderChunk>();
        }
    }
}
