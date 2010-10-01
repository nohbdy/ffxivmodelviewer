using System.Collections.Generic;

namespace DatDigger.Sections.Shader
{
    public class ShaderSection : SectionBase
    {
        private Chunk baseChunk;

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.baseChunk = ChunkLoader.LoadChunk(SectionType.sdrb, reader, null);
            this.Children = new List<INavigable>();
            this.Children.Add(baseChunk);
        }
    }
}
