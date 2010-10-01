using System;

namespace ModelViewer.Renderer
{
    public interface IRenderable : IDisposable
    {
        void Init();

        void BeginRender();
        void Render();
        void EndRender();

        void ResourceLoad();
        void ResourceUnload();
    }
}
