using System.Runtime.InteropServices;

namespace SPTrEngine
{
    public static class Input
    {
        /// <summary>
        /// 현재 입력의 비트마스크
        /// </summary>
        public static ulong[] CurrentInput => _currentInput;

        /// <summary>
        /// 과거 입력의 비트마스크
        /// </summary>
        public static ulong[] OldInput => _oldInput;

        static ulong[] _currentInput = new ulong[4];
        static ulong[] _oldInput = new ulong[4];

        /// <summary>
        /// WinAPI - user32.dll용 상수, 키가 눌러져 있는 경우
        /// </summary>
        const int KEY_PRESSED = 0x8000;

        /// <summary>
        /// 매개변수로 전달한 키값의 상태를 확인합니다.
        /// </summary>
        /// <param name="vKey">키의 고유번호</param>
        /// <returns>키의 상태</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool GetKey(ConsoleKey keyCode)
        {
            var i = (int)keyCode / 64;
            return (_currentInput[i] & ((ulong)1 << (int)keyCode)) != 0;
        }

        public static bool GetKeyUp(ConsoleKey keyCode)
        {
            var i = (int)keyCode / 64;
            return (_oldInput[i] & ((ulong)1 << (int)keyCode)) != 0
                && (_currentInput[i] & ((ulong)1 << (int)keyCode)) == 0;
        }

        public static bool GetKeyDown(ConsoleKey keyCode)
        {
            var i = (int)keyCode / 64;
            return (_oldInput[i] & ((ulong)1 << (int)keyCode)) == 0
                && (_currentInput[i] & ((ulong)1 << (int)keyCode)) != 0;
        }

        /// <summary>
        /// 인풋을 설정합니다
        /// </summary>
        public static void SetInput()
        {
            ulong[] nInput = { 0,0,0,0 };

            foreach(ConsoleKey key in Enum.GetValues(typeof(ConsoleKey)))
            {
                if((GetAsyncKeyState((int)key) & KEY_PRESSED) != 0)
                {
                    var i = (int)key / 64;
                    nInput[i] |= (ulong)1 << (int)key % 64;
                }
            }

            //foreach(var input in nInput)
            //{
            //    Console.Write($" {Convert.ToString((long)input, toBase: 2)}");
            //}

            //Console.WriteLine();

            _oldInput = _currentInput;
            _currentInput = nInput;
        }
    }
}
