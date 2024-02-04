using SPTrEngine;
using SPTrEngine.Math.Vector;

namespace SPTrApp
{
    public class Player : GameObject
    {
        public float moveSpeed = 8f;

        public Player(char mesh)
        {
            _mesh = mesh;
            _enabled = true;
        }

        public override void Tick()
        {
            int h = (Input.GetKey(ConsoleKey.UpArrow) ? 1 : 0) + (Input.GetKey(ConsoleKey.DownArrow) ? -1 : 0);
            int v = (Input.GetKey(ConsoleKey.RightArrow) ? 1 : 0) + (Input.GetKey(ConsoleKey.LeftArrow) ? -1 : 0);

            position += new Vector2(v, h).Normalized * moveSpeed * (float)Time.deltaTime;
        }
    }
}
