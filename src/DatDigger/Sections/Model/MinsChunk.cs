namespace DatDigger.Sections.Model
{
    public class MinsChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "<MINS>";
        }
    }
}
