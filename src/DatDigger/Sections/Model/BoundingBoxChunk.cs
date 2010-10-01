namespace DatDigger.Sections.Model
{
    public class BoundingBoxChunk : Chunk
    {
        private SlimDX.BoundingBox boundingBox;
        public SlimDX.BoundingBox BoundingBox { get { return boundingBox; } }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.boundingBox.Minimum.X = reader.ReadSingle(Endianness.BigEndian);
            this.boundingBox.Minimum.Y = reader.ReadSingle(Endianness.BigEndian);
            this.boundingBox.Minimum.Z = reader.ReadSingle(Endianness.BigEndian);
            this.boundingBox.Maximum.X = reader.ReadSingle(Endianness.BigEndian);
            this.boundingBox.Maximum.Y = reader.ReadSingle(Endianness.BigEndian);
            this.boundingBox.Maximum.Z = reader.ReadSingle(Endianness.BigEndian);

            this.DisplayName = "AABB";
        }
    }
}
