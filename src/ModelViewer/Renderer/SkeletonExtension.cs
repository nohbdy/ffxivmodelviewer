using System;
using System.Collections.Generic;

using SlimDX;

namespace ModelViewer.Renderer
{
    public class SkeletonExtension : ISkeleton
    {
        public bool IsLoaded { get; protected set; }
        public Bone[] Bones { get; protected set; }

        private ISkeleton extendingFrame;
        private ISkeleton baseFrame;
        private Dictionary<string, Bone> boneNameMap = new Dictionary<string, Bone>();

        public ISkeleton CurrentFrame { get; set; }

        public SkeletonExtension(ISkeleton extendingFrame, ISkeleton baseFrame)
        {
            this.extendingFrame = extendingFrame;
            this.baseFrame = baseFrame;
            if (this.baseFrame == null) { this.baseFrame = this; }
        }

        public Bone GetBoneById(int id)
        {
            if ((id & Int32.MinValue) == 0)
            {
                return this.Bones[id];
            }
            else
            {
                return this.CurrentFrame.Bones[id ^ Int32.MinValue];
            }
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
                if (this.extendingFrame != null)
                {
                    return this.extendingFrame.GetBoneId(boneName);
                }

                throw new InvalidOperationException("Cannot find bone named " + boneName);
            }
        }

        public int[] BoneNamesToIds(List<string> boneNames)
        {
            int[] result = new int[boneNames.Count];
            Bone bone;
            for (var i = 0; i < boneNames.Count; i++)
            {
                if (boneNameMap.TryGetValue(boneNames[i], out bone))
                {
                    result[i] = bone.Id;
                }
                else
                {
                    // Retrieve bone ID from baseFrame
                    int boneId = this.extendingFrame.GetBoneId(boneNames[i]);
                    result[i] = boneId | Int32.MinValue; // Set the MSB to 1 to signify it is contained in the base skeleton and not this one
                }
            }

            return result;
        }

        public void CalculateJointMatrices()
        {
            for (var i = 0; i < this.Bones.Length; i++)
            {
                Bone thisBone = this.Bones[i];
                int boneId = thisBone.Id;
                if ((boneId & Int32.MinValue) == 0)
                {
                    thisBone.CalculateJointMatrix(this.baseFrame);
                }
                else
                {
                    boneId = boneId ^ Int32.MinValue; // Unset MSB
                    thisBone = this.CurrentFrame.Bones[boneId];
                    thisBone.CalculateJointMatrix(this.extendingFrame);
                }
            }
        }

        public void LoadMatrices(int[] boneList)
        {
            if (boneList.Length > 48) { throw new InvalidOperationException("Too many bones! Expected < 48, got " + boneList.Length); }

            var global = GlobalRenderSettings.Instance;
            int boneIdx = 0;
            int boneId;
            Bone b;
            for (var i = 0; i < boneList.Length; i++)
            {
                boneId = boneList[i];
                if ((boneId & Int32.MinValue) == 0)
                {
                    // MSB is NOT set - this is a local bone
                    b = this.Bones[boneId];

                    if (!b.JointMatrixSet)
                    {
                        b.CalculateJointMatrix(this.baseFrame);
                    }
                }
                else
                {
                    // MSB is set - this bone is in the main skeleton
                    boneId = boneId ^ Int32.MinValue; // Unset the MSB bit
                    b = this.CurrentFrame.Bones[boneId];

                    if (!b.JointMatrixSet)
                    {
                        b.CalculateJointMatrix(this.extendingFrame);
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
                    //thisBone.Orientation = parent.Orientation * skeleton.Bones[i].Rotation; // Order is important
                    thisBone.Orientation = Quaternion.Normalize(thisBone.Orientation);

                    thisBone.Scale = Vector3.Modulate(skeleton.Bones[i].Scale, parent.Scale);
                }
                else if (parentBoneIndex == -1)
                {
                    thisBone.ParentId = -1;

                    thisBone.Orientation = skeleton.Bones[i].Rotation;
                    thisBone.Position = skeleton.Bones[i].Translation;
                    thisBone.Scale = skeleton.Bones[i].Scale;
                }
                else
                {
                    // Parent bone is a bone in the main skeleton
                    int parentIdx = parentBoneIndex ^ Int32.MinValue; // Unset the MSB
                    Bone parent = this.extendingFrame.Bones[parentIdx];
                    thisBone.ParentId = parentBoneIndex;

                    // Transform our translation by our parent's rotation quaternion and add to our parent translation
                    Vector4 transformedPos = Vector3.Transform(skeleton.Bones[i].Translation, parent.Orientation);
                    thisBone.Position.X = transformedPos.X + parent.Position.X;
                    thisBone.Position.Y = transformedPos.Y + parent.Position.Y;
                    thisBone.Position.Z = transformedPos.Z + parent.Position.Z;

                    // Rotate our baseframe orientation by our parent joint's rotation quaternion
                    thisBone.Orientation = skeleton.Bones[i].Rotation * parent.Orientation; // Order is important
                    //thisBone.Orientation = parent.Orientation * skeleton.Bones[i].Rotation; // Order is important
                    thisBone.Orientation = Quaternion.Normalize(thisBone.Orientation);

                    thisBone.Scale = Vector3.Modulate(skeleton.Bones[i].Scale, parent.Scale);
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
