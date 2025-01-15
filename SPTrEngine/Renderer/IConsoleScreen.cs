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
}
