using System.Runtime.InteropServices;
using Silk.NET.Input;

namespace SPTrEngine
{
    public enum KeyCode
    {
        None = 0,
        A = Key.A,
        B = Key.B,
        C = Key.C,
        D = Key.D,
        E = Key.E,
        F = Key.F,
        G = Key.G,
        H = Key.H,
        I = Key.I,
        J = Key.J,
        K = Key.K,
        L = Key.L,
        M = Key.M,
        N = Key.N,
        O = Key.O,
        P = Key.P,
        Q = Key.Q,
        R = Key.R,
        S = Key.S,
        T = Key.T,
        U = Key.U,
        V = Key.V,
        W = Key.W,
        X = Key.X,
        Y = Key.Y,
        Z = Key.Z,
        Num0 = Key.D0,
        Num1 = Key.Number1,
        Num2 = Key.Number2,
        Num3 = Key.Number3,
        Num4 = Key.Number4,
        Num5 = Key.Number5,
        Num6 = Key.Number6,
        Num7 = Key.Number7,
        Num8 = Key.Number8,
        Num9 = Key.Number9,
        /// <summary>
        /// 억음 키 ( ` , ~ )
        /// </summary>
        BackQuote = Key.GraveAccent,
        Minus = Key.Minus,
        Equal = Key.Equal,
        BackSpace = Key.Backspace,
        LeftBracket = Key.LeftBracket,
        RightBracket = Key.RightBracket,
        BackSlash = Key.BackSlash,
        Semicolon = Key.Semicolon,
        /// <summary>
        /// 인용 키 ( ' , " )
        /// </summary>
        Apostrophe = Key.Apostrophe,
        Comma = Key.Comma,
        Period = Key.Period,
        Slash = Key.Slash,
        Space = Key.Space,
        LeftShift = Key.ShiftLeft,
        RightShift = Key.ShiftRight,
        LeftControl = Key.ControlLeft,
        RightControl = Key.ControlRight,
        LeftAlt = Key.AltLeft,
        RightAlt = Key.AltRight,
        CapsLock = Key.CapsLock,
        Enter = Key.Enter,
        F1 = Key.F1,
        F2 = Key.F2,
        F3 = Key.F3,
        F4 = Key.F4,
        F5 = Key.F5,
        F6 = Key.F6,
        F7 = Key.F7,
        F8 = Key.F8,
        F9 = Key.F9,
        F10 = Key.F10,
        F11 = Key.F11,
        F12 = Key.F12,
        Print = Key.PrintScreen,
        ScrollLock = Key.ScrollLock,
        Pause = Key.Pause,
        Insert = Key.Insert,
        Home = Key.Home,
        PageUp = Key.PageUp,
        PageDown = Key.PageDown,
        Delete = Key.Delete,
        End = Key.End,
        Tab = Key.Tab,
        Esc = Key.Escape,

        UpArrow = Key.Up,
        DownArrow = Key.Down,
        LeftArrow = Key.Left,
        RightArrow = Key.Right,
    }

    public static class Input
    {
        static int _currentInput = 0;
        static int _oldInput = 0;

        public static bool GetKey(KeyCode key)
        {
            return (_currentInput & (1 << (int)key)) != 0;
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return (_oldInput & (1 << (int)key)) != 0
                && (_currentInput & (1 << (int)key)) == 0;
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return (_oldInput & (1 << (int)key)) == 0
                && (_currentInput & (1 << (int)key)) != 0;
        }

        /// <summary>
        /// 인풋을 설정합니다
        /// </summary>
        public static void SetInput(IReadOnlyList<IKeyboard> keyboards)
        {
            int nInput = 0;

            foreach (IKeyboard keyboard in keyboards)
            {
                foreach (Key key in Enum.GetValues(typeof(Key)))
                {
                    if ((int)key == -1)
                        continue;

                     if (keyboard.IsKeyPressed(key))
                    {
                        nInput |= 1 << (int)key;
                    }
                }
            }

            _oldInput = _currentInput;
            _currentInput = nInput;
        }
    }
}
