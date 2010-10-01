using System;
using System.Collections.Generic;
using DatDigger;
using DatDigger.Sections.Animation;
using SlimDX;

namespace ModelViewer.Renderer
{
    public class AnimatedSkeleton : ISkeleton
    {
        public bool IsLoaded { get; protected set; }
        public Bone[] Bones { get; protected set; }

        private Dictionary<string, Bone> boneNameMap = new Dictionary<string, Bone>();
        private ISkeleton baseFrame;
        private MtbSection mtbSection;
        private DatDigger.Sections.Skeleton.SkeletonSection skeleton;

        /// <summary>Timer for running the animation</summary>
        public float CurrentTime { get; set; }

        /// <summary>Speed of the Animation (in Frames Per Second)</summary>
        public float Fps { get; set; }

        /// <summary>Number of Frames in the Animation</summary>
        public float TotalFrames { get; private set; }

        /// <summary>Length of animation in Seconds</summary>
        public float AnimationLength { get; private set; }

        public AnimatedSkeleton(ISkeleton baseFrame, MtbSection mtbSection)
        {
            if (baseFrame == null) { throw new ArgumentNullException("baseFrame"); }

            this.baseFrame = baseFrame;
            this.IsLoaded = false;
            this.Bones = null;
            this.mtbSection = mtbSection;

            this.Fps = mtbSection.Header.Float1;
            this.TotalFrames = mtbSection.Header.AnimationLength;
            this.AnimationLength = this.TotalFrames / this.Fps;
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
                if (!thisBone.JointMatrixSet)
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
                    b.CalculateJointMatrix(baseFrame);
                }

