using SlimDX;

namespace ModelViewer.Renderer
{
    public class Bone
    {
        public int Id;
        public int ParentId;
        public Vector3 Position;
        public Quaternion Orientation;
        public Vector3 Scale;

        public Matrix JointMatrix;
        public Matrix InverseJointMatrix;
        public bool JointMatrixSet;

        public void CalculateJointMatrix()
        {
            if (this.JointMatrixSet) { return; }

            this.JointMatrix = Matrix.Scaling(this.Scale);
            this.JointMatrix *= Matrix.RotationQuaternion(this.Orientation);
            this.JointMatrix *= Matrix.Translation(this.Position);

            this.InverseJointMatrix = Matrix.Invert(this.JointMatrix);
            this.JointMatrixSet = true;
        }

        public void CalculateJointMatrix(ISkeleton baseFrame)
        {
            if (this.JointMatrixSet) { return; }

            this.JointMatrix = Matrix.Scaling(this.Scale);
            this.JointMatrix *= Matrix.RotationQuaternion(this.Orientation);
            this.JointMatrix *= Matrix.Translation(this.Position);

            this.JointMatrix = baseFrame.GetBoneById(this.Id).InverseJointMatrix * this.JointMatrix;

            this.JointMatrixSet = true;
        }
    }
}
