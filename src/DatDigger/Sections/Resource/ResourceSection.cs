using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Resource
{
    public class ResourceSection : SectionBase
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ResourceHeader ResourceHeader { get; protected set; }

        [Browsable(false)]
        public List<ResourceData> Resources { get; protected set; }

        [Browsable(false)]
        public List<SectionType> ResourceTypes { get; protected set; }

        [Browsable(false)]
        public List<string> ResourceIds { get; protected set; }

        public List<string> StringTable { get; protected set; }

        /// <summary>Position in the stream where the resource info entries begin</summary>
        public long ResourceInfoStart { get; protected set; }

        /// <summary>Base position from which ResourceInfo.Offset and ResourceHeader.StringTableOffset are based</summary>
        public long BasePosition { get; protected set; }

        /// <summary>Name to display in file navigator</summary>
        public override string DisplayName { get { return "Resource Container"; } }

        /// <summary>List of subitems to display in file navigator</summary>
        public override List<INavigable> Children { get; protected set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.LoadHeader(reader);
            this.LoadResourceTypes(reader);
            this.LoadResourceIds(reader);
            this.LoadStringTable(reader);
            this.LoadResources(reader);
        }

        private void LoadHeader(BinaryReaderEx reader)
        {
            // Load Header
            this.ResourceHeader = new ResourceHeader();
            this.ResourceHeader.NumResources = reader.ReadInt32();
            this.ResourceHeader.StringTableOffset = reader.ReadInt32();
            this.ResourceHeader.NumStrings = reader.ReadInt32();
            this.ResourceHeader.ResourceType = (SectionType)reader.ReadInt32();

            this.LoadResourceInfo(reader);
        }

        private void LoadResourceTypes(BinaryReaderEx reader)
        {
            this.ResourceTypes = new List<SectionType>(this.ResourceHeader.NumResources);

            reader.BaseStream.Position = this.BasePosition + this.Resources[this.ResourceHeader.NumResources - 2].Offset;
            for (var i = 0; i < this.ResourceHeader.NumResources; i++)
            {
                this.ResourceTypes.Add((SectionType)reader.ReadInt32());
            }
        }

        private void LoadResourceIds(BinaryReaderEx reader)
        {
            this.ResourceIds = new List<string>(this.ResourceHeader.NumResources);

            var idTableOffset = this.BasePosition + this.Resources[this.ResourceHeader.NumResources - 1].Offset;
            for (var i = 0; i < this.ResourceHeader.NumResources; i++)
            {
                reader.BaseStream.Position = idTableOffset;
                this.ResourceIds.Add(reader.ReadNullTerminatedString());
                idTableOffset += 0x10;
            }
        }

        private void LoadResourceInfo(BinaryReaderEx reader)
        {
            this.ResourceInfoStart = reader.BaseStream.Position;

            // Load Resources
            this.Resources = new List<ResourceData>(this.ResourceHeader.NumResources);
            for (var i = 0; i < this.ResourceHeader.NumResources; i++)
            {
                ResourceData resource = new ResourceData();
                resource.Index = reader.ReadInt32();
                resource.Offset = reader.ReadInt32();
                resource.Size = reader.ReadInt32();
                resource.Type = reader.ReadInt32();
                this.Resources.Add(resource);
            }

            this.BasePosition = reader.BaseStream.Position;
        }

        private void LoadStringTable(BinaryReaderEx reader)
        {
            // Load String Table
            this.StringTable = new List<string>(this.ResourceHeader.NumStrings);
            reader.BaseStream.Position = this.BasePosition + this.ResourceHeader.StringTableOffset;

            for (var i = 0; i < this.ResourceHeader.NumStrings; i++)
            {
                this.StringTable.Add(reader.ReadNullTerminatedString());
            }
        }

        private void LoadResources(BinaryReaderEx reader)
        {
            long savedPosition = reader.BaseStream.Position;

            this.Children = new List<INavigable>(this.Resources.Count);
            for (var i = 0; i < this.Resources.Count; i++)
            {
                ResourceData resource = this.Resources[i];
                SectionType sectionType = this.ResourceTypes[i];
                if (resource.Type == 0 || resource.Size == 0 || (int)sectionType == 0)
                {
                    continue;
                }

                reader.BaseStream.Position = this.BasePosition + resource.Offset;
                resource.Section = SectionLoader.LoadSection(sectionType, reader, this, resource.Size);
                resource.Section.ResourceId = this.ResourceIds[i];
                resource.Section.ResourcePath = this.StringTable[i];
                this.Children.Add(resource.Section);
            }

            reader.BaseStream.Position = savedPosition;
        }
    }
}
