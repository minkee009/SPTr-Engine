using SPTrEngine.Extensions.Kernel32;
using SPTrEngine.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTrEngine
{
    public interface IConsoleScreen
    {
        public char[,] Screen { get; }
        public char[] ScreenText { get; }

        public Vector2Int ScreenSize { get; }

        public IConsoleScreen SetScreenSize(int wSize, int hSize);
    }

    public class ConsoleRenderer : IConsoleScreen, IDisposable
    {
        private long _frameCount = 0;

        private static int _screenIndex = 0;
        private static Rect _windowRect;
        private static IntPtr[] _screenBuffer = new IntPtr[2];

        private char[,] _screen;
        private char[,] _clearedScreen;
        private char[] _screenText;
        private bool _disposed;

        private Vector2Int _screenSize;

        public long FrameCount => _frameCount;

        public char[,] Screen => _screen;

        public char[] ScreenText => _screenText;

        public Vector2Int ScreenSize => _screenSize;

#pragma warning disable CS8618
        public ConsoleRenderer()
        {
            SetScreenSize(10, 10);
            Console.OutputEncoding = Encoding.UTF8;
        }
#pragma warning restore CS8618

        public IConsoleScreen SetScreenSize(int wSize, int hSize)
        {
            _screenSize = new Vector2Int(wSize, hSize);
            _screen = new char[_screenSize.y, _screenSize.x];
            _clearedScreen = new char[_screenSize.y, _screenSize.x];
            _screenText = new char[wSize * hSize * 2 + hSize];

            for (int i = _screenSize.y - 1; i > -1; i--)
            {
                for (int j = 0; j < _screenSize.x; j++)
                {
                    _clearedScreen[i, j] = '.';
                }
            }

            return this;
        }

        ~ConsoleRenderer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                _disposed = true;
                DestroyScreenHandle(null, EventArgs.Empty);

                if(disposing)
                    GC.SuppressFinalize(this);
            }
        }

        public void CreateScreenHandle()
        {
            CONSOLE_CURSOR_INFO cci = new CONSOLE_CURSOR_INFO(1, false);

            CONSOLE_SCREEN_BUFFER_INFO consoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();

            Kernel32.GetConsoleScreenBufferInfo(Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE), out consoleInfo);

            consoleInfo.dwSize.X = (short)_screenSize.x;
            consoleInfo.dwSize.Y = (short)_screenSize.y;

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

            AppDomain.CurrentDomain.ProcessExit += DestroyScreenHandle;
        }

        public void DestroyScreenHandle(object? sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                if (_screenBuffer[i] != IntPtr.Zero)
                    Kernel32.CloseHandle(_screenBuffer[i]);
            }
        }

        public void Render(List<GameObject> objects)
        {
            ClearScreen();

            uint dw = 0;
            COORD cursorPos = new COORD(0, 0);
            Kernel32.SetConsoleCursorPosition(_screenBuffer[_screenIndex], cursorPos);
            DrawConsole(objects);
            byte[] buffer = Encoding.UTF8.GetBytes(_screenText);

            Kernel32.WriteFile(_screenBuffer[_screenIndex], buffer, (uint)buffer.Length, out dw, IntPtr.Zero);
            SwapScreen();

            _frameCount++;
        }

        private void SwapScreen()
        {
            Kernel32.SetConsoleActiveScreenBuffer(_screenBuffer[_screenIndex]);
            _screenIndex = _screenIndex == 0 ? 1 : 0;
        }

        private void ClearScreen()
        {
            COORD pos = new COORD(0, 0);
            uint dw;
            uint length = _windowRect.Height * _windowRect.Width;
            Kernel32.FillConsoleOutputCharacter(_screenBuffer[_screenIndex], ' ', length, pos, out dw);
            Kernel32.SetConsoleCursorPosition(_screenBuffer[_screenIndex], pos);
        }

        private void DrawConsole(List<GameObject> objects)
        {
            //화면 초기화
            Array.Copy(_clearedScreen, _screen, _clearedScreen.Length);

            //렌더링 오브젝트 정보입력 (추후에 Sorting Order 추가하기)
            foreach (var obj in objects)
            {
                var posToInt = new Vector2Int((int)obj.Transform.Position.x,(int)obj.Transform.Position.y);

                if (obj.TryGetComponent(out Mesh? m) 
                    && (m?.MeshSet ?? '.') != '.'
                    && posToInt.x <= _screenSize.x && posToInt.x > 0
                    && posToInt.y <= _screenSize.y && posToInt.y > 0)
                {
                    _screen[posToInt.y - 1, posToInt.x - 1] = m?.MeshSet ?? '.';
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
    }
}
