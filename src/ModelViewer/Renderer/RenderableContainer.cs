using System.Collections.Generic;

namespace ModelViewer.Renderer
{
    public abstract class RenderableContainer<T> : Renderable where T : Renderable
    {
        protected List<T> Children = new List<T>();

        public override void Init()
        {
            base.Init();

            Children.ForEach(x => x.Init());
        }

        public override void ResourceLoad()
        {
            base.ResourceLoad();

            Children.ForEach(x => x.ResourceLoad());
        }

        public override void ResourceUnload()
        {
            base.ResourceUnload();

            Children.ForEach(x => x.ResourceUnload());
        }

        public override void BeginRender()
        {
            base.BeginRender();

            Children.ForEach(x => x.BeginRender());
        }

        public override void Render()
        {
            base.Render();

            Children.ForEach(x => x.Render());
        }

        public override void EndRender()
        {
            base.EndRender();

            Children.ForEach(x => x.EndRender());
        }

        protected override void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                Children.ForEach(x => x.Dispose());
            }

            base.Dispose(disposeManaged);
        }
    }
}
