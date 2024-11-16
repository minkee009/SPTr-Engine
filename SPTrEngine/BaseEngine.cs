using System.Diagnostics;
using System.Runtime.InteropServices;
using SPTrEngine.Math.Vector;
using System.Text;
using SPTrEngine.Extensions.Kernel32;
using Microsoft.Win32.SafeHandles;
using System.Security.Policy;

namespace SPTrEngine
{
    public enum EngineState
    {
        CheckInput = 0,
        FixedTick = 1,
        Tick = 2,
        AfterTick = 3,
        Render = 4,
    }

    public class BaseEngine
    {
        public static BaseEngine instance = new BaseEngine();
        public static IReadOnlyList<GameObject> Objects => _objects;
        private static List<GameObject> _objects = new List<GameObject>();

        private EngineState _state;
        private bool _isExit;
        private ConsoleRenderer _consoleRenderer;
        private Queue<Action> _objCountManager;
        private Dictionary<string, string> _hashs;

        public IConsoleScreen EngineScreen => _consoleRenderer;

        public EngineState State => _state;

        BaseEngine()
        {
            _state = EngineState.CheckInput;
            _isExit = false;
            _consoleRenderer = new ConsoleRenderer();
            _objCountManager = new Queue<Action>();
            _hashs = new Dictionary<string, string>();
        }

        public void Run()
        {
            Kernel32.Beep(200, 55);
            Kernel32.Beep(350, 30);
            Kernel32.Beep(240, 55);
            Stopwatch stopwatch = Stopwatch.StartNew();

            _consoleRenderer.CreateScreenHandle();

            double accumlator = 0;

            while (!_isExit)
            {
                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                accumlator += Time.deltaTime;

                //오브젝트 등록관리
                while (_objCountManager.Count > 0)
                {
                    _objCountManager.Dequeue().Invoke();
                }

                _state = EngineState.CheckInput;
                Input.ScanInput();

                //fixedTick
                _state = EngineState.FixedTick;
                while (accumlator > 0.0)
                {
                    foreach (var obj in _objects)
                    {
                        if (obj.Enabled)
                        {
                            foreach (var com in obj.Components)
                            {
                                var script = com as ScriptBehavior;

                                if (script?.Enabled ?? false)
                                {
                                    script.FixedTick();
                                    script.CheckYield();
                                }
                            }
                        }

                    }
                    accumlator -= Time.fixedDeltaTime;
                }

                //tick
                _state = EngineState.Tick;
                foreach (var obj in _objects)
                {
                    if (obj.Enabled)
                    {
                        foreach (var com in obj.Components)
                        {
                            var script = com as ScriptBehavior;

                            if (script?.Enabled ?? false)
                            {
                                script.Tick();
                                script.CheckYield();
                            }
                        }
                    }
                }

                //after tick
                _state = EngineState.AfterTick;
                foreach (var obj in _objects)
                {
                    if (obj.Enabled)
                    {
                        foreach (var com in obj.Components)
                        {
                            var script = com as ScriptBehavior;

                            if (script?.Enabled ?? false)
                            {
                                script.AfterTick();
                                script.CheckYield();
                            }
                        }
                    }
                }

                //사운드 처리

                //화면 처리
                _state = EngineState.Render;
                _consoleRenderer.Render(_objects);
                foreach (var obj in _objects)
                {
                    if (obj.Enabled)
                    {
                        foreach (var com in obj.Components)
                        {
                            var script = com as ScriptBehavior;

                            if (script?.Enabled ?? false)
                            {
                                script.CheckYield();
                            }
                        }
                    }
                }
            }
        }

        public void Exit()
        {
            _isExit = true;
        }

        public void Dispose()
        {

        }

        public void RegisterGameObject(GameObject obj)
        {
            if (_hashs.ContainsKey(obj.Hash))
                return;

            _hashs.Add(obj.Hash, obj.name);
            _objCountManager.Enqueue(() => _objects.Add(obj));
        }

        public void UnregisterGameObject(GameObject obj) 
        {
            if (!_hashs.ContainsKey(obj.Hash))
                return;

            _hashs.Remove(obj.Hash);
            _objCountManager.Enqueue(() => _objects.Remove(obj));
        }
    }
}

