using System;
using System.Text;

namespace DatDigger.Sections.Model
{
    public class StringChunk : Chunk
    {
        public String String { get; protected set; }

        public override void LoadData(BinaryReaderEx reader)
        {
            base.LoadData(reader);

            this.String = reader.ReadNullTerminatedString(Encoding.ASCII);
            this.DisplayName = "STR";
        }
    }
}
