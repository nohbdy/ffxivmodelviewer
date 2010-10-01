using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Texture
{
    public struct MipMapData
    {
        public uint Offset;
        public int Length;
    }

    public class TextureSection : SectionBase
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TextureHeader TextureHeader { get; protected set; }
        public GtexData Gtex { get; protected set; }

        public TextureSection() : base() { }
        public TextureSection(string resourceName, GtexData gtex)
            : base()
        {
            this.ResourceId = resourceName;
            this.Gtex = gtex;
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.TextureHeader = new TextureHeader();

            this.TextureHeader.Unknown1 = reader.ReadInt32();
            this.TextureHeader.Unknown2 = reader.ReadInt32();
            this.TextureHeader.Unknown3 = reader.ReadInt32();
            this.TextureHeader.Unknown4 = reader.ReadInt32();

            // Load GTEX
            Gtex = new GtexData();
            Gtex.Parent = this;
            Gtex.LoadSection(reader);

            this.Children = new List<INavigable>();
            this.Children.Add(this.Gtex);
        }
    }
}
