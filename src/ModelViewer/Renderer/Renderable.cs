using System;

namespace ModelViewer.Renderer
{
    public abstract class Renderable : IRenderable
    {
        protected GlobalRenderSettings RenderSettings { get; private set; }
        protected SlimDX.Direct3D9.Device Device { get { return this.RenderSettings.Device; } }

        public Renderable()
        {
            this.RenderSettings = GlobalRenderSettings.Instance;
        }

        ~Renderable()
        {
            this.Dispose(false);
        }

        /// <summary>Perform initialization</summary>
        public virtual void Init() { }

        /// <summary>Called before any rendering occurs - perform any per-frame operations</summary>
        public virtual void BeginRender() { }

        /// <summary>Rendering should be performed here</summary>
        public virtual void Render() { }
        public virtual void EndRender() { }

        public virtual void ResourceLoad() { }
        public virtual void ResourceUnload() { }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged) { }
    }
}
