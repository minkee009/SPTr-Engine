using System.Runtime.InteropServices;

namespace SPTrEngine
{
    public static class Input
    {
        /// <summary>
        /// 현재 입력의 비트마스크
        /// </summary>
        public static int CurrentInput => _currentInput;

        /// <summary>
        /// 과거 입력의 비트마스크
        /// </summary>
        public static int OldInput => _oldInput;

        static int _currentInput = 0;
        static int _oldInput = 0;

        /// <summary>
        /// WinAPI - user32.dll용 상수, 키가 눌러져 있는 경우
        /// </summary>
        const int KEY_PRESSED = 0x8000;

        /// <summary>
        /// 매개변수로 전달한 키값의 키가 눌러져 있는지 확인합니다.
        /// </summary>
        /// <param name="vKey">키의 고유번호</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool GetKey(ConsoleKey keyCode)
        {
            return (_currentInput & (1 << (int)keyCode)) != 0;
        }

        public static bool GetKeyUp(ConsoleKey keyCode)
        {
            return (_oldInput & (1 << (int)keyCode)) != 0
                && (_currentInput & (1 << (int)keyCode)) == 0;
        }

        public static bool GetKeyDown(ConsoleKey keyCode)
        {
            return (_oldInput & (1 << (int)keyCode)) == 0
                && (_currentInput & (1 << (int)keyCode)) != 0;
        }

        /// <summary>
        /// 인풋을 설정합니다
        /// </summary>
        public static void SetInput()
        {
            int nInput = 0;

            foreach(ConsoleKey key in Enum.GetValues(typeof(ConsoleKey)))
            {
                if((GetAsyncKeyState((int)key) & KEY_PRESSED) != 0)
                {
                    nInput |= 1 << (int)key;
                }
            }

            _oldInput = _currentInput;
            _currentInput = nInput;
        }
    }
}
