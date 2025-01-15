using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SPTrEngine.Extensions.Kernel32
{
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

    public static class Kernel32
    {
        public const int STD_OUTPUT_HANDLE = -11;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint CONSOLE_TEXTMODE_BUFFER = 1;

        [DllImport("Kernel32.dll")]
        public static extern bool Beep(uint frequency, uint duration);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCursorInfo(IntPtr hConsoleOutput, [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateConsoleScreenBuffer(
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwFlags,
            IntPtr lpScreenBufferData
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD dwSize);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, [In] ref SMALL_RECT lpConsoleWindow);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FillConsoleOutputCharacter(IntPtr hConsoleOutput, char cCharacter, uint nLength, COORD dwWriteCoord, out uint lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD dwCursorPosition);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

    }
}
