using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections
{
    public abstract class SectionBase : INavigable
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SectionHeader SectionHeader { get; protected set; }

        public SectionType SectionType { get; internal set; }

        [Browsable(false)]
        public virtual string DisplayName { get { return String.Format("{0} Section", this.SectionType); } }

        [Browsable(false)]
        public INavigable Parent { get; internal set; }

        [Browsable(false)]
        public virtual List<INavigable> Children { get; protected set; }

        public string ResourceId { get; internal set; }

        public string ResourcePath { get; internal set; }

        /// <summary>Position within the stream of the beginning of this section</summary>
        public long SectionStart { get; protected set; }

        /// <summary>Length in bytes of the section</summary>
        public int SectionLength { get; internal set; }

        public virtual void LoadSection(BinaryReaderEx reader)
        {
            this.SectionStart = reader.BaseStream.Position;

            this.SectionHeader = new SectionHeader();
            this.SectionHeader.Magic = reader.ReadInt64();
            this.SectionHeader.Version = reader.ReadInt32();
            this.SectionHeader.Unknown2 = reader.ReadInt32();
            this.SectionHeader.SectionLength = reader.ReadInt32();
            this.SectionHeader.Junk = reader.ReadBytes(28);
        }
    }
}
