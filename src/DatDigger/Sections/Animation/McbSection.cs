using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Animation
{
    public class RstChunk : INavigable
    {
        public short ChunkBlocks { get; set; } // ChunkSize = ChunkBlocks * 8 bytes/block
        public short NumResources { get; set; }
        public List<int> ResourceOffsets { get; set; }

        public string DisplayName
        {
            get { return "RST"; }
        }

        public INavigable Parent { get; set; }
        public List<INavigable> Children { get; set; }
    }

    public class ResHeader
    {
        public short ChunkBlocks { get; set; } // ChunkSize = ChunkBlocks * 8 bytes/block
        public short Unknown1 { get; set; }
        public string ResType { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
    }

    public class McbResource : SectionBase
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ResHeader ResHeader { get; protected set; }

        public McbResource(ResHeader header) : base()
        {
            this.ResHeader = header;
        }

        public override string DisplayName { get { return this.ResHeader.ResType; } }
        public override void LoadSection(BinaryReaderEx reader) { }
    }

    public class McbStrings : McbResource
    {
        public List<string> Strings { get; protected set; }

        public McbStrings(ResHeader header) : base(header) { }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            long baseOffset = reader.BaseStream.Position;
            List<short> offsets = new List<short>();
            short strOffset = reader.ReadInt16();
            int numOffsets = strOffset / 2;
            offsets.Add(strOffset);
            for (var o = 1; o < numOffsets; o++)
            {
                strOffset = reader.ReadInt16();
                if (strOffset == 0) { continue; }
                offsets.Add(strOffset);
            }

            this.Strings = new List<string>(offsets.Count);
            for (var i = 0; i < offsets.Count; i++)
            {
                reader.BaseStream.Position = baseOffset + offsets[i];
                this.Strings.Add(reader.ReadNullTerminatedString());
            }
        }
    }

    public class McbChunk : INavigable
    {
        public string DisplayName { get; protected set; }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; private set; }

        public McbChunkType ChunkType { get; set; }
        public short ChunkBlocks { get; set; }
        public short NumChildren { get; set; }

        public McbChunk() { this.Children = new List<INavigable>(); }

        public virtual void LoadSection(BinaryReaderEx reader) {
            this.ChunkType = (McbChunkType)reader.ReadInt32();
            this.ChunkBlocks = reader.ReadInt16();
            this.NumChildren = reader.ReadInt16();

            this.DisplayName = this.ChunkType.ToString();
        }
    }

    public class StringEntry : INavigable
    {
        public short StringIndex { get; set; }
        public short Value { get; set; }
        public string DisplayName { get; internal set; }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; private set; }
    }

    public class TrackEntry : INavigable
    {
        public short TrackIndex { get; set; }
        public short Value { get; set; }
        public string DisplayName { get; internal set; }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; private set; }
    }

    public class AttChunk : McbChunk
    {
        public List<StringEntry> Entries { get; private set; }
        public AttChunk() : base() {
            this.Entries = new List<StringEntry>();
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            for (var i = 0; i < this.NumChildren; i++)
            {
                StringEntry entry = new StringEntry()
                {
                    StringIndex = reader.ReadInt16(),
                    Value = reader.ReadInt16(),
                    Parent = this
                };

                entry.DisplayName = entry.StringIndex.ToString();
                this.Children.Add(entry);
            }
        }
    }

    public class CptChunk : McbChunk
    {
        public List<StringEntry> Entries { get; private set; }
        public CptChunk() : base() {
            this.Entries = new List<StringEntry>();
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            for (var i = 0; i < this.NumChildren; i++)
            {
                StringEntry entry = new StringEntry()
                {
                    StringIndex = reader.ReadInt16(),
                    Value = reader.ReadInt16(),
                    Parent = this
                };

                entry.DisplayName = entry.StringIndex.ToString();
                this.Children.Add(entry);
            }
        }
    }

    public class ActChunk : McbChunk
    {
        public ActChunk()
            : base()
        {
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            
        }
    }

    public class TrkChunk : McbChunk
    {
        public List<TrackEntry> Entries { get; private set; }
        public int NumEntries { get; protected set; }
        public TrkChunk()
            : base()
        {
            this.Entries = new List<TrackEntry>();
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.NumEntries = reader.ReadInt32();
            this.Entries.Capacity = this.NumEntries;

            for (var i = 0; i < this.NumEntries; i++)
            {
                TrackEntry entry = new TrackEntry()
                {
                    TrackIndex = reader.ReadInt16(),
                    Value = reader.ReadInt16(),
                    Parent = this
                };

                entry.DisplayName = "TrackEntry " + i.ToString();
                this.Children.Add(entry);
            }
        }
    }

    public enum McbChunkType
    {
        ATT = 0x54544140,
        CPT = 0x54504340,
        ACT = 0x54434140,
        TRK = 0x4B525440,
        BKT = 0x544B4240,
        RES = 0x53455240
    }

    public class McbMotionC : McbResource
    {
        public McbMotionC(ResHeader header) : base(header) {
            this.Children = new List<INavigable>();
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            bool breakLoop = false;
            McbChunk thisChunk;
            while (!breakLoop)
            {
                long streamPos = reader.BaseStream.Position;
                McbChunkType chunkType = (McbChunkType)reader.ReadInt32();
                reader.BaseStream.Position = streamPos;
                thisChunk = null;
                switch (chunkType)
                {
                    case McbChunkType.ATT:
                        thisChunk = new AttChunk();
                        break;
                    case McbChunkType.CPT:
                        thisChunk = new CptChunk();
                        break;
                    case McbChunkType.ACT:
                        thisChunk = new McbChunk();
                        break;
                    case McbChunkType.TRK:
                        thisChunk = new TrkChunk();
                        break;
                    case McbChunkType.BKT:
                        thisChunk = new McbChunk();
                        break;
                    case McbChunkType.RES:
                        // Ran out of chunks
                        break;
                    default:
                        System.Diagnostics.Trace.WriteLine("Unknown MCB Chunk type: " + (int)chunkType);
                        break;
                }

                if (thisChunk != null)
                {
                    thisChunk.LoadSection(reader);
                    reader.BaseStream.Position = streamPos + 8 * thisChunk.ChunkBlocks;
                    thisChunk.Parent = this;
                    this.Children.Add(thisChunk);
                }
                else
                {
                    breakLoop = true;
                }
            }
        }
    }

    public class McbSection : SectionBase
    {
        private RstChunk rstChunk;

        public override string DisplayName
        {
            get { return String.Format("MCB [{0}]", this.ResourceId); }
        }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.Children = new List<INavigable>();

            ReadRst(reader);
        }

        private void ReadRst(BinaryReaderEx reader)
        {
            long rstStart = reader.BaseStream.Position;

            rstChunk = new RstChunk();
            rstChunk.Parent = this;
            int magic = reader.ReadInt32();
            if (magic != 0x54535240) { throw new InvalidOperationException("Expecting @RST (0x54535240), Found " + magic.ToString("X")); }

            rstChunk.ChunkBlocks = reader.ReadInt16();
            rstChunk.NumResources = reader.ReadInt16();
            rstChunk.ResourceOffsets = new List<int>();
            for (var i = 0; i < rstChunk.NumResources; i++)
            {
                rstChunk.ResourceOffsets.Add(reader.ReadInt32());
            }

            this.Children.Add(rstChunk);

            int rstLen = rstChunk.ChunkBlocks * 8;
            long rstOffset = rstStart + rstLen;

            ReadResources(reader, rstOffset);
        }

        private void ReadResources(BinaryReaderEx reader, long rstOffset)
        {
            for (var i = 0; i < this.rstChunk.ResourceOffsets.Count; i++)
            {
                long offset = rstOffset + this.rstChunk.ResourceOffsets[i];
                reader.BaseStream.Position = offset;

                ResHeader resHeader = new ResHeader();
                int magic = reader.ReadInt32();

                if (magic != 0x53455240)
                {
                    throw new InvalidOperationException("Expected @RST (0x53455240), found " + magic.ToString("X"));
                }

                resHeader.ChunkBlocks = reader.ReadInt16();
                resHeader.Unknown1 = reader.ReadInt16();
                resHeader.ResType = reader.ReadFixedLengthString(8).TrimEnd('\0');
                resHeader.Unknown2 = reader.ReadInt32();
                resHeader.Unknown3 = reader.ReadInt32();

                McbResource resource;
                switch (resHeader.ResType)
                {
                    case "String":
                        resource = new McbStrings(resHeader);
                        break;
                    case "motionC":
                        resource = new McbMotionC(resHeader);
                        break;
                    case "RIDTBL":
                    case "Effect":
                    case "ClipGid":
                        resource = new McbResource(resHeader);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown Mcb Resource: " + resHeader.ResType);
                }
                resource.Parent = this;
                resource.LoadSection(reader);
                this.Children.Add(resource);
            }

            var strings = this.GetChildOfType<McbStrings>();
            var stringEntries = this.FindAllChildren<StringEntry>();
            foreach (var entry in stringEntries)
            {
                entry.DisplayName = strings.Strings[entry.StringIndex];
            }
        }
    }
}
