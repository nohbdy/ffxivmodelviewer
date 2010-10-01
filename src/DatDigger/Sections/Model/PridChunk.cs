namespace DatDigger.Sections.Model
{
    public class PridChunk : Chunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "<PRID>";
        }
    }
}
