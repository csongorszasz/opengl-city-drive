using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace projekt
{
    class Program
    {
        private static IWindow window;
        private static GL Gl;
        private static IKeyboard primaryKeyboard;
        private static IMouse primaryMouse;
        private static ImGuiController guiController;
        
        private static GameManager _gameManager;
        private static ModelManager _modelManager;
        private static ShaderManager _shaderManager;
        
        // Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;
        private static bool isCursorVisible = false;

        private static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1280, 720);
            options.Title = "scim2304";
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;

            window.Run();

            window.Dispose();
        }

        private static void OnLoad()
        {
            IInputContext input = window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            primaryMouse = input.Mice.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }
            if (primaryMouse != null)
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Raw;
                primaryMouse.MouseMove += OnMouseMove;
                primaryMouse.Scroll += OnMouseWheel;
            }

            Gl = GL.GetApi(window);
            guiController = new ImGuiController(Gl, window, input);
            
            // Order matters here
            _shaderManager = new ShaderManager(Gl); // loads the fragment shaders
            _modelManager = new ModelManager(Gl); // loads the models
            _gameManager = new GameManager(Gl, window, primaryKeyboard, _modelManager, _shaderManager); // sets up the game session, holds session info
            // Drawer.Initialize(Gl, _shaderManager.LightingShader1);
        }
        
        private static unsafe void OnUpdate(double deltaTime)
        {
            if (_gameManager.IsSceneResetting)
                return;
            _gameManager.Update(deltaTime);
            guiController.Update((float)deltaTime);
        }
        
        private static unsafe void OnRender(double deltaTime)
        {
            // Gl.Enable(EnableCap.CullFace);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _gameManager.Render(deltaTime);
            RenderGui();
        }

        private static void RenderGui()
        {
            ImGuiNET.ImGui.Begin("Camera Views",
                ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);
            
            float buttonWidth = 200.0f;
            if (ImGuiNET.ImGui.Button("Third Person view", new Vector2(buttonWidth, 0)))
            {
                _gameManager.SetTPView();
            }
            if (ImGuiNET.ImGui.Button("Rear view", new Vector2(buttonWidth, 0)))
            {
                _gameManager.SetRearView();
            }
            if (ImGuiNET.ImGui.Button("Front Left Wheel view", new Vector2(buttonWidth, 0)))
            {
                _gameManager.SetFrontLeftWheelView();
            }
            if (ImGuiNET.ImGui.Button("Front Right Wheel view", new Vector2(buttonWidth, 0)))
            {
                _gameManager.SetFrontRightWheelView();
            }

            if (ImGuiNET.ImGui.Button("First Person view", new Vector2(buttonWidth, 0)))
            {
                _gameManager.SetFPView();
            }

            ImGuiNET.ImGui.End();
            guiController.Render();
        }
        
        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
            _gameManager.Camera.AspectRatio = (float)newSize.X / newSize.Y;
        }

        private static unsafe void OnMouseMove(IMouse mouse, Vector2 position)
        {
            if (_gameManager.IsCarBeingControlled || isCursorVisible)
            {
                LastMousePosition = position;
                return;
            }

            const float lookSensitivity = 0.1f;
            if (LastMousePosition == default)
            {
                LastMousePosition = position;
            }
            else
            {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                _gameManager.Camera.ModifyDirection(xOffset, yOffset, _gameManager);
            }
        }

        private static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            _gameManager.Camera.ModifyZoom(scrollWheel.Y);
        }

        private static void OnClose()
        {
            _shaderManager.Dispose();
            _modelManager.Dispose();
            _gameManager.Dispose();
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.C:
                    ChangeCameraView();
                    break;
                case Key.F:
                    StopDriving();
                    break;
                case Key.Escape:
                    ToggleCursorVisibility();
                    break;
            }
        }

        private static void ChangeCameraView()
        {
            _gameManager.Camera.NextCarView();
        }
        
        private static void StopDriving()
        {
            _gameManager.IsDriving = !_gameManager.IsDriving;
            _gameManager.Camera.Front = _gameManager.Car.Transform.Forward;
        }
        
        private static void ToggleCursorVisibility()
        {
            isCursorVisible = !isCursorVisible;
            if (primaryMouse != null)
            {
                primaryMouse.Cursor.CursorMode = isCursorVisible ? CursorMode.Normal : CursorMode.Raw;
            }
        }
    }
}
