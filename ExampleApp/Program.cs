using SPTrApp.ExampleApp;
using SPTrApp.SPTrEngine;
using SPTrEngine;
using SPTrEngine.Math.Vector;

namespace SPTrApp
{
    internal static class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new Form1());

            //InternalCMD.MainArgsCMD(args);

            //Enemy enemy = new Enemy('E');
            //enemy.name += " | enemy";
            //enemy.position = new Vector2 { x = 3, y = 7 };

            //Player player = new Player('P');
            //player.name += " | player";
            //player.position = new Vector2 { x = 5, y = 4 };

            //BaseEngine.instance.EngineScreen.SetScreenSize(24,24);

            //BaseEngine.instance.Run();
        }
    }
}