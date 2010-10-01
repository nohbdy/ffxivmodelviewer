namespace DatDigger.Sections.Model
{
    public class BoundingBoxContainerChunk : ContainerChunk
    {
        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.DisplayName = "AABB";
        }
    }
}
