using SPTrEngine;
using System.Numerics;

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
                Transform.Position += Vector3.UnitY;
            }

            var currentPos = Transform.Position;

            currentPos.Y %= 24;

            Transform.Position = currentPos;
        }
    }
}
