namespace DatDigger.Sections.Model
{
    public class RstpChunk : Chunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "<RSTP>";
        }
    }
}
