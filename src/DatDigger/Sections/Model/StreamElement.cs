namespace DatDigger.Sections.Model
{
    public class StreamElement
    {
        public short Unknown1 { get; set; }
        public short OffsetInVertex { get; set; }
        public int UncompressedOffsetInVertex { get; set; }
        public StreamCompressionType CompressionType { get; set; }
        public int NumElements { get; set; }
        public StreamDataType DataType { get; set; }
        public short Unknown2 { get; set; }

        public SlimDX.Direct3D9.VertexElement GenerateVertexElement()
        {
            return new SlimDX.Direct3D9.VertexElement(0, (short)this.UncompressedOffsetInVertex, DataType.DeclarationType, SlimDX.Direct3D9.DeclarationMethod.Default, DataType.DeclarationUsage, DataType.UsageIndex);
        }
    }
}
