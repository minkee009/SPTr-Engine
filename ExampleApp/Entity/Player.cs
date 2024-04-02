using SPTrEngine;
using SPTrEngine.Extensions.Kernel32;
using SPTrEngine.Math.Vector;

namespace SPTrApp
{
    public class Player : GameObject
    {
        public float moveSpeed = 8f;

        public float footprintTime = 0;

        public Player(char mesh)
        {
            _mesh = mesh;
            _enabled = true;
        }

        public override void Tick()
        {
            int h = (Input.GetKey(ConsoleKey.UpArrow) ? 1 : 0) + (Input.GetKey(ConsoleKey.DownArrow) ? -1 : 0);
            int v = (Input.GetKey(ConsoleKey.RightArrow) ? 1 : 0) + (Input.GetKey(ConsoleKey.LeftArrow) ? -1 : 0);

            Vector2 input = new Vector2(v, h).Normalized;

            position += input * moveSpeed * (float)Time.deltaTime;

            footprintTime += input.Magnitude > 0f ? (float)Time.deltaTime : 0;

            if( footprintTime > 0.7)
            {
                Kernel32.Beep(300, 25);
                footprintTime = 0;
            }
        }
    }
}
