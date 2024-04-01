using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using SPTrEngine.Math.Vector;
using static System.Net.WebRequestMethods;
using Microsoft.Win32.SafeHandles;
using System.Text;

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

        const int STD_OUTPUT_HANDLE = -11;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint CONSOLE_TEXTMODE_BUFFER = 1;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCursorInfo(IntPtr hConsoleOutput, [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateConsoleScreenBuffer(
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwFlags,
            IntPtr lpScreenBufferData
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD dwSize);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, [In] ref SMALL_RECT lpConsoleWindow);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FillConsoleOutputCharacter(IntPtr hConsoleOutput, char cCharacter, uint nLength, COORD dwWriteCoord, out uint lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD dwCursorPosition);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);


        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_CURSOR_INFO
        {
            public uint Size;
            public bool Visible;

            public CONSOLE_CURSOR_INFO(uint size, bool visible)
            {
                Size = size;
                Visible = visible;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;

            public SMALL_RECT(short left, short top, short right, short bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public ushort wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public uint Width;
            public uint Height;
        }

        private static int _screenIndex = 0;
        private static Rect _windowRect;
        private static IntPtr[] _screenBuffer = new IntPtr[2];


        BaseEngine(int screenSizeW = 10, int screenSizeH = 10)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            _screenSize = new Vector2Int(screenSizeW, screenSizeH);
            _screen = new char[_screenSize.y, _screenSize.x];
            _clearedScreen = new char[_screenSize.y, _screenSize.x];

            _screenText = new char[screenSizeW * screenSizeH * 4];
            //SetScreenSize( screenSizeW, screenSizeH );
        }

        public void DestroyConsoleHandle(object? sender, EventArgs e)
        {
            for(int i = 0; i < 2; i++)
            {
                if(_screenBuffer[i] != IntPtr.Zero)
                    CloseHandle(_screenBuffer[i]);
            }
        }

        public void Run()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            CONSOLE_CURSOR_INFO cci = new CONSOLE_CURSOR_INFO(1, false);

            CONSOLE_SCREEN_BUFFER_INFO consoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();

            _screenBuffer[0] = new IntPtr();
            _screenBuffer[1] = new IntPtr();

            GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), out consoleInfo);

            consoleInfo.dwSize.X = (short)ScreenSize.x;
            consoleInfo.dwSize.Y = (short)ScreenSize.y;

            _windowRect.Width = (uint)(consoleInfo.srWindow.Right - consoleInfo.srWindow.Left);


            for (int i = 0; i < 2; i++)
            {
                _screenBuffer[i] = CreateConsoleScreenBuffer(
                    GENERIC_READ | GENERIC_WRITE,
                    0,
                    IntPtr.Zero,
                    CONSOLE_TEXTMODE_BUFFER,
                    IntPtr.Zero);

                SetConsoleScreenBufferSize(_screenBuffer[i], consoleInfo.dwSize);
                SetConsoleWindowInfo(_screenBuffer[i], true, ref consoleInfo.srWindow);
                SetConsoleCursorInfo(_screenBuffer[i], ref cci);
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
            SetConsoleCursorPosition(_screenBuffer[_screenIndex],cursorPos);
            DrawConsole();
            byte[] buffer = Encoding.UTF8.GetBytes(_screenText);

            WriteFile(_screenBuffer[_screenIndex], buffer, (uint)buffer.Length, out dw, IntPtr.Zero);
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
        }

        public void SwapScreen()
        {
            SetConsoleActiveScreenBuffer(_screenBuffer[_screenIndex]);
            _screenIndex = _screenIndex == 0 ? 1 : 0;
        }

        public void ClearScreen()
        {
            COORD pos = new COORD(0,0);
            uint dw;
            uint length = _windowRect.Height * _windowRect.Width;
            FillConsoleOutputCharacter(_screenBuffer[_screenIndex], ' ', length, pos, out dw);
            SetConsoleCursorPosition(_screenBuffer[_screenIndex], pos);
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
            _screenText = new char[_screenSize.x * _screenSize.y * 4];

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