                global.JointMatrices[boneIdx * 3] = b.JointMatrix.get_Columns(0);
                global.JointMatrices[boneIdx * 3 + 1] = b.JointMatrix.get_Columns(1);
                global.JointMatrices[boneIdx * 3 + 2] = b.JointMatrix.get_Columns(2);
                boneIdx++;
            }
        }

        public void LoadBones(DatDigger.Sections.Skeleton.SkeletonSection skeleton)
        {
            if (IsLoaded) { throw new InvalidOperationException("AnimatedSkeleton already populated"); }

            this.skeleton = skeleton;
            this.Bones = new Bone[skeleton.Bones.Count];
            Dictionary<int, Bone> boneIdMap = new Dictionary<int, Bone>();
            SpuBinary spuBinary = this.mtbSection.FindChild<SpuBinary>();

            for (var i = 0; i < this.Bones.Length; i++)
            {
                Bone thisBone = new Bone();
                thisBone.Id = i;
                this.Bones[i] = thisBone;

                AnimatedBoneNode aniBoneNode = spuBinary.FindChild<AnimatedBoneNode>(x => x.Bone.BoneId == i);
                AnimatedBone aniBone = (aniBoneNode == null) ? null : aniBoneNode.Bone;
                Vector3 translation;
                Vector3 scale;
                Quaternion rotation;

                if (aniBone != null)
                {
                    translation = new Vector3();
                    scale = new Vector3();

                    translation.X = (aniBone.TranslationX == null) ? skeleton.Bones[i].Translation.X : aniBone.TranslationX.GetValue(0);
                    translation.Y = (aniBone.TranslationY == null) ? skeleton.Bones[i].Translation.Y : aniBone.TranslationY.GetValue(0);
                    translation.Z = (aniBone.TranslationZ == null) ? skeleton.Bones[i].Translation.Z : aniBone.TranslationZ.GetValue(0);
                    scale.X = (aniBone.ScaleX == null) ? skeleton.Bones[i].Scale.X : aniBone.ScaleX.GetValue(0);
                    scale.Y = (aniBone.ScaleY == null) ? skeleton.Bones[i].Scale.Y : aniBone.ScaleY.GetValue(0);
                    scale.Z = (aniBone.ScaleZ == null) ? skeleton.Bones[i].Scale.Z : aniBone.ScaleZ.GetValue(0);
                    rotation = (aniBone.RotationCurve == null) ? skeleton.Bones[i].Rotation : aniBone.RotationCurve.GetValue(0);
                }
                else
                {
                    translation = skeleton.Bones[i].Translation;
                    scale = skeleton.Bones[i].Scale;
                    rotation = skeleton.Bones[i].Rotation;
                }

                boneNameMap[skeleton.Bones[i].Name] = thisBone;
                boneIdMap[skeleton.Bones[i].BoneIndex] = thisBone;

                int parentBoneIndex = skeleton.Bones[i].ParentBoneIndex;
                if (parentBoneIndex >= 0)
                {
                    Bone parent = boneIdMap[parentBoneIndex];
                    thisBone.ParentId = parent.Id;

                    // Transform our translation by our parent's rotation quaternion and add to our parent translation
                    Vector4 transformedPos = Vector3.Transform(translation, parent.Orientation);
                    thisBone.Position.X = transformedPos.X + parent.Position.X;
                    thisBone.Position.Y = transformedPos.Y + parent.Position.Y;
                    thisBone.Position.Z = transformedPos.Z + parent.Position.Z;

                    // Rotate our baseframe orientation by our parent joint's rotation quaternion
                    thisBone.Orientation = rotation * parent.Orientation; // Order is important
                    thisBone.Orientation = Quaternion.Normalize(thisBone.Orientation);

                    thisBone.Scale = Vector3.Modulate(scale, parent.Scale);
                }
                else
                {
                    thisBone.ParentId = -1;

                    thisBone.Orientation = rotation;
                    thisBone.Position = translation;
                    thisBone.Scale = scale;
                }
            }

            this.IsLoaded = true;
        }

        public void Update(float elapsedTimeInSeconds)
        {
            this.CurrentTime += elapsedTimeInSeconds; // In Seconds
            if (CurrentTime > AnimationLength) { CurrentTime -= AnimationLength; }
            if (CurrentTime < 0) { CurrentTime += AnimationLength; }

            float currentFrame = Fps * CurrentTime;

            SpuBinary spuBinary = this.mtbSection.FindChild<SpuBinary>();
            Dictionary<int, Bone> boneIdMap = new Dictionary<int, Bone>();

            for (var i = 0; i < this.Bones.Length; i++)
            {
                var thisBone = this.Bones[i];
                thisBone.JointMatrixSet = false;

                AnimatedBoneNode aniBoneNode = spuBinary.FindChild<AnimatedBoneNode>(x => x.Bone.BoneId == thisBone.Id);
                AnimatedBone aniBone = (aniBoneNode == null) ? null : aniBoneNode.Bone;
                Vector3 translation;
                Vector3 scale;
                Quaternion rotation;

                translation = skeleton.Bones[i].Translation;
                scale = skeleton.Bones[i].Scale;
                rotation = skeleton.Bones[i].Rotation;

                if (aniBone != null)
                {
                    translation.X = (aniBone.TranslationX == null) ? skeleton.Bones[i].Translation.X : aniBone.TranslationX.GetValue(currentFrame);
                    translation.Y = (aniBone.TranslationY == null) ? skeleton.Bones[i].Translation.Y : aniBone.TranslationY.GetValue(currentFrame);
                    translation.Z = (aniBone.TranslationZ == null) ? skeleton.Bones[i].Translation.Z : aniBone.TranslationZ.GetValue(currentFrame);
                    scale.X = (aniBone.ScaleX == null) ? skeleton.Bones[i].Scale.X : aniBone.ScaleX.GetValue(currentFrame);
                    scale.Y = (aniBone.ScaleY == null) ? skeleton.Bones[i].Scale.Y : aniBone.ScaleY.GetValue(currentFrame);
                    scale.Z = (aniBone.ScaleZ == null) ? skeleton.Bones[i].Scale.Z : aniBone.ScaleZ.GetValue(currentFrame);
                    if (aniBone.RotationCurve != null)
                    {
                        rotation = aniBone.RotationCurve.GetValue(currentFrame);
                    }
                }

                boneIdMap[skeleton.Bones[i].BoneIndex] = thisBone;

                int parentBoneIndex = skeleton.Bones[i].ParentBoneIndex;
                if (parentBoneIndex >= 0)
                {
                    Bone parent = boneIdMap[parentBoneIndex];

                    // Transform our translation by our parent's rotation quaternion and add to our parent translation
                    Vector4 transformedPos = Vector3.Transform(translation, parent.Orientation);
                    thisBone.Position.X = transformedPos.X + parent.Position.X;
                    thisBone.Position.Y = transformedPos.Y + parent.Position.Y;
                    thisBone.Position.Z = transformedPos.Z + parent.Position.Z;

                    // Rotate our baseframe orientation by our parent joint's rotation quaternion
                    thisBone.Orientation = rotation * parent.Orientation; // Order is important
                    thisBone.Orientation = Quaternion.Normalize(thisBone.Orientation);

                    thisBone.Scale = Vector3.Modulate(scale, parent.Scale);
                }
                else
                {
                    thisBone.Orientation = rotation;
                    thisBone.Position = translation;
                    thisBone.Scale = scale;
                }
            }
        }
    }
}
