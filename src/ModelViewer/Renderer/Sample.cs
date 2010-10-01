/*
* Copyright (c) 2007-2010 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using SlimDX.Windows;

namespace ModelViewer.Renderer
{
    /// <summary>
    /// Implements core application logic of a SlimDX sample.
    /// 
    /// The Sample class provides a minimal wrapper around window setup, user
    /// interaction, and OS-level details, but provides very little abstraction
    /// of the underlying DirectX functionality. The reason for this is that the 
    /// purpose of a SlimDX sample is to illustrate how a particular technique 
    /// might be implemented using SlimDX; providing high level rendering abstractions
    /// in the sample framework simplify obfuscates that.
    /// 
    /// A sample is implemented by overriding various base class methods (those prefixed
    /// with "on"). 
    /// </summary>
    public class Sample : IDisposable
    {
        #region Public Interface

        /// <summary>
        /// Gets the width of the renderable area of the sample window.
        /// </summary>
        public int WindowWidth
        {
            get { return _configuration.WindowWidth; }
        }

        /// <summary>
        /// Gets the number of seconds passed since the last frame.
        /// </summary>
        public float FrameDelta { get; private set; }

        /// <summary>
        /// Gets the height of the renderable area of the sample window.
        /// </summary>
        public int WindowHeight
        {
            get { return _configuration.WindowHeight; }
        }

        public UserInterface UserInterface
        {
            get { return userInterface; }
        }

        /// <summary>
        /// Represents a Direct3D9 Context, only valid after calling InitializeDevice(DeviceSettings9)
        /// </summary>
        public DeviceContext9 Context { get; private set; }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if (userInterfaceRenderer != null)
                {
                    userInterfaceRenderer.Dispose();
                }

                renderList.ForEach(x => x.Dispose());
                this.globalSettings.Dispose();
                apiContext.Dispose();
                _form.Dispose();
            }
        }

        protected virtual Form CreateForm(SampleConfiguration config)
        {
            return new RenderForm(config.WindowTitle)
            {
                ClientSize = new Size(config.WindowWidth, config.WindowHeight)
            };
        }

        /// <summary>
        /// Runs the sample.
        /// </summary>
        public void Run()
        {
            _configuration = OnConfigure();
            _form = CreateForm(_configuration);

            currentFormWindowState = _form.WindowState;

            bool isFormClosed = false;
            bool formIsResizing = false;

            _form.MouseClick += HandleMouseClick;
            _form.KeyDown += HandleKeyDown;
            _form.KeyUp += HandleKeyUp;
            _form.MouseDown += HandleMouseDown;
            _form.MouseUp += HandleMouseUp;
            _form.MouseMove += HandleMouseMove;
            _form.MouseWheel += HandleMouseMove;

            _form.Resize += (o, args) =>
            {
                if (_form.WindowState != currentFormWindowState)
                {
                    HandleResize(o, args);
                }

                currentFormWindowState = _form.WindowState;
            };

            _form.ResizeBegin += (o, args) => { formIsResizing = true; };
            _form.ResizeEnd += (o, args) =>
            {
                formIsResizing = false;
                HandleResize(o, args);
            };

            _form.Closed += (o, args) => { isFormClosed = true; };

            userInterface = new UserInterface();
            var stats = new Element();
            stats.SetBinding("Label", framesPerSecond);
            userInterface.Container.Add(stats);

            arcBall.SetWindow(_form.ClientSize.Width, _form.ClientSize.Height);

            var settings = new DeviceSettings9
            {
                AdapterOrdinal = 0,
                CreationFlags = CreateFlags.HardwareVertexProcessing,
                Width = WindowWidth,
                Height = WindowHeight
            };

            InitializeDevice(settings);

            OnInitialize();
            this.renderList.ForEach(x => x.Init());

            OnResourceLoad();

            clock.Start();
            MessagePump.Run(_form, () =>
            {
                if (isFormClosed)
                {
                    return;
                }

                Update();
                if (!formIsResizing)
                {
                    Render(false);
                }
            });

            OnResourceUnload();
        }

        /// <summary>
        /// In a derived class, implements logic to control the configuration of the sample
        /// via a <see cref="SampleConfiguration"/> object.
        /// </summary>
        /// <returns>A <see cref="SampleConfiguration"/> object describing the desired configuration of the sample.</returns>
        protected virtual SampleConfiguration OnConfigure()
        {
            return new SampleConfiguration();
        }

        /// <summary>
        /// In a derived class, implements logic to initialize the sample.
        /// </summary>
        protected virtual void OnInitialize() { }

        protected virtual void OnResourceLoad() {
            //userInterfaceRenderer = new UserInterfaceRenderer9(Context.Device, WindowWidth, WindowHeight);
            this.renderList.ForEach(x => x.ResourceLoad());

            this.globalSettings.ProjectionMatrix = SlimDX.Matrix.PerspectiveFovRH(3.14159f / 4, (float)WindowWidth / (float)WindowHeight, 1.0f, 1000f);
        }

        protected virtual void OnResourceUnload() {
            this.renderList.ForEach(x => x.ResourceUnload());
            //userInterfaceRenderer.Dispose();
        }

        /// <summary>
        /// In a derived class, implements logic to update any relevant sample state.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// In a derived class, implements logic to render the sample.
        /// </summary>
        protected virtual void OnRender() {
            this.renderList.ForEach(x => x.Render());
        }

        /// <summary>
        /// In a derived class, implements logic that should occur before all
        /// other rendering.
        /// </summary>
        protected virtual void OnRenderBegin(bool isScreenshot) {
            this.globalSettings.ShaderManager.Begin();
            this.globalSettings.TextureManager.Begin();

            if (EnableCamera)
            {
                SlimDX.Vector3 forward = cameraPosition + cameraForward;
                this.globalSettings.SetViewMatrix(cameraPosition, forward, cameraUp);
            }

            this.globalSettings.SetWorldMatrix(arcBall.GetRotationMatrix(), zoom, sceneOffset);

            SlimDX.Color4 clearColor = (isScreenshot) ? new SlimDX.Color4(0, 0, 0, 0) : new SlimDX.Color4(0.3f, 0.3f, 0.3f);
            Context.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, clearColor, 1.0f, 0);
            Context.Device.BeginScene();

            this.renderList.ForEach(x => x.BeginRender());
        }

        /// <summary>
        /// In a derived class, implements logic that should occur after all
        /// other rendering.
        /// </summary>
        protected virtual void OnRenderEnd(bool isScreenshot) {
            this.renderList.ForEach(x => x.EndRender());

            Context.Device.EndScene();
            if (!isScreenshot)
            {
                Context.Device.Present();
            }
        }

        /// <summary>
        /// Initializes a <see cref="DeviceContext9">Direct3D9 device context</see> according to the specified settings.
        /// The base class retains ownership of the context and will dispose of it when appropriate.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The initialized device context.</returns>
        protected void InitializeDevice(DeviceSettings9 settings)
        {
            var result = new DeviceContext9(_form.Handle, settings);
            userInterfaceRenderer = new UserInterfaceRenderer9(result.Device, settings.Width, settings.Height);
            apiContext = result;
            Context = result;

            this.globalSettings.Device = Context.Device;
            this.globalSettings.Init();
        }

        /// <summary>
        /// Quits the sample.
        /// </summary>
        protected void Quit()
        {
            _form.Close();
        }

        #endregion

        #region Implementation Detail

        private readonly Clock clock = new Clock();
        private readonly Bindable<float> framesPerSecond = new Bindable<float>();
        private IDisposable apiContext;
        private SampleConfiguration _configuration;
        private FormWindowState currentFormWindowState;
        private Form _form;
        private float frameAccumulator;
        private int frameCount;
        private bool deviceLost = false;
        private UserInterface userInterface;
        private UserInterfaceRenderer userInterfaceRenderer;
        private SlimDX.Direct3D9.Surface screenshotSurface;

        protected ModelViewer.Renderer.GlobalRenderSettings globalSettings;
        protected float zoomSensitivity = 0.005f;
        protected float zoom = 1;
        protected SlimDX.Vector3 sceneOffset = new SlimDX.Vector3(0, 0, 0);
        protected ModelViewer.ArcBall arcBall = new ModelViewer.ArcBall();
        protected bool EnableCamera = true;
        protected SlimDX.Vector3 cameraPosition = new SlimDX.Vector3(0, 0, 10);
        protected SlimDX.Vector3 cameraForward = new SlimDX.Vector3(0, 0, -1);
        protected SlimDX.Vector3 cameraUp = new SlimDX.Vector3(0, 1, 0);
        protected SlimDX.Vector3 cameraRight = new SlimDX.Vector3(1, 0, 0);
        protected float cameraForwardDirection = 0f;
        protected float cameraRaiseDirection = 0f;
        protected float cameraStrafeDirection = 0f;
        protected float cameraVelocity = 2; // World units per second
        protected List<IRenderable> renderList = new List<IRenderable>();

        public Sample()
        {
            this.globalSettings = ModelViewer.Renderer.GlobalRenderSettings.Instance;
        }

        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~Sample()
        {
            Dispose(false);
        }

        /// <summary>
        /// Updates sample state.
        /// </summary>
        private void Update()
        {
            FrameDelta = clock.Update();
            userInterface.Container.Update();

            if (EnableCamera)
            {
                float cameraDist = cameraVelocity * FrameDelta;
                if (cameraForwardDirection != 0)
                {
                    cameraPosition = cameraPosition + cameraForward * cameraDist * cameraForwardDirection;
                }

                if (cameraStrafeDirection != 0)
                {
                    cameraPosition = cameraPosition + cameraRight * cameraDist * cameraStrafeDirection;
                }

                if (cameraRaiseDirection != 0)
                {
                    cameraPosition = cameraPosition + cameraUp * cameraDist * cameraRaiseDirection;
                }
            }

            OnUpdate();
        }

        /// <summary>
        /// Renders the sample.
        /// </summary>
        private void Render(bool isScreenshot)
        {
            if (deviceLost)
            {
                if (Context.Device.TestCooperativeLevel() == SlimDX.Direct3D9.ResultCode.DeviceNotReset)
                {
                    Context.Device.Reset(Context.PresentParameters);
                    deviceLost = false;
                    userInterfaceRenderer = new UserInterfaceRenderer9(Context.Device, WindowWidth, WindowHeight);
                    OnResourceLoad();
                }
                else
                {
                    Thread.Sleep(100);
                    return;
                }
            }

            frameAccumulator += FrameDelta;
            ++frameCount;
            if (frameAccumulator >= 1.0f)
            {
                framesPerSecond.Value = frameCount / frameAccumulator;

                frameAccumulator = 0.0f;
                frameCount = 0;
            }

            SlimDX.Direct3D9.Surface renderTarget = null;
            try
            {
                if (isScreenshot)
                {
                    renderTarget = Context.Device.GetRenderTarget(0);
                    if (!Context.Direct3D.CheckDepthStencilMatch(0, DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8, SlimDX.Direct3D9.Format.A8R8G8B8, SlimDX.Direct3D9.Format.D24X8))
                    {
                        throw new InvalidOperationException("DepthStencil mismatch");
                    }
                    screenshotSurface = SlimDX.Direct3D9.Surface.CreateRenderTarget(Context.Device, WindowWidth, WindowHeight, SlimDX.Direct3D9.Format.A8R8G8B8, Context.MultisampleType, Context.MultisampleQuality, false);
                    Context.Device.SetRenderTarget(0, screenshotSurface);
                }
                OnRenderBegin(isScreenshot);
                OnRender();
                if (userInterfaceRenderer != null && isScreenshot == false)
                {
                    userInterfaceRenderer.Render(userInterface);
                }
                OnRenderEnd(isScreenshot);
                if (isScreenshot)
                {
                    string myPicturesPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                    string fileName = string.Format("ss{0}.png", System.DateTime.Now.ToFileTime());
                    string filePath = System.IO.Path.Combine(myPicturesPath, fileName);
                    SlimDX.Direct3D9.Surface.ToFile(screenshotSurface, filePath, ImageFileFormat.Png);
                    screenshotSurface.Dispose();
                    screenshotSurface = null;
                    Context.Device.SetRenderTarget(0, renderTarget);
                }
            }
            catch (SlimDX.Direct3D9.Direct3D9Exception e)
            {
                if (screenshotSurface != null) { screenshotSurface.Dispose(); }

                if (e.ResultCode == SlimDX.Direct3D9.ResultCode.DeviceLost)
                {
                    OnResourceUnload();
                    userInterfaceRenderer.Dispose();
                    deviceLost = true;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (renderTarget != null) { renderTarget.Dispose(); }
            }
        }

        private void RenderScreenshot()
        {
            this.Render(true);
        }

        private void HandleMouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
            {
                arcBall.OnBegin(e.X, e.Y);
            }
        }

        private void HandleMouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
            {
                arcBall.OnEnd(e.X, e.Y);
            }
        }

        private void HandleMouseMove(object sender, MouseEventArgs e) {
            if (e.Delta != 0)
            {
                // Zoom in or out based on mouse wheel movement
                float zoomAmt = zoomSensitivity;
                if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                {
                    zoomAmt /= 100f;
                }

                zoom += zoomAmt * e.Delta;
                if (zoom < 0.5f) { zoom = 0.5f; }
                return;
            }

            arcBall.OnMove(e.X, e.Y);
        }

        /// <summary>
        /// Handles a mouse click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void HandleMouseClick(object sender, MouseEventArgs e) { }

        /// <summary>
        /// Handles a key down event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (EnableCamera)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        // Move forward in space
                        cameraForwardDirection = 1f;
                        break;
                    case Keys.S:
                        // Move backwards in space
                        cameraForwardDirection = -1f;
                        break;
                    case Keys.A:
                        // Strafe left
                        cameraStrafeDirection = -1f;
                        break;
                    case Keys.D:
                        // Strafe right
                        cameraStrafeDirection = 1f;
                        break;
                    case Keys.PageUp:
                        cameraRaiseDirection = 1f;
                        break;
                    case Keys.PageDown:
                        cameraRaiseDirection = -1f;
                        break;
                    case Keys.R:
                        cameraPosition = new SlimDX.Vector3(0, 0, 10);
                        break;
                }
            }
            OnKeyDown(sender, e);
        }

        protected virtual void OnKeyDown(object sender, KeyEventArgs e) { }

        /// <summary>
        /// Handles a key up event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (EnableCamera)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                    case Keys.S:
                        cameraForwardDirection = 0;
                        break;
                    case Keys.A:
                    case Keys.D:
                        cameraStrafeDirection = 0;
                        break;
                    case Keys.PageDown:
                    case Keys.PageUp:
                        cameraRaiseDirection = 0;
                        break;
                }
            }

            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                OnResourceUnload();

                isFullScreen = !isFullScreen;

                if (Context != null)
                {
                    userInterfaceRenderer.Dispose();

                    Context.PresentParameters.BackBufferWidth = _configuration.WindowWidth;
                    Context.PresentParameters.BackBufferHeight = _configuration.WindowHeight;
                    Context.PresentParameters.Windowed = !isFullScreen;

                    if (!isFullScreen)
                        _form.MaximizeBox = true;

                    Context.Device.Reset(Context.PresentParameters);

                    arcBall.SetWindow(_form.ClientSize.Width, _form.ClientSize.Height);
                    userInterfaceRenderer = new UserInterfaceRenderer9(Context.Device, _form.ClientSize.Width, _form.ClientSize.Height);
                }

                OnResourceLoad();
            }
            if (e.KeyCode == Keys.PrintScreen)
            {
                RenderScreenshot();
            }

            OnKeyUp(sender, e);
        }

        protected virtual void OnKeyUp(object sender, KeyEventArgs e) { }

        private bool isFullScreen = false;
        private void HandleResize(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            OnResourceUnload();

            arcBall.SetWindow(_form.ClientSize.Width, _form.ClientSize.Height);
            _configuration.WindowWidth = _form.ClientSize.Width;
            _configuration.WindowHeight = _form.ClientSize.Height;

            if (Context != null)
            {
                userInterfaceRenderer.Dispose();

                Context.PresentParameters.BackBufferWidth = _configuration.WindowWidth;
                Context.PresentParameters.BackBufferHeight = _configuration.WindowHeight;
                Context.Device.Reset(Context.PresentParameters);

                userInterfaceRenderer = new UserInterfaceRenderer9(Context.Device, _form.ClientSize.Width, _form.ClientSize.Height);
            }

            OnResourceLoad();
        }

        #endregion
    }
}