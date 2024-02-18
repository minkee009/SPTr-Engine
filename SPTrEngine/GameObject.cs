using SPTrEngine.Tools;
using SPTrEngine.Math;
using System.Reflection;
using System.Collections;
using Silk.NET.Vulkan;
using Silk.NET.OpenGL;

namespace SPTrEngine
{
    public interface ISptrObject
    {
        public void Awake();
        public void OnEnable();
        public void OnDisable();
        public void Start();
        public void FixedTick();
        public void Tick();
        public void AfterTick();
    }

    public class GameObject : IDisposable, IEquatable<GameObject>, ISptrObject
    {
        public Vector2 position = new Vector2(0, 0);
        public string name = "";
        public string tag = "";
        public int layer = 0;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _lastEnabled = _enabled;
                _enabled = value;

                if (_lastEnabled != _enabled)
                {
                    if(_enabled)
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
        public string Hash => _hash;
        public char Mesh => _mesh;

        protected char _mesh = '.';
        protected bool _enabled = true;
        protected bool _lastEnabled = true;

        private string _hash;
        private bool _startHasExcute = false;
        private Dictionary<string,Coroutine> _activatedCoroutines = new Dictionary<string, Coroutine>();


        public GameObject(string name, char mesh = '.', bool enabled = true)
        {
            position = Vector2.Zero;
            this.name = name;
            _hash = HashMaker.ComputeSHA256(name);
            _mesh = mesh;

            BaseEngine.objects.Add(this);
            Awake();

            _enabled = enabled;
            _lastEnabled = enabled;
            if (_enabled)
            {
                OnEnable();
                Start();
            }
        }

        public GameObject(char mesh = '.')
        {
            position = Vector2.Zero;
            name = $"GameObject[{BaseEngine.objects.Count}]";
            _hash = HashMaker.ComputeSHA256(name);
            _mesh = mesh;

            BaseEngine.objects.Add(this);
            Awake();
            OnEnable();
            Start();
        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void Awake()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void FixedTick()
        {

        }

        public virtual void Tick()
        {

        }

        public virtual void AfterTick()
        {

        }

        public void CheckYield()
        {
            List<string> needStop = new List<string>(_activatedCoroutines.Count);
            Coroutine[] list = new Coroutine[_activatedCoroutines.Count];
            _activatedCoroutines.Values.CopyTo(list, 0);
            foreach (var routine in list)
            {
                if (routine.Callable()
                    || (routine.waitOption == null 
                    && BaseEngine.State == EngineState.Tick))
                {
                    if (routine.MoveNext())
                    {
                        if (routine.enumerator.Current as YieldInstruction != null)
                            routine.waitOption = (YieldInstruction)routine.enumerator.Current;
                    }
                    else
                        needStop.Add(routine.methodName);
                }
            }

            if(needStop.Count > 0 )
            {
                foreach(var name in needStop)
                    StopCoroutine(name);
            }

            needStop.Clear();
        }

        public void StartCoroutine(string methodName)
        {
            MethodInfo? routineInfo = GetType().GetMethod(methodName);

            if(routineInfo != null 
                && routineInfo.ReturnType == typeof(IEnumerator)
                && !_activatedCoroutines.ContainsKey(methodName))
            {
                _activatedCoroutines.Add(methodName, new Coroutine(methodName, (IEnumerator)routineInfo.Invoke(this,null), null));
                _activatedCoroutines[methodName]?.MoveNext();

                if (_activatedCoroutines[methodName].enumerator.Current as YieldInstruction != null)
                    _activatedCoroutines[methodName].waitOption = (YieldInstruction)_activatedCoroutines[methodName].enumerator.Current;
            }
        }

        public void StopCoroutine(string methodName)
        {
            _activatedCoroutines.Remove(methodName);
        }

        public void StopAllCoroutines()
        {
            _activatedCoroutines.Clear();
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        public void Dispose()
        {

        }

        public bool Equals(GameObject? other)
        {
            return _hash == other?._hash;
        }

        public static GameObject? FindObjectByName(string name)
        {
            GameObject? findObj = null;
            string toHash = HashMaker.ComputeSHA256(name);

            foreach (var obj in BaseEngine.objects)
            {
                if (obj._hash == toHash)
                {
                    findObj = obj;
                    break;
                }
            }

            return findObj;
        }

        public static GameObject? FindObjectByTag(string tag)
        {
            GameObject? findObj = null;
            
            foreach(var obj in BaseEngine.objects)
            {
                if (obj.tag == tag)
                {
                    findObj = obj;
                    break;
                }
            }

            return findObj;
        }
    }
}
