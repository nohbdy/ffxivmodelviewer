using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Animation
{
    public class AnimatedBoneNode : INavigable
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AnimatedBone Bone { get; internal set; }

        public string DisplayName { get { return "Bone " + this.Bone.BoneId; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; internal set; }
    }

    public abstract class MtbSubSection : INavigable
    {
        public int Index { get; protected set; }
        public string SectionName { get; internal set; }

        public MtbSubSection() { this.Children = new List<INavigable>(); }

        public virtual void Load(BinaryReaderEx reader)
        {
            this.Index = reader.ReadInt32();
            reader.BaseStream.Seek(12, System.IO.SeekOrigin.Current);
        }

        public string DisplayName { get { return SectionName; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; private set; }
    }

    public class SpuSectionData : INavigable
    {
        public int Index { get; private set; }
        public int Offset { get; private set; }
        public int NumChunks { get; private set; }
        public string Name { get; private set; }

        public string DisplayName { get { return this.Name; } }
        public INavigable Parent { get; internal set; }
        public List<SpuChunkData> Chunks { get; private set; }
        public List<INavigable> Children { get; private set; }

        public SpuSectionData()
        {
            this.Children = new List<INavigable>();
            this.Chunks = new List<SpuChunkData>();
        }

        public void Load(BinaryReaderEx reader, int index)
        {
            var mtbSection = this.GetParent<MtbSection>();
            this.Name = mtbSection.ChunkNames[index];

            Index = index;
            Offset = reader.ReadInt32(Endianness.BigEndian);
            NumChunks = reader.ReadInt32(Endianness.BigEndian);
        }

        public void LoadChunks(BinaryReaderEx reader)
        {
            for (var i = 0; i < NumChunks; i++)
            {
                SpuChunkData chunk = new SpuChunkData();
                chunk.Load(reader, i);
                chunk.Parent = this;
                this.Chunks.Add(chunk);
            }
        }
    }

    public class SpuChunkData : INavigable
    {
        public int Index { get; private set; }
        public int Offset { get; private set; }
        public short Length { get; private set; }
        public byte NumChildren { get; private set; }
        public byte Flag { get; private set; }

        public SpuChunkData() { this.Children = new List<INavigable>(); }

        public void Load(BinaryReaderEx reader, int index)
        {
            this.Index = index;

            Offset = reader.ReadInt32(Endianness.BigEndian);
            Length = reader.ReadInt16(Endianness.BigEndian);
            NumChildren = reader.ReadByte();
            NumChildren++;
            Flag = reader.ReadByte();
        }

        public string DisplayName { get { return "Chunk " + Index.ToString(); }}
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; private set; }
    }

    public class MtbGenericSection : MtbSubSection
    {

    }

    public class SpuBinary : MtbSubSection
    {
        public long SpuBasePosition { get; protected set; }
        public int SectionLength { get; protected set; }
        public short NumSections { get; protected set; }
        public short NumBones { get; set; }
        public List<SpuSectionData> Sections { get; private set; }

        public override void Load(BinaryReaderEx reader)
        {
            base.Load(reader);

            SectionLength = reader.ReadInt32(Endianness.BigEndian);
            reader.BaseStream.Seek(12, System.IO.SeekOrigin.Current);

            SpuBasePosition = reader.BaseStream.Position;

            NumSections = reader.ReadInt16();
            NumBones = reader.ReadInt16();

            reader.BaseStream.Seek(12, System.IO.SeekOrigin.Current);

            this.Sections = new List<SpuSectionData>();
            for (var i = 0; i < NumSections; i++)
            {
                SpuSectionData sectionData = new SpuSectionData();
                sectionData.Parent = this;
                this.Sections.Add(sectionData);
                this.Children.Add(sectionData);
                sectionData.Load(reader, i);
            }

            for (var i = 0; i < NumSections; i++)
            {
                var section = this.Sections[i];
                reader.BaseStream.Position = SpuBasePosition + section.Offset;
                section.LoadChunks(reader);
            }

            SpuCurveLoader.LoadCurves(reader, this);
        }
    }

    public class MtbHeader : MtbSubSection
    {
        public struct ChunkIndex
        {
            public int Index;
            public bool Flag;

            private string toStringCache;
            public override string ToString()
            {
                if (toStringCache == null)
                {
                    toStringCache = Index.ToString() + " [Flag: " + Flag.ToString() + "]";
                }
                return toStringCache;
            }
        }

        public MtbHeader() { Offsets = new List<ChunkIndex>(); }

        public float Float1 { get; private set; }
        public float AnimationLength { get; private set; }
        public short Bones { get; private set; }
        public short NumIndices { get; private set; }
        public byte Flags { get; private set; }

        public List<ChunkIndex> Offsets { get; private set; }

        public override void Load(BinaryReaderEx reader)
        {
            base.Load(reader);

            Float1 = reader.ReadSingle();
            AnimationLength = reader.ReadSingle();
            Bones = reader.ReadInt16();
            NumIndices = reader.ReadInt16();
            Flags = reader.ReadByte();
            reader.BaseStream.Seek(3, System.IO.SeekOrigin.Current);
            for (var i = 0; i < NumIndices; i++)
            {
                int val = reader.ReadInt32();
                ChunkIndex idx = new ChunkIndex();
                idx.Index = val & 0x7FFFFFFF;
                idx.Flag = (val & 0x80000000) == 0x80000000;
                this.Offsets.Add(idx);
            }
        }
    }

    public class MtbSection : SectionBase
    {
        public List<MtbSubSection> Sections { get; protected set; }
        public List<string> SectionNames { get; protected set; }
        public List<string> ChunkNames { get; protected set; }
        public int NumSections { get; protected set; }
        public int NumSectionNames { get; protected set; }
        public int NumChunkNames { get; protected set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MtbHeader Header { get; protected set; }

        private int[] sectionOffsets;
        private int[] sectionNameOffsets;
        private int[] chunkNameOffsets;

        public override string DisplayName { get { return String.Format("MTB [{0}]", this.ResourceId); } }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            reader.BaseStream.Position += 16;
            this.NumSections = reader.ReadInt32();
            sectionOffsets = new int[this.NumSections];
            for (var i = 0; i < this.NumSections; i++)
            {
                sectionOffsets[i] = reader.ReadInt32();
            }

            this.NumSectionNames = reader.ReadInt32();
            this.sectionNameOffsets = new int[this.NumSectionNames];
            for (var i = 0; i < this.NumSectionNames; i++)
            {
                sectionNameOffsets[i] = reader.ReadInt32();
            }

            this.NumChunkNames = reader.ReadInt32();
            this.chunkNameOffsets = new int[this.NumChunkNames];
            for (var i = 0; i < this.NumChunkNames; i++)
            {
                chunkNameOffsets[i] = reader.ReadInt32();
            }

            this.SectionNames = new List<string>();
            this.ChunkNames = new List<string>();
            LoadStrings(reader, sectionNameOffsets, this.SectionNames);
            LoadStrings(reader, chunkNameOffsets, this.ChunkNames);

            // Read Sections --------------------------------------
            this.Sections = new List<MtbSubSection>();
            this.Children = new List<INavigable>();
            for (var i = 0; i < this.NumSections; i++)
            {
                reader.BaseStream.Position = this.SectionStart + this.sectionOffsets[i];
                int nameIdx = reader.ReadInt32();
                reader.BaseStream.Seek(-4, System.IO.SeekOrigin.Current);

                if (nameIdx >= this.NumSectionNames)
                {
                    throw new InvalidOperationException("Unexpectedly large section name index");
                }

                string sectionName = this.SectionNames[nameIdx];
                MtbSubSection section = null;
                switch (sectionName)
                {
                    case "Header":
                        section = new MtbHeader();
                        break;
                    case "SpuBinary":
                        section = new SpuBinary();
                        break;
                    default:
                        section = new MtbGenericSection();
                        break;
                }

                if (section != null)
                {
                    section.Parent = this;
                    this.Children.Add(section);
                    this.Sections.Add(section);
                    section.Load(reader);
                    section.SectionName = sectionName;
                }
            }

            this.Header = (MtbHeader)FindSection("Header", 0);
        }

        public MtbSubSection FindSection(string sectionName, int skip)
        {
            int sectionIndex = FindSectionNameIndex(sectionName);
            if (sectionIndex < 0) { return null; }
            int skipped = 0;

            for (var i = 0; i < this.Sections.Count; i++)
            {
                MtbSubSection section = this.Sections[i];
                if (!String.Equals(section.SectionName, sectionName))
                {
                    continue;
                }

                if (skip == skipped)
                {
                    return section;
                }

                skipped++;
            }

            return null;
        }

        public int FindSectionNameIndex(string sectionName)
        {
            for (var i = 0; i < NumSectionNames; i++)
            {
                if (!String.Equals(sectionName, this.SectionNames[i]))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        private void LoadStrings(BinaryReaderEx reader, int[] offsets, List<string> list)
        {
            long pos = reader.BaseStream.Position;
            for (var i = 0; i < offsets.Length; i++)
            {
                reader.BaseStream.Position = this.SectionStart + offsets[i];
                list.Add(reader.ReadNullTerminatedString());
            }
            reader.BaseStream.Position = pos;
        }
    }
}
