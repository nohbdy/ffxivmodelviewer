using System;
using System.Windows.Forms;

using SlimDX;

namespace ModelViewer
{
    public class ArcBall
    {
        protected Vector3 downPt;
        protected Vector3 currentPt;
        protected System.Drawing.Point offset;
        protected Matrix rotationMatrix;
        protected Matrix translationMatrix;
        protected Matrix translationDeltaMatrix;
        protected int windowWidth;
        protected int windowHeight;
        protected Vector2 center;
        protected float radius;
        protected float radiusTranslation;
        protected System.Drawing.Point ptLastMouse;
        protected Quaternion qDown;
        protected Quaternion qNow;
        protected bool isDragging;

        protected Vector3 ScreenToVector(float screenX, float screenY) {
            float x = (screenX - offset.X - windowWidth / 2) / (radius * windowWidth / 2);
            float y = -(screenY - offset.Y - windowHeight / 2) / (radius * windowHeight / 2);
            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                x /= 100f;
                y /= 100f;
            }

            float z = 0;
            float mag = x * x + y * y;

            if (mag > 1.0f)
            {
                float scale = 1.0f / (float)Math.Sqrt(mag);
                x *= scale;
                y *= scale;
            }
            else
            {
                z = (float)Math.Sqrt(1.0f - mag);
            }

            return new Vector3(x, y, z);
        }

        public ArcBall() {
            Reset();
            downPt = new Vector3(0, 0, 0);
            currentPt = new Vector3(0, 0, 0);
            offset.X = 0;
            offset.Y = 0;
        }

        public void Reset()
        {
            qDown = Quaternion.Identity;
            qNow = Quaternion.Identity;
            rotationMatrix = Matrix.Identity;
            translationMatrix = Matrix.Identity;
            translationDeltaMatrix = Matrix.Identity;

            isDragging = false;
            radiusTranslation = 1f;
            radius = 1f;
        }

        public void SetTranslationRadius(float radiusTranslation) { this.radiusTranslation = radiusTranslation; }
        public void SetWindow(int width, int height, float radius = 0.9f)
        {
            this.windowWidth = width;
            this.windowHeight = height;
            this.radius = radius;
            this.center = new Vector2(width / 2f, height / 2f);
        }
        public void SetOffset(int x, int y)
        {
            this.offset.X = x;
            this.offset.Y = y;
        }

        public void OnBegin(int x, int y)
        {
            // Only enter the drag state if the click falls
            // inside the click rectangle.
            if (x >= offset.X &&
                x < offset.X + windowWidth &&
                y >= offset.Y &&
                y < offset.Y + windowHeight)
            {
                isDragging = true;
                qDown = qNow;
                downPt = ScreenToVector((float)x, (float)y);
            }
        }

        public void OnMove(int x, int y)
        {
            if (isDragging)
            {
                currentPt = ScreenToVector((float)x, (float)y);
                qNow = qDown * QuatFromBallPoints(downPt, currentPt);
            }
        }

        public void OnEnd(int x, int y)
        {
            isDragging = false;
        }

        public Matrix GetRotationMatrix()
        {
            Matrix.RotationQuaternion(ref qNow, out rotationMatrix);
            return rotationMatrix;
        }

        public Matrix GetTranslationMatrix
        {
            get { return this.translationMatrix; }
        }

        public Matrix GetTranslationDeltaMatrix
        {
            get { return this.translationDeltaMatrix; }
        }

        public bool IsBeingDragged { get { return isDragging; } }
        public Quaternion QuatNow { get { return qNow; } set { qNow = value; } }

        public static Quaternion QuatFromBallPoints(Vector3 from, Vector3 to) {
            Vector3 part;
            float dot = Vector3.Dot(from, to);
            part = Vector3.Cross(from, to);

            return new Quaternion(part, dot);
        }
    }
}
