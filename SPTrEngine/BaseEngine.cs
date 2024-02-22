using System;
using System.Diagnostics;
using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using SPTrEngine.Math;

namespace SPTrEngine
{
    public enum EngineState
    {
        CheckInput = 0,
        FixedTick = 1,
        Tick = 2,
        AfterTick = 3,
        Render = 4,
    }

    public static class BaseEngine
    {
        public static List<GameObject> objects = new List<GameObject>();

        public static string windowTitle = "SPTr Engine";

        private static bool _isRunning = false;

        private static EngineState _state;

        //윈도우(창) 및 게임 스크린
        public static bool VSync
        {
            get => _window.VSync; 
            set => _window.VSync = value;
        }

        private static IWindow _window;
        private static WindowOptions _wOptions = WindowOptions.Default with { Size = new Vector2D<int>(640, 480) , Title = windowTitle };
        private static Vector2Int _screenSize;

        //렌더러
        private static GL _gl;
        private static uint _vao;
        private static uint _vbo;
        private static uint _ebo;
        private static uint _program;

        //엔진 틱
        private static double _accumlator = 0;
        private static long _frameCount = 0;
        private static int _frameLimit = 30;

        //엔진 컨텍스트 
        private static IInputContext _inputContext;

        //프로퍼티
        public static bool IsRunning => _isRunning;
        public static long FrameCount => _frameCount;
        public static Vector2Int ScreenSize => _screenSize;

        public static EngineState State => _state;

        public static void Run()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _isRunning = true;

            CreateWindow();

            StartOpenGL();

            VSync = true;

            _inputContext = _window.CreateInput();

            var fps = 0.0f;

            while (_isRunning)
            {
                _window.Title = $"{windowTitle} - 엔진 실행 중 , 프레임 카운트 : {FrameCount} , {MathF.Round(fps,2)}fps";
                

                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                _accumlator += Time.deltaTime;

                if (!VSync)
                {
                    double waitTime = (1 / (double)_frameLimit) - Time.deltaTime;

                    waitTime = (int)(waitTime * 1000);

                    if(waitTime > 0)
                    {
                        Thread.Sleep((int)waitTime);
                    }
                }

                _state = EngineState.CheckInput;
                Input.SetInput(_inputContext.Keyboards);

                //fixedTick
                _state = EngineState.FixedTick;
                while (_accumlator > 0.0)
                {
                    foreach (var obj in objects)
                    {
                        if (obj.Enabled)
                        {
                            obj.FixedTick();
                            obj.CheckYield();
                        }
                    }
                    _accumlator -= Time.fixedDeltaTime;

                    fps = 1.0f / (float)Time.deltaTime;
                }

                //tick
                _state = EngineState.Tick;
                foreach (var obj in objects)
                {
                    if(obj.Enabled)
                    {
                        obj.Tick();
                        obj.CheckYield();
                    }
                }

                //after tick
                _state = EngineState.AfterTick;
                foreach (var obj in objects)
                {
                    if (obj.Enabled)
                    {
                        obj.AfterTick();
                        obj.CheckYield();
                    }
                }

                //사운드 처리

                //화면 처리
                _state = EngineState.Render;
                Render();
                foreach (var obj in objects)
                {
                    if (obj.Enabled)
                    {
                        obj.CheckYield();
                    }
                }

                _frameCount++;
            }

            CloseWindow();
        }

        

        public static unsafe void Render()
        {
            _window.DoEvents();
            _window.DoUpdate();

            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _gl.BindVertexArray(_vao);
            _gl.UseProgram(_program);
            _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);

            _window.DoRender();

            //_window.SwapBuffers();
        }

        public static void SetScreenSize(int x, int y)
        {
            _screenSize = new Vector2Int(x, y);
            _wOptions.Size = new Vector2D<int>(x, y);
        }

        public static void Exit()
        {
            _isRunning = false;
            _inputContext?.Dispose();
            _gl?.Dispose();
        }

        public static void Dispose()
        {

        }

        private static unsafe void StartOpenGL()
        {
            _gl = _window.CreateOpenGL();
            _gl.ClearColor(Color.CornflowerBlue);
            InitOpenGLResource();
        }

        private static unsafe void InitOpenGLResource()
        {
            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

            float[] vertices =
            {
                0.5f,  0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
               -0.5f, -0.5f, 0.0f,
               -0.5f,  0.5f, 0.0f
            };

            fixed (float* buf = vertices)
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

            uint[] indices =
            {
                0u, 1u, 3u,
                1u, 2u, 3u
            };

            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

            fixed (uint* buf = indices)
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

            const string vertexCode = @"
#version 330 core

layout (location = 0) in vec3 aPosition;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
}";

            const string fragmentCode = @"
#version 330 core

out vec4 out_color;

void main()
{
    out_color = vec4(1.0, 0.5, 0.2, 1.0);
}";

            uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertexShader, vertexCode);

            _gl.CompileShader(vertexShader);

            _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + _gl.GetShaderInfoLog(vertexShader));

            uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragmentShader, fragmentCode);

            _gl.CompileShader(fragmentShader);

            _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + _gl.GetShaderInfoLog(fragmentShader));

            _program = _gl.CreateProgram();
            _gl.AttachShader(_program, vertexShader);
            _gl.AttachShader(_program, fragmentShader);

            _gl.LinkProgram(_program);

            _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + _gl.GetProgramInfoLog(_program));

            _gl.DetachShader(_program, vertexShader);
            _gl.DetachShader(_program, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            const uint positionLoc = 0;
            _gl.EnableVertexAttribArray(positionLoc);
            _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

            _gl.BindVertexArray(0);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        }

        private static void CreateWindow()
        {
            if (_window == null)
            {
                _window = Window.Create(_wOptions);
                _window.Closing += Exit;
                _window.Initialize();
            }
        }

        private static void CloseWindow()
        {
            _window.Close();
        }

    }
}

