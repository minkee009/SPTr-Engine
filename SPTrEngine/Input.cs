using System;
using System.Runtime.InteropServices;

namespace SPTrEngine
{
    public static class Input
    {
        /// <summary>
        /// 현재 입력의 비트마스크
        /// </summary>
        public static uint[] CurrentInput => _currentInput;

        /// <summary>
        /// 과거 입력의 비트마스크
        /// </summary>
        public static uint[] OldInput => _oldInput;

        static uint[] _currentInput = new uint[8];
        static uint[] _oldInput = new uint[8];

        /// <summary>
        /// WinAPI - user32.dll용 상수, 키가 눌러져 있는 경우
        /// </summary>
        const int KEY_PRESSED = 0x8000;
        const uint UNSINGED_ONE = 1;

        /// <summary>
        /// 매개변수로 전달받은 키값의 상태를 확인합니다.
        /// </summary>
        /// <param name="vKey">키의 고유번호</param>
        /// <returns>키의 상태</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool GetKey(ConsoleKey keyCode)
        {
            var i = (int)keyCode / 32;

            return (_currentInput[i] & (UNSINGED_ONE << (int)keyCode % 32)) != 0;
        }

        public static bool GetKeyUp(ConsoleKey keyCode)
        {
            var i = (int)keyCode / 32;
            return (_oldInput[i] & (UNSINGED_ONE << (int)keyCode) % 32) != 0
                && (_currentInput[i] & (UNSINGED_ONE << (int)keyCode) % 32) == 0;
        }

        public static bool GetKeyDown(ConsoleKey keyCode)
        {
            var i = (int)keyCode / 32;
            return (_oldInput[i] & (UNSINGED_ONE << (int)keyCode % 32)) == 0
                && (_currentInput[i] & (UNSINGED_ONE << (int)keyCode % 32)) != 0;
        }

        /// <summary>
        /// 입력을 탐지합니다
        /// </summary>
        public static void ScanInput()
        {
            Array.Copy(_currentInput,_oldInput,_currentInput.Length);
            Array.Clear(_currentInput, 0, _currentInput.Length);

            foreach(ConsoleKey key in Enum.GetValues(typeof(ConsoleKey)))
            {
                if((GetAsyncKeyState((int)key) & KEY_PRESSED) != 0)
                {
                    var i = (int)key / 32;
                    _currentInput[i] |= UNSINGED_ONE << (int)key % 32;
                }
            }
        }
    }
}
