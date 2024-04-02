using System.Diagnostics;
using System.Runtime.InteropServices;
using SPTrEngine.Math.Vector;
using System.Text;
using SPTrEngine.Extensions.Kernel32;

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
        public static List<GameObject> objects = new List<GameObject>();

        public long FrameCount => _frameCount;

        public Vector2Int ScreenSize => _screenSize;
        public char[,] Screen => _screen;
        public char[] ScreenText => _screenText;

        public EngineState State => _state;

        private EngineState _state;

        private bool _isExit = false;

        private char[,] _screen;
        private char[,] _clearedScreen;
        private char[] _screenText;
        private Vector2Int _screenSize;
        private string _lastScreenString = "";
        private double _accumlator = 0;

        private long _frameCount = 0;

        

        private static int _screenIndex = 0;
        private static Rect _windowRect;
        private static IntPtr[] _screenBuffer = new IntPtr[2];


        BaseEngine(int screenSizeW = 10, int screenSizeH = 10)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            _screenSize = new Vector2Int(screenSizeW, screenSizeH);
            _screen = new char[_screenSize.y, _screenSize.x];
            _clearedScreen = new char[_screenSize.y, _screenSize.x];

            _screenText = new char[screenSizeW * screenSizeH * 2 + screenSizeH];
            //SetScreenSize( screenSizeW, screenSizeH );
        }

        public void DestroyConsoleHandle(object? sender, EventArgs e)
        {
            for(int i = 0; i < 2; i++)
            {
                if(_screenBuffer[i] != IntPtr.Zero)
                    Kernel32.CloseHandle(_screenBuffer[i]);
            }
        }

        public void Run()
        {
            Kernel32.Beep(200, 55);
            Kernel32.Beep(350, 30);
            Kernel32.Beep(240, 55);
            Stopwatch stopwatch = Stopwatch.StartNew();

            CONSOLE_CURSOR_INFO cci = new CONSOLE_CURSOR_INFO(1, false);

            CONSOLE_SCREEN_BUFFER_INFO consoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();

            Kernel32.GetConsoleScreenBufferInfo(Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE), out consoleInfo);

            consoleInfo.dwSize.X = (short)ScreenSize.x;
            consoleInfo.dwSize.Y = (short)ScreenSize.y;

            _windowRect.Width = (uint)(consoleInfo.srWindow.Right - consoleInfo.srWindow.Left);

            for (int i = 0; i < 2; i++)
            {
                _screenBuffer[i] = Kernel32.CreateConsoleScreenBuffer(
                    Kernel32.GENERIC_READ | Kernel32.GENERIC_WRITE,
                    0,
                    IntPtr.Zero,
                    Kernel32.CONSOLE_TEXTMODE_BUFFER,
                    IntPtr.Zero);

                Kernel32.SetConsoleScreenBufferSize(_screenBuffer[i], consoleInfo.dwSize);
                Kernel32.SetConsoleWindowInfo(_screenBuffer[i], true, ref consoleInfo.srWindow);
                Kernel32.SetConsoleCursorInfo(_screenBuffer[i], ref cci);
            }

            AppDomain.CurrentDomain.ProcessExit += DestroyConsoleHandle;

            while (!_isExit)
            {
                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                _accumlator += Time.deltaTime;

                _state = EngineState.CheckInput;
                Input.SetInput();

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
                }

                //tick
                _state = EngineState.Tick;
                foreach (var obj in objects)
                {
                    if (obj.Enabled)
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
        }

        public void Render()
        {
            ClearScreen();

            uint dw = 0;
            COORD cursorPos = new COORD(0, 0);
            Kernel32.SetConsoleCursorPosition(_screenBuffer[_screenIndex],cursorPos);
            DrawConsole();
            byte[] buffer = Encoding.UTF8.GetBytes(_screenText);

            Kernel32.WriteFile(_screenBuffer[_screenIndex], buffer, (uint)buffer.Length, out dw, IntPtr.Zero);
            SwapScreen();
        }

        public void DrawConsole()
        {
            //화면 초기화
            Array.Copy(_clearedScreen, _screen, _clearedScreen.Length);

            //렌더링 오브젝트 정보입력 (추후에 Sorting Order 추가하기)
            foreach (var obj in objects)
            {
                var posToInt = obj.position.ToVector2Int();
                if (obj.Mesh != '.'
                    && posToInt.x < ScreenSize.x && posToInt.x >= 0
                    && posToInt.y < ScreenSize.y && posToInt.y >= 0)
                {
                    _screen[posToInt.y, posToInt.x] = obj.Mesh;
                }
            }

            //var finalString = new char[ScreenSize.x * ScreenSize.y * 4];
            var currentIDX = 0;

            for (int i = _screenSize.y - 1; i > -1; i--)
            {
                for (int j = 0; j < _screenSize.x; j++)
                {
                    _screenText[currentIDX++] = _screen[i, j];
                    _screenText[currentIDX++] = ' ';

                    if (j == _screenSize.x - 1)
                        _screenText[currentIDX++] = '\n';
                }
            }

            var gg = currentIDX;
        }

        public void SwapScreen()
        {
            Kernel32.SetConsoleActiveScreenBuffer(_screenBuffer[_screenIndex]);
            _screenIndex = _screenIndex == 0 ? 1 : 0;
        }

        public void ClearScreen()
        {
            COORD pos = new COORD(0,0);
            uint dw;
            uint length = _windowRect.Height * _windowRect.Width;
            Kernel32.FillConsoleOutputCharacter(_screenBuffer[_screenIndex], ' ', length, pos, out dw);
            Kernel32.SetConsoleCursorPosition(_screenBuffer[_screenIndex], pos);
        }

        public void OldRender()
        {
            //화면 초기화
            Array.Copy(_clearedScreen,_screen,_clearedScreen.Length);

            //렌더링 오브젝트 정보입력 (추후에 Sorting Order 추가하기)
            foreach (var obj in objects)
            {
                var posToInt = obj.position.ToVector2Int();
                if (obj.Mesh != '.' 
                    && posToInt.x < ScreenSize.x && posToInt.x >= 0
                    && posToInt.y < ScreenSize.y && posToInt.y >= 0)
                {
                    _screen[posToInt.y,posToInt.x] = obj.Mesh;
                }
            }

            //최종 출력
            var finalString = "";
            for (int i = _screenSize.y - 1; i > -1; i--)
            {
                for (int j = 0; j < _screenSize.x; j++)
                {
                    finalString += _screen[i, j] + " ";
                    if (j == _screenSize.x - 1)
                        finalString += "\n";
                }
            }

            if (finalString != _lastScreenString)
            {
                Console.Clear();
                Console.WriteLine(finalString);
            }

            _lastScreenString = finalString;
        }

        public void SetScreenSize(int x, int y)
        {
            _screenSize = new Vector2Int(x, y);
            _screen = new char[_screenSize.y, _screenSize.x];
            _clearedScreen = new char[_screenSize.y, _screenSize.x];
            _screenText = new char[_screenSize.x * _screenSize.y * 2 + ScreenSize.y];

            for (int i = _screenSize.y - 1; i > -1; i--)
            {
                for (int j = 0; j < _screenSize.x; j++)
                {
                    _clearedScreen[i, j] = '.';
                }
            }
        }

        public void Exit()
        {
            _isExit = true;
        }

        public void Dispose()
        {

        }
    }
}

