using System.Diagnostics;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SPTrEngine.Math.Vector;

namespace SPTrEngine
{
    public class BaseEngine
    {
        public static List<GameObject> objects = new List<GameObject>();

        private static bool _isRunning = false;

        //윈도우(창) 및 게임 스크린
        private static IWindow _window;
        private static WindowOptions _wOptions = WindowOptions.Default with { Size = new Vector2D<int>(640, 480) };
        private static Vector2Int _screenSize;

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

            var silkInput = _window.CreateInput();

            while (_isRunning && !_window.IsClosing)
            {
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
            _window.SwapBuffers();

            _window.DoRender();
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

