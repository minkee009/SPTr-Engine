using SPTrEngine;
using SPTrEngine.Math;

namespace SPTrApp
{
    public class Enemy : ScriptBehavior
    {
        int _internalTickCount = 0;

        public override void FixedTick()
        {
            base.FixedTick();
            _internalTickCount++;

            _internalTickCount %= 5;

            if (_internalTickCount == 0)
            {
                Transform.Position += Vector3.up;
            }

            var currentPos = Transform.Position;

            currentPos.y %= BaseEngine.instance.EngineScreen.ScreenSize.y;

            Transform.Position = currentPos;
        }
    }
}
