using System.Text;

namespace DatDigger.Sections.Model
{
    public class NameChunk : Chunk
    {
        public string String { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.String = reader.ReadNullTerminatedString(Encoding.ASCII);
            this.DisplayName = "NAME";
        }
    }
}
