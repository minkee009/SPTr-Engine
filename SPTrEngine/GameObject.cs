using SPTrEngine.Tools;
using SPTrEngine.Math.Vector;

namespace SPTrEngine
{
    public class GameObject : IDisposable, IEquatable<GameObject>, ISptrObject
    {
        public Vector2 position = new Vector2(0, 0);
        public string name = "";

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
        protected string _consoleMSG = "";

        private bool _lastEnabled = true;
        private string _hash;
        private bool _startHasExcute = false;


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

        public bool CheckConsoleMSG() => _consoleMSG != "";

        public static GameObject? FindObjectByName(string name)
        {
            GameObject findObj = null;
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
    }
}
