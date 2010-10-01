namespace DatDigger.Sections
{
    public class CibSection : SectionBase
    {
        public int CibHeader { get; private set; }
        public override void LoadSection(BinaryReaderEx reader)
        {
            // Don't call the base LoadSection since cib sections do not have standard headers
            this.CibHeader = reader.ReadInt32();
        }
    }
}
