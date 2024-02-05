using SPTrEngine;
using SPTrEngine.Math;

namespace SPTrApp
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            Enemy enemy = new Enemy('E');
            enemy.name += " | enemy";
            enemy.position = new Vector2 { x = 3, y = 7 };

            Player player = new Player('P');
            player.name += " | player";
            player.position = new Vector2 { x = 5, y = 4 };
            player.tag = "P";

            BaseEngine.Run();
        }
    }
}