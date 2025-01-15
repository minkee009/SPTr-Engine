using System;
using System.Diagnostics;
using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
using System.Text;
using SPTrEngine.Extensions.Kernel32;
using Microsoft.Win32.SafeHandles;
using System.Security.Policy;

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

    public class BaseEngine
    {
        public static BaseEngine instance = new BaseEngine();
        public IReadOnlyList<GameObject> Objects => _objects;
        private List<GameObject> _objects = new List<GameObject>();

        private EngineState _state;
        private bool _isExit;
        //private ConsoleRenderer _consoleRenderer;
        private Queue<Action> _objCountManager;
        private Dictionary<string, string> _hashs;

        public string windowTitle = "SPTr Engine";

                //윈도우(창) 및 게임 스크린
        public bool VSync
        {
            get => _window.VSync; 
            set => _window.VSync = value;
	    }

        //public IConsoleScreen EngineScreen => _consoleRenderer;

        public EngineState State => _state;

        BaseEngine()
        {
            _state = EngineState.CheckInput;
            _isExit = false;
            //_consoleRenderer = new ConsoleRenderer();
            _objCountManager = new Queue<Action>();
            _hashs = new Dictionary<string, string>();
        }

        private IWindow _window;
        private WindowOptions _wOptions = WindowOptions.Default with { Size = new Vector2D<int>(640, 480) , Title = "SPTr Engine" };

        //렌더러
        private GL _gl;
        private uint _vao;
        private uint _vbo;
        private uint _ebo;
        private uint _program;

        //엔진 틱
        private double _accumlator = 0;
        private long _frameCount = 0;
        private int _frameLimit = 120;

        //엔진 컨텍스트 
        private IInputContext _inputContext;

        //프로퍼티
        public bool IsRunning => !_isExit;
        public long FrameCount => _frameCount;

        public void Run()
        {
            Kernel32.Beep(200, 55);
            Kernel32.Beep(350, 30);
            Kernel32.Beep(240, 55);
            Stopwatch stopwatch = Stopwatch.StartNew();

            //_consoleRenderer.CreateScreenHandle();

            double accumlator = 0;

            CreateWindow();

            StartOpenGL();

            VSync = true;

            _inputContext = _window.CreateInput();

            var fps = 0.0f;

            while (!_isExit)
            {
                _window.Title = $"{windowTitle} - 엔진 실행 중 , 프레임 카운트 : {FrameCount} , {MathF.Round(fps,2)}fps";
                

                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                accumlator += Time.deltaTime;

                if (!VSync && _frameLimit > 0)
                {
                    double waitTime = (1 / (double)_frameLimit) - Time.deltaTime;

                    waitTime *= 1000;

                    if(waitTime > 0)
                    {
                        Thread.Sleep((int)waitTime);
                    }
                }
                
                //오브젝트 등록관리
                while (_objCountManager.Count > 0)
                {
                    _objCountManager.Dequeue().Invoke();
                }

                _state = EngineState.CheckInput;
                Input.ScanInput(_inputContext.Keyboards);

                //fixedTick
                _state = EngineState.FixedTick;
                while (accumlator > 0.0)
                {
                    foreach (var obj in _objects)
                    {
                        if (obj.Enabled)
                        {
                            foreach (var com in obj.Components)
                            {
                                var script = com as ScriptBehavior;

                                if (script?.Enabled ?? false)
                                {
                                    script.FixedTick();
                                    script.CheckYield();
                                }
                            }
                        }

                    }
                    accumlator -= Time.fixedDeltaTime;
                    fps = 1.0f / (float)Time.deltaTime;
                }

                //window polling event / update
                _window.DoEvents();
                _window.DoUpdate();

                //tick
                _state = EngineState.Tick;
                foreach (var obj in _objects)
                {
                    if (obj.Enabled)
                    {
                        foreach (var com in obj.Components)
                        {
                            var script = com as ScriptBehavior;

                            if (script?.Enabled ?? false)
                            {
                                script.Tick();
                                script.CheckYield();
                            }
                        }
                    }
                }

                //after tick
                _state = EngineState.AfterTick;
                foreach (var obj in _objects)
                {
                    if (obj.Enabled)
                    {
                        foreach (var com in obj.Components)
                        {
                            var script = com as ScriptBehavior;

                            if (script?.Enabled ?? false)
                            {
                                script.AfterTick();
                                script.CheckYield();
                            }
                        }
                    }
                }

                //사운드 처리

                //화면 처리
                _state = EngineState.Render;
                Render();
                //_consoleRenderer.Render(_objects);
                foreach (var obj in _objects)
                {
                    if (obj.Enabled)
                    {
                        foreach (var com in obj.Components)
                        {
                            var script = com as ScriptBehavior;

                            if (script?.Enabled ?? false)
                            {
                                script.CheckYield();
                            }
                        }
                    }
                }

                _frameCount++;
            }

            CloseWindow();
        }

        

        public unsafe void Render()
        {
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _gl.BindVertexArray(_vao);
            _gl.UseProgram(_program);
            _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);

            _window.DoRender();

            //_window.SwapBuffers();
        }

        public void SetScreenSize(int x, int y)
        {
            _wOptions.Size = new Vector2D<int>(x, y);
        }

        public void Exit()
        {
            _isExit = true;
            _inputContext?.Dispose();
            _gl?.Dispose();
        }

        public void Dispose()
        {

        }

        private unsafe void StartOpenGL()
        {
            _gl = _window.CreateOpenGL();
            _gl.ClearColor(Color.CornflowerBlue);
            InitOpenGLResource();
        }

        private unsafe void InitOpenGLResource()
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

        private void CreateWindow()
        {
            if (_window == null)
            {
                _window = Window.Create(_wOptions);
                _window.Closing += Exit;
                _window.Initialize();
            }
        }

        private void CloseWindow()
        {
            _window.Close();
        }

        public void RegisterGameObject(GameObject obj)
        {
            if (_hashs.ContainsKey(obj.Hash))
                return;

            _hashs.Add(obj.Hash, obj.name);
            _objCountManager.Enqueue(() => _objects.Add(obj));
        }

        public void UnregisterGameObject(GameObject obj) 
        {
            if (!_hashs.ContainsKey(obj.Hash))
                return;

            _hashs.Remove(obj.Hash);
            _objCountManager.Enqueue(() => _objects.Remove(obj));
        }
    }
}

