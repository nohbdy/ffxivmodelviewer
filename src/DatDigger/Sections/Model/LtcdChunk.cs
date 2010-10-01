namespace DatDigger.Sections.Model
{
    public class LtcdChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "<LTCD>";
        }
    }
}
