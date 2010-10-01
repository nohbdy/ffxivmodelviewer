using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Skeleton
{
    public class SkeletonSection : SectionBase
    {
        public Skeleton.Header SkeletonHeader { get; protected set; }

        [Browsable(false)]
        public List<Skeleton.SkeletonBone> Bones { get; protected set; }
        public List<string> StringTable { get; protected set; }

        public int BoneCount { get; protected set; }

        private long BasePosition { get; set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.LoadHeader(reader);
            this.LoadStringTable(reader);
            this.LoadBones(reader);
        }

        private void LoadHeader(BinaryReaderEx reader)
        {
            this.SkeletonHeader = new Skeleton.Header();
            this.SkeletonHeader.Unknown1 = reader.ReadInt32();
            this.SkeletonHeader.StringTableOffset = reader.ReadInt32();
            this.SkeletonHeader.NumStrings = reader.ReadInt32();
            this.SkeletonHeader.Lks = reader.ReadInt32();
            this.SkeletonHeader.StringIndex = reader.ReadInt32();
            this.SkeletonHeader.Unknown2 = reader.ReadInt32();
            this.SkeletonHeader.BoneDataLength = reader.ReadInt32();
            this.SkeletonHeader.Unknown3 = reader.ReadInt32();

            this.BasePosition = reader.BaseStream.Position;
            this.BoneCount = this.SkeletonHeader.BoneDataLength / Skeleton.SkeletonBone.BytesPerBone;
        }

        private void LoadBones(BinaryReaderEx reader)
        {
            reader.BaseStream.Position = this.BasePosition;

            this.Bones = new List<Skeleton.SkeletonBone>(this.BoneCount);
            this.Children = new List<INavigable>(this.BoneCount);

            for (var i = 0; i < this.BoneCount; i++)
            {
                Skeleton.SkeletonBone bone = new Skeleton.SkeletonBone();
                bone.Parent = this;
                bone.StringIndex = reader.ReadInt32();
                bone.Unknown1 = reader.ReadInt32();
                bone.Unknown2 = reader.ReadInt32();
                bone.Unknown3 = reader.ReadInt32();
                bone.Translation.X = reader.ReadSingle();
                bone.Translation.Y = reader.ReadSingle();
                bone.Translation.Z = reader.ReadSingle();
                bone.Rotation.X = reader.ReadSingle();
                bone.Rotation.Y = reader.ReadSingle();
                bone.Rotation.Z = reader.ReadSingle();
                bone.Rotation.W = reader.ReadSingle();
                bone.Scale.X = reader.ReadSingle();
                bone.Scale.Y = reader.ReadSingle();
                bone.Scale.Z = reader.ReadSingle();
                bone.ParentBoneIndex = reader.ReadInt32();
                bone.ChildBoneIndex = reader.ReadInt32();
                bone.SiblingBoneIndex = reader.ReadInt32();
                bone.BoneIndex = reader.ReadInt32();
                bone.Unknown4 = reader.ReadInt32();
                bone.Unknown5 = reader.ReadInt32();
                bone.Stuff = reader.ReadBytes(96);
                this.Bones.Add(bone);
                this.Children.Add(bone);

                bone.Name = this.StringTable[bone.StringIndex];
            }
        }

        private void LoadStringTable(BinaryReaderEx reader)
        {
            // Load String Table
            this.StringTable = new List<string>(this.SkeletonHeader.NumStrings);
            reader.BaseStream.Position = this.BasePosition + this.SkeletonHeader.StringTableOffset;

            for (var i = 0; i < this.SkeletonHeader.NumStrings; i++)
            {
                this.StringTable.Add(reader.ReadNullTerminatedString());
            }
        }
    }
}
