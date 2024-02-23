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
        public abstract void Reset();
    }

    public class WaitForSeconds : YieldInstruction
    {
        private double _checkTime;
        private float _sec;

        public WaitForSeconds(float sec) 
        {
            _sec = sec;
            Reset();
        }

        public override bool Callable()
        {
            bool check = _checkTime <= Time.time;
            if(check)
            {
                Reset();
            }

            return check;
        }

        public override void Reset()
        {
            _checkTime = Time.time + _sec;
        }
    }

    public class WaitForFixedTick : YieldInstruction
    {
        public override bool Callable()
        {
            return BaseEngine.State == EngineState.FixedTick;
        }
        public override void Reset()
        {

        }
    }

    public class WaitForEndOfFrame : YieldInstruction
    {
        public override bool Callable()
        {
            return BaseEngine.State == EngineState.Render;
        }

        public override void Reset()
        {
            
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

        public override void Reset()
        {
            
        }
    }

    public class Coroutine : YieldInstruction
    {
        public string methodName;
        public IEnumerator enumerator;
        public YieldInstruction? waitOption;
        public bool Done => _done;

        private bool _done = false;


        public Coroutine(string methodName, IEnumerator enumerator, YieldInstruction? waitOption)
        {
            this.methodName = methodName;
            this.enumerator = enumerator;
            this.waitOption = waitOption;
        }

        public override bool Callable()
        {
            if (waitOption as Coroutine != null)
                return ((Coroutine)waitOption).Done;
            else
                return waitOption?.Callable() ?? BaseEngine.State == EngineState.Tick;
        }

        public bool MoveNext()
        {
            if (!enumerator.MoveNext())
            {
                _done = true;
                return false;
            }

            else
            {
                if (enumerator.Current as YieldInstruction != null)
                {
                    waitOption = (YieldInstruction)enumerator.Current;
                    waitOption.Reset();
                }
                    
                else 
                    waitOption = null;

                return true;
            }
            
        }

        public override void Reset()
        {
            waitOption?.Reset();
        }
    }
}
