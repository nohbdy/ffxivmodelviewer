using System.Collections.Generic;
using System.ComponentModel;

namespace DatDigger.Sections.Skeleton
{
    public class SkeletonBone : INavigable
    {
        [Browsable(false)]
        public string DisplayName { get { return this.Name; } }

        [Browsable(false)]
        public INavigable Parent { get; set; }

        [Browsable(false)]
        public List<INavigable> Children { get { return null; } }

        public const int BytesPerBone = 0xB0;

        public int StringIndex { get; set; }
        public int Unknown1 { get; set; } // Possibly another StringIndex for some other purpose?
        public int Unknown2 { get; set; } // Possibly yet another StringIndex for some other purpose?
        public int Unknown3 { get; set; } // signed integer used for something (values -1, 0, 1)
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }

        public SlimDX.Vector3 Translation;
        public SlimDX.Quaternion Rotation;
        public SlimDX.Vector3 Scale;

        public SlimDX.Vector3 Translation_ { get { return Translation; } }
        public SlimDX.Quaternion Rotation_ { get { return Rotation; } }
        public SlimDX.Vector3 Scale_ { get { return Scale; } }

        public int ParentBoneIndex { get; set; }
        public int ChildBoneIndex { get; set; }
        public int SiblingBoneIndex { get; set; }
        public int BoneIndex { get; set; }
        public byte[] Stuff { get; set; }

        public string Name { get; set; }
    }
}
