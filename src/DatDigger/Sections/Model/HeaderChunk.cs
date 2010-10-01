namespace DatDigger.Sections.Model
{
    public class HeaderChunk : Chunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "Header";
        }
    }
}
