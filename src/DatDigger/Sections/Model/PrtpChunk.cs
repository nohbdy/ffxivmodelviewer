namespace DatDigger.Sections.Model
{
    public class PrtpChunk : Chunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "<PRTP>";
        }
    }
}
