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
            InternalCMD.MainArgsCMD(args);

            GameObject player = new GameObject("Player");
            player.AddComponent<Player>();
            player.AddComponent<Mesh>().MeshSet = 'P';
            player.Transform.Position = new Vector3 { x = 5, y =4 };


            GameObject enemy = new GameObject("Enemy");
            enemy.AddComponent<Enemy>();
            enemy.AddComponent<Mesh>().MeshSet = 'E';
            enemy.Transform.Position = new Vector3 { x = 3, y = 7 };

            BaseEngine.instance.EngineScreen.SetScreenSize(24,24);

            BaseEngine.instance.Run();
        }
    }
}