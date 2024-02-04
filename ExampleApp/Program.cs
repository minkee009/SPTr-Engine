using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using SPTrEngine;
using SPTrEngine.Math.Vector;

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

            BaseEngine.Run();
        }
    }
}