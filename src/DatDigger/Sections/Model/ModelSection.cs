using System.Collections.Generic;

namespace DatDigger.Sections.Model
{
    public class ModelSection : SectionBase
    {
        private Chunk baseChunk;

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.baseChunk = ChunkLoader.LoadChunk(SectionType.wrb, reader, this);
            this.Children = new List<INavigable>();
            this.Children.Add(baseChunk);
        }
    }
}
