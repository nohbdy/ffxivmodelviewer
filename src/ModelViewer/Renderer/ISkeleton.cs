using System.Collections.Generic;

namespace ModelViewer.Renderer
{
    public interface ISkeleton
    {
        bool IsLoaded { get; }
        Bone[] Bones { get; }

        Bone GetBoneById(int id);
        int GetBoneId(string name);
        int[] BoneNamesToIds(List<string> boneNames);
        void LoadMatrices(int[] boneList);
        void LoadBones(DatDigger.Sections.Skeleton.SkeletonSection skeleton);
        void CalculateJointMatrices();
    }
}
