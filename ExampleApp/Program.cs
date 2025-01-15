using SPTrApp.ExampleApp;
using SPTrEngine;
using System.Numerics;

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
            //        () => { Console.WriteLine("씨발"); },
            //        () => { Console.WriteLine("페이즈2"); },
            //        () => { return 42; }
            //    }
            //);


            //while (routine.MoveNext())
            //{
            //    Console.WriteLine("좆까");
            //    Console.WriteLine(routine.Current ?? "없어욥");
            //}

            InternalCMD.MainArgsCMD(args);

            GameObject player = new GameObject("Player");
            player.AddComponent<Player>();
            player.Transform.Position = new Vector3 { X = 5, Y = 4 };


            GameObject enemy = new GameObject("Enemy");
            enemy.AddComponent<Enemy>();
            enemy.Transform.Position = new Vector3 { X = 3, Y = 7 };

            BaseEngine.instance.Run();
        }
    }
}