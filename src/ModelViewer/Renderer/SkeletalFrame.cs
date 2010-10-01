using System;
using System.Collections.Generic;

using SlimDX;

namespace ModelViewer.Renderer
{
    public class SkeletalFrame : ISkeleton
    {
        public bool IsLoaded { get; protected set; }
        public Bone[] Bones { get; protected set; }

        private Dictionary<string, Bone> boneNameMap = new Dictionary<string, Bone>();
        private ISkeleton baseFrame;

        public SkeletalFrame(ISkeleton baseFrame)
        {
            this.baseFrame = baseFrame;
            if (baseFrame == null) { this.baseFrame = this; }
            this.IsLoaded = false;
            this.Bones = null;
        }

        public Bone GetBoneById(int id)
        {
            return this.Bones[id];
        }

        public int GetBoneId(string boneName)
        {
            Bone bone;
            if (boneNameMap.TryGetValue(boneName, out bone))
            {
                return bone.Id;
            }
            else
            {
                throw new InvalidOperationException("Cannot find bone named " + boneName);
            }
        }

        public int[] BoneNamesToIds(List<string> boneNames)
        {
            int[] result = new int[boneNames.Count];
            for (var i = 0; i < boneNames.Count; i++)
            {
                result[i] = boneNameMap[boneNames[i]].Id;
            }

            return result;
        }

        public void CalculateJointMatrices()
        {
            for (var i = 0; i < this.Bones.Length; i++)
            {
                Bone thisBone = this.Bones[i];
                if (baseFrame == this)
                {
                    thisBone.CalculateJointMatrix();
                }
                else
                {
                    thisBone.CalculateJointMatrix(baseFrame);
                }
            }
        }

        public void LoadMatrices(int[] boneList)
        {
            if (boneList.Length > 48) { throw new InvalidOperationException("Too many bones! Expected < 48, got " + boneList.Length); }

            var global = GlobalRenderSettings.Instance;
            int boneIdx = 0;

            for (var i = 0; i < boneList.Length; i++)
            {
                Bone b = this.Bones[boneList[i]];
                if (!b.JointMatrixSet)
                {
                    if (baseFrame == this)
                    {
                        b.CalculateJointMatrix();
                    }
                    else
                    {
                        b.CalculateJointMatrix(baseFrame);
                    }
                }

                global.JointMatrices[boneIdx * 3] = b.JointMatrix.get_Columns(0);
                global.JointMatrices[boneIdx * 3 + 1] = b.JointMatrix.get_Columns(1);
                global.JointMatrices[boneIdx * 3 + 2] = b.JointMatrix.get_Columns(2);
                boneIdx++;
            }
        }

        public void LoadBones(DatDigger.Sections.Skeleton.SkeletonSection skeleton)
        {
            if (IsLoaded) { throw new InvalidOperationException("SkeletalFrame already populated"); }

            this.Bones = new Bone[skeleton.Bones.Count];
            Dictionary<int, Bone> boneIdMap = new Dictionary<int, Bone>();

            for (var i = 0; i < this.Bones.Length; i++)
            {
                Bone thisBone = new Bone();
                thisBone.Id = i;
                this.Bones[i] = thisBone;

                boneNameMap[skeleton.Bones[i].Name] = thisBone;
                boneIdMap[skeleton.Bones[i].BoneIndex] = thisBone;

                int parentBoneIndex = skeleton.Bones[i].ParentBoneIndex;
                if (parentBoneIndex >= 0)
                {
                    Bone parent = boneIdMap[skeleton.Bones[i].ParentBoneIndex];
                    thisBone.ParentId = parent.Id;

                    // Transform our translation by our parent's rotation quaternion and add to our parent translation
                    Vector4 transformedPos = Vector3.Transform(skeleton.Bones[i].Translation, parent.Orientation);
                    thisBone.Position.X = transformedPos.X + parent.Position.X;
                    thisBone.Position.Y = transformedPos.Y + parent.Position.Y;
                    thisBone.Position.Z = transformedPos.Z + parent.Position.Z;

                    // Rotate our baseframe orientation by our parent joint's rotation quaternion
                    thisBone.Orientation = skeleton.Bones[i].Rotation * parent.Orientation; // Order is important
                    thisBone.Orientation = Quaternion.Normalize(thisBone.Orientation);

                    thisBone.Scale = Vector3.Modulate(skeleton.Bones[i].Scale, parent.Scale);
                }
                else
                {
                    thisBone.ParentId = -1;

                    thisBone.Orientation = skeleton.Bones[i].Rotation;
                    thisBone.Position = skeleton.Bones[i].Translation;
                    thisBone.Scale = skeleton.Bones[i].Scale;
                }

                if (this.baseFrame == this)
                {
                    thisBone.CalculateJointMatrix();
                }
            }

            this.IsLoaded = true;
        }
    }
}
