using SPTrEngine;
using SPTrEngine.Math.Vector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPTrApp.SPTrEngine
{
    public interface ISPTrLoop
    {
        public void Awake();
        public void OnInitialized();
        public void OnEnable();
        public void OnDisable();
        public void Start();
        public void FixedTick();
        public void Tick();
        public void AfterTick();
    }

    public class ScriptBehavior : Component, ISPTrLoop
    {
        private bool _enabled = true;
        private bool _lastEnabled = true;
        private bool _startHasExcute = false;
        private bool _hasInitalized = false;

        private Dictionary<string, Coroutine> _activatedCoroutines = new Dictionary<string, Coroutine>();
        private static List<string> _needStopRoutines = new List<string>(8);

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _lastEnabled = _enabled;
                _enabled = value;

                if (_lastEnabled != _enabled)
                {
                    if (_enabled)
                    {
                        OnEnable();
                        if (!_startHasExcute)
                        {
                            Start();
                            _startHasExcute = true;
                        }
                    }
                    else
                        OnDisable();
                }
            }
        }

        public void OnInitialized()
        {
            if (_hasInitalized)
                return;

            Awake();

            Enabled = GameObject?.Enabled ?? false;

            if (Enabled)
            {
                OnEnable();
                Start();
                _startHasExcute = true;
            }

            _hasInitalized = true;
        }

        public void CheckYield()
        {
            _needStopRoutines.Clear();

            foreach (var r in _activatedCoroutines.Values.ToArray())
            {
                if (r.Callable() && !r.MoveNext())
                    _needStopRoutines.Add(r.methodName);
            }

            if (_needStopRoutines.Count > 0)
            {
                foreach (var name in _needStopRoutines)
                    StopCoroutine(name);
            }

            _needStopRoutines.Clear();
        }

        public Coroutine? StartCoroutine(string methodName)
        {
            MethodInfo? routineInfo = GetType().GetMethod(methodName);

            if (routineInfo != null
                && routineInfo.ReturnType == typeof(IEnumerator)
                && !_activatedCoroutines.ContainsKey(methodName))
            {
                _activatedCoroutines.Add(methodName, new Coroutine(methodName, (IEnumerator)routineInfo.Invoke(this, null), null));
                _activatedCoroutines[methodName]?.MoveNext();
                return _activatedCoroutines[methodName];
            }

            else
                return null;

        }

        public void StopCoroutine(string methodName)
        {
            if (_activatedCoroutines.ContainsKey(methodName)
                && _activatedCoroutines[methodName].waitOption is Coroutine)
                StopCoroutine(((Coroutine)_activatedCoroutines[methodName].waitOption).methodName);

            _activatedCoroutines.Remove(methodName);
        }

        public void StopAllCoroutines()
        {
            _activatedCoroutines.Clear();
        }


        public virtual void AfterTick()
        {
            
        }

        public virtual void Awake()
        {
            
        }

        public virtual void FixedTick()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }

        public virtual void OnEnable()
        {
            
        }

        public virtual void Start()
        {
            
        }

        public virtual void Tick()
        {
            
        }
    }
}
