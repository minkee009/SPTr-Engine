using SPTrApp.ExampleApp;
using SPTrEngine;
using SPTrEngine.Math.Vector;

namespace SPTrApp
{
    internal static class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            //AsyncSubRoutine routine = new AsyncSubRoutine(
            //    new Delegate[]
            //    {
            //        () => { Console.WriteLine("페이즈1"); },
            //        () => { Console.WriteLine("페이즈2"); },
            //        () => { return 42; }
            //    }
            //);


            //while (routine.MoveNext())
            //{
            //    Console.WriteLine("중간출력");
            //    Console.WriteLine(routine.Current ?? "없어욥");
            //}

            InternalCMD.MainArgsCMD(args);

            GameObject player = new GameObject("Player");
            player.AddComponent<Player>();
            player.AddComponent<Mesh>().MeshSet = 'P';
            player.Transform.Position = new Vector3 { x = 5, y = 4 };


            GameObject enemy = new GameObject("Enemy");
            enemy.AddComponent<Enemy>();
            enemy.AddComponent<Mesh>().MeshSet = 'E';
            enemy.Transform.Position = new Vector3 { x = 3, y = 7 };

            BaseEngine.instance.EngineScreen.SetScreenSize(24, 24);

            BaseEngine.instance.Run();
        }
    }
}
