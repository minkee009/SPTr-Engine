using SPTrEngine;
using SPTrEngine.Math.Vector;

namespace SPTrApp
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            InternalCMD.MainArgsCMD(args);

            Enemy enemy = new Enemy('E');
            enemy.name += " | enemy";
            enemy.position = new Vector2 { x = 3, y = 7 };

            Player player = new Player('P');
            player.name += " | player";
            player.position = new Vector2 { x = 5, y = 4 };

            BaseEngine.instance.SetScreenSize(24, 24);

            BaseEngine.instance.Run();
        }
    }
}