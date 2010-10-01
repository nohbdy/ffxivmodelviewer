using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.UI
{
    public class SparkleImage : INavigable
    {
        public SparkleImage() { this.Children = new List<INavigable>(); }

        public string FileName { get; internal set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SparkleImageClip ImageClip { get; internal set; }

        public string DisplayName { get { return FileName; } }
        public INavigable Parent { get; internal set; }
        public List<INavigable> Children { get; protected set; }
    }

    public abstract class SparkleBlock
    {
        public short Type { get; protected set; }

        public abstract void Load(BinaryReaderEx reader);
    }

    public class SparkleImageClip : SparkleBlock
    {
        public short X { get; private set; }
        public short Y { get; private set; }
        public short Width { get; private set; }
        public short Height { get; private set; }

        public override void Load(BinaryReaderEx reader)
        {
            Type = reader.ReadInt16();
            reader.ReadInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
        }
    }

    public class SparkleType1 : SparkleBlock
    {
        public short Unk1 { get; private set; }
        public int Unk2 { get; private set; }
        public float Float1 { get; private set; }
        public float Float2 { get; private set; }
        public float Float3 { get; private set; }
        public float Float4 { get; private set; }
        public float Float5 { get; private set; }
        public float Float6 { get; private set; }

        public override void Load(BinaryReaderEx reader)
        {
            Type = reader.ReadInt16();
            Unk1 = reader.ReadInt16();
            Unk2 = reader.ReadInt32();
            Float1 = reader.ReadSingle();
            Float2 = reader.ReadSingle();
            Float3 = reader.ReadSingle();
            Float4 = reader.ReadSingle();
            Float5 = reader.ReadSingle();
            Float6 = reader.ReadSingle();
        }
    }

    public abstract class SparkleChunkBase
    {
        public short Type { get; protected set; }
        public short Reference { get; protected set; }

        public abstract void Load(BinaryReaderEx reader);
    }

    public class SparkleChunk1 : SparkleChunkBase {
        public override void Load(BinaryReaderEx reader)
        {
            reader.BaseStream.Position += 10;
            this.Reference = reader.ReadInt16();
        }
    }

    public class SparkleChunk2 : SparkleChunkBase {
        public override void Load(BinaryReaderEx reader)
        {
            reader.BaseStream.Position += 22;
            this.Reference = reader.ReadInt16();
        }
    }

    public class SparkleChunk3 : SparkleChunkBase {
        public override void Load(BinaryReaderEx reader)
        {
            this.Reference = 0;
        }
    }

    public class SparkleContainer : SparkleBlock
    {
        public SparkleContainer()
        {
            this.Offsets = new List<int>();
            this.Strings = new List<string>();
            this.StringOffsets = new List<int>();
            this.Chunks = new List<SparkleChunkBase>();
        }

        public long BasePosition { get; private set; }
        public short Unk1 { get; private set; }
        public short Unk2 { get; private set; }
        public short NumOffsets { get; private set; }
        public List<int> Offsets { get; private set; }
        public long StringTableBase { get; private set; }
        public int StringTableOffset { get; private set; }
        public int NumStrings { get; private set; }
        public List<int> StringOffsets { get; private set; }
        public List<string> Strings { get; private set; }
        public List<SparkleChunkBase> Chunks { get; private set; }

        public override void Load(BinaryReaderEx reader)
        {
            BasePosition = reader.BaseStream.Position;
            Type = reader.ReadInt16();
            Unk1 = reader.ReadInt16();
            Unk2 = reader.ReadInt16();
            NumOffsets = reader.ReadInt16();
            StringTableOffset = reader.ReadInt32();

            for (var i = 0; i < NumOffsets; i++)
            {
                Offsets.Add(reader.ReadInt32());
            }

            if (StringTableOffset != 0)
            {
                reader.BaseStream.Position = BasePosition + StringTableOffset;
                StringTableBase = BasePosition + StringTableOffset;
                NumStrings = reader.ReadInt32();

                for (var i = 0; i < NumStrings; i++)
                {
                    StringOffsets.Add(reader.ReadInt32());
                }

                for (var i = 0; i < NumStrings; i++)
                {
                    reader.BaseStream.Position = StringTableBase + StringOffsets[i] + 4;
                    Strings.Add(reader.ReadNullTerminatedString());
                }
            }

            for (var i = 0; i < NumOffsets; i++)
            {
                reader.BaseStream.Position = BasePosition + this.Offsets[i];
                short type = reader.ReadInt16();
                SparkleChunkBase c;
                switch (type)
                {
                    case 1:
                        c = new SparkleChunk1();
                        break;
                    case 2:
                        c = new SparkleChunk2();
                        break;
                    case 3:
                        c = new SparkleChunk3();
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected Sparkle Chunk Type: " + type);
                }

                reader.BaseStream.Position = BasePosition + this.Offsets[i];
                c.Load(reader);
                this.Chunks.Add(c);
            }
        }
    }

    public class SparkleFile : INavigable
    {
        protected string FilePath { get; private set; }
        public long BasePosition { get; private set; }
        public short Unknown1 { get; private set; }
        public short NumOffsets { get; private set; }
        public short Unknown2 { get; private set; }
        public short Unknown3 { get; private set; }
        public short Unknown4 { get; private set; }
        public short NumFiles { get; private set; }
        public int Unknown5 { get; private set; }

        public List<int> Offsets { get; private set; }
        public List<SparkleBlock> Blocks { get; private set; }
        public List<SparkleImage> Images { get; private set; }

        public SparkleFile(string path)
        {
            this.FilePath = path;
            this.Children = new List<INavigable>();
            this.Parent = null;
            this.Offsets = new List<int>();
            this.Blocks = new List<SparkleBlock>();
            this.Images = new List<SparkleImage>();
        }

        public string DisplayName
        {
            get { return System.IO.Path.GetFileName(FilePath); }
        }

        public INavigable Parent { get; protected set; }
        public List<INavigable> Children { get; private set; }

        public void LoadFile(BinaryReaderEx reader)
        {
            BasePosition = reader.BaseStream.Position;

            reader.ReadInt32();
            Unknown1 = reader.ReadInt16();
            NumOffsets = reader.ReadInt16();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadInt16();
            Unknown4 = reader.ReadInt16();
            NumFiles = reader.ReadInt16();
            Unknown5 = reader.ReadInt32();

            for (var i = 0; i < NumOffsets; i++)
            {
                this.Offsets.Add(reader.ReadInt32());
            }

            SparkleBlock b;
            for (var i = 0; i < NumOffsets; i++)
            {
                long offsetPos = BasePosition + this.Offsets[i];
                reader.BaseStream.Position = offsetPos;
                short type = reader.ReadInt16();
                switch (type)
                {
                    case 0:
                        b = new SparkleImageClip();
                        break;
                    case 1:
                        b = new SparkleType1();
                        break;
                    case 2:
                        b = new SparkleContainer();
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected SparkleBlock Type: " + type);
                }

                reader.BaseStream.Position = offsetPos;
                b.Load(reader);
                this.Blocks.Add(b);
            }

            SparkleContainer fileContainer = this.Blocks[0] as SparkleContainer;
            if (fileContainer == null)
            {
                throw new InvalidOperationException("First sparkle block is not a container?");
            }

            SparkleImage img;
            for (var i = 0; i < NumFiles; i++)
            {
                img = new SparkleImage();
                img.Parent = this;

                if (i < fileContainer.Strings.Count)
                {
                    img.FileName = fileContainer.Strings[i];
                }

                SparkleChunkBase c = fileContainer.Chunks[i];
                if (c.Reference == 0)
                {
                    img.ImageClip = null;
                }
                else
                {
                    b = this.Blocks[c.Reference - 1];
                    int recursion = 0;
                    while (recursion < 5)
                    {
                        if (b is SparkleImageClip)
                        {
                            img.ImageClip = (SparkleImageClip)b;
                            break;
                        }
                        else if (b is SparkleType1)
                        {
                            img.ImageClip = null;
                            break;
                        }
                        else if (b is SparkleContainer)
                        {
                            SparkleContainer cont = (SparkleContainer)b;
                            if (cont.Chunks[0].Reference == 0)
                            {
                                img.ImageClip = null;
                                break;
                            }

                            b = this.Blocks[cont.Chunks[0].Reference - 1];
                        }
                        recursion++;
                    }
                }

                if (img.ImageClip != null)
                {
                    this.Children.Add(img);
                    this.Images.Add(img);
                }
            }
        }
    }
}
