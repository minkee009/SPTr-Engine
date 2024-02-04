using SPTrEngine;
using SPTrEngine.Math.Vector;

namespace SPTrApp
{
    public class Enemy : GameObject
    {
        public Enemy(char mesh = 'E')
        {
            _mesh = mesh;
        }

        int _internalTickCount = 0;

        public override void FixedTick()
        {
            base.FixedTick();
            _internalTickCount++;

            _internalTickCount %= 5;

            if (_internalTickCount == 0)
            {
                position += Vector2.Up;
            }

            position.y %= BaseEngine.instance.ScreenSize.y;
        }
    }
}
