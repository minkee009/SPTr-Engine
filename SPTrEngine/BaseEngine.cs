using System.Diagnostics;
using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using SPTrEngine.Math.Vector;


namespace SPTrEngine
{
    public static class BaseEngine
    {
        public static List<GameObject> objects = new List<GameObject>();

        public static string windowTitle = "SPTr Engine";

        private static bool _isRunning = false;

        //윈도우(창) 및 게임 스크린
        private static IWindow _window;
        private static WindowOptions _wOptions = WindowOptions.Default with { Size = new Vector2D<int>(640, 480) , Title = windowTitle };
        private static Vector2Int _screenSize;

        //렌더러
        private static GL _gl;

        //엔진 틱
        private static double _accumlator = 0;
        private static long _frameCount = 0;

        //프로퍼티
        public static bool IsRunning => _isRunning;
        public static long FrameCount => _frameCount;
        public static Vector2Int ScreenSize => _screenSize;

        public static void Run()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _isRunning = true;

            CreateWindow();

            StartOpenGL();

            //_window.Load += () =>
            //{
            //    _gl = GL.GetApi(_window);
            //};

            //_window.Render += (dt) =>
            //{
            //    _gl.ClearColor(Color.CornflowerBlue);
            //    _gl.Clear(ClearBufferMask.ColorBufferBit);
            //};

            //_window.Run();

            var silkInput = _window.CreateInput();

            while (_isRunning && !_window.IsClosing)
            {
                _window.Title = $"{windowTitle} - 엔진 실행 중 , 프레임 카운트 : {FrameCount}";

                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                _accumlator += Time.deltaTime;

                Input.SetInput(silkInput.Keyboards);

                //fixedTick
                while (_accumlator > 0.0)
                {
                    foreach (var obj in objects)
                    {
                        if (obj.Enabled)
                            obj.FixedTick();
                    }
                    _accumlator -= Time.fixedDeltaTime;
                }

                //tick
                foreach (var obj in objects)
                {
                    if(obj.Enabled)
                        obj.Tick();
                }

                //after tick
                foreach (var obj in objects)
                {
                    if (obj.Enabled)
                        obj.AfterTick();
                }

                
                //사운드 처리

                //화면 처리
                Render();

                _frameCount++;
            }

            CloseWindow();
        }

        

        public static void Render()
        {
            _window.DoEvents();
            _window.DoUpdate();

            _gl.ClearColor(Color.CornflowerBlue);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

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
        }

        public static void Dispose()
        {

        }

        private static void StartOpenGL()
        {
            _gl = GL.GetApi(_window);
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

