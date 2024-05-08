using SFML.Graphics;
using SPTrEngine;
using SPTrEngine.Math.Vector;
using SFML.Window;
using SFML;

namespace SPTrApp
{
    internal static class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            RenderWindow window = new RenderWindow(new VideoMode(320, 240), "엿이나 잡숴!");
            window.Closed += OnClose;

            Color BGColor = new Color(0, 192, 255);

            while(window.IsOpen)
            {
                //이벤트 실행
                window.DispatchEvents();

                //내부 화면 초기화
                window.Clear(BGColor);

                //윈도우 화면 출력
                window.Display();
            }

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

        public static void OnClose(object? sender, EventArgs e)
        {
            if(sender as RenderWindow != null)
            {
                ((RenderWindow)sender).Close();
            }
        }
    }
}