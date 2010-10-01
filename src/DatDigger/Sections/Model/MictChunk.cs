namespace DatDigger.Sections.Model
{
    public class MictChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "<MICT>";
        }
    }
}
