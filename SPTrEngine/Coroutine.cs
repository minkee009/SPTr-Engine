using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPTrEngine
{
    public abstract class YieldInstruction
    {
        public abstract bool Callable();
    }

    public class WaitForSeconds : YieldInstruction
    {
        private double _checkTime;
        private float _sec;

        public WaitForSeconds(float sec) 
        {
            _sec = sec;
            _checkTime = Time.time + sec;
        }

        public override bool Callable()
        {
            bool check = _checkTime < Time.time;
            if(check)
            {
                _checkTime = Time.time + _sec;
            }

            return check;
        }
    }

    public class WaitForFixedTick : YieldInstruction
    {
        public override bool Callable()
        {
            return BaseEngine.State == EngineState.FixedTick;
        }
    }

    public class WaitForEndOfFrame : YieldInstruction
    {
        public override bool Callable()
        {
            return BaseEngine.State == EngineState.Render;
        }
    }

    public class WaitUntil :YieldInstruction
    {
        public Func<bool> func;

        public WaitUntil(Func<bool> func)
        {
            this.func = func;
        }

        public override bool Callable()
        {
            return func.Invoke();
        }
    }

    public class Coroutine : YieldInstruction
    {
        public string methodName;
        public IEnumerator enumerator;
        public YieldInstruction? waitOption;

        public Coroutine(string methodName, IEnumerator enumerator, YieldInstruction? waitOption)
        {
            this.methodName = methodName;
            this.enumerator = enumerator;
            this.waitOption = waitOption;
        }

        public override bool Callable()
        {
            return waitOption?.Callable() ?? false;
        }

        public bool MoveNext()
        {
            if (!enumerator.MoveNext())
            {
                return false;
            }

            return true;
        }
    }
}
