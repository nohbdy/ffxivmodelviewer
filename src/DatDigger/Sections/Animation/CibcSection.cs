using System.Collections.Generic;

namespace DatDigger.Sections.Animation
{
    public class CibcSection : CibSection
    {
        private const int bytesPerBlock = 0x10;

        public List<string> Strings { get; set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.Strings = new List<string>();
            int numStrings = (this.SectionLength - 4) / bytesPerBlock;
            for (var i = 0; i < numStrings; i++)
            {
                this.Strings.Add(reader.ReadFixedLengthString(16).Trim());
            }
        }
    }
}
