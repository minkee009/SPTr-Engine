using System.Diagnostics;
using System.Runtime.InteropServices;
using SPTrEngine.Math.Vector;
using System.Text;
using SPTrEngine.Extensions.Kernel32;
using Microsoft.Win32.SafeHandles;

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
        public static List<GameObject> objects = new List<GameObject>();

        private EngineState _state;
        private bool _isExit;
        private ConsoleRenderer _consoleRenderer;

        public IConsoleScreen EngineScreen => _consoleRenderer;

        public EngineState State => _state;

        BaseEngine()
        {
            _state = EngineState.CheckInput;
            _isExit = false;
            _consoleRenderer = new ConsoleRenderer();
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

                var objsCheckCount = objects.Count;

                _state = EngineState.CheckInput;
                Input.SetInput();

                //fixedTick
                _state = EngineState.FixedTick;
                while (accumlator > 0.0)
                {
                    for (int i = 0; i < objsCheckCount; i++)
                    {
                        if (objects[i].Enabled)
                        {
                            var comps = objects[i].Components;
                            var comCount = comps.Count;
                            for (int j = 0; j < comCount; j++)
                            {
                                var script = comps[j] as ScriptBehavior;

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
                for(int i = 0; i < objsCheckCount; i++)
                {
                    if (objects[i].Enabled)
                    {
                        var comps = objects[i].Components;
                        var comCount = comps.Count;
                        for (int j = 0; j < comCount; j++)
                        {
                            var script = comps[j] as ScriptBehavior;

                            if(script?.Enabled ?? false)
                            {
                                script.Tick();
                                script.CheckYield();
                            }
                        }
                    }
                }

                //after tick
                _state = EngineState.AfterTick;
                for (int i = 0; i < objsCheckCount; i++)
                {
                    if (objects[i].Enabled)
                    {
                        var comps = objects[i].Components;
                        var comCount = comps.Count;
                        for (int j = 0; j < comCount; j++)
                        {
                            var script = comps[j] as ScriptBehavior;

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
                _consoleRenderer.Render(objects);
                for (int i = 0; i < objsCheckCount; i++)
                {
                    if (objects[i].Enabled)
                    {
                        var comps = objects[i].Components;
                        var comCount = comps.Count;
                        for (int j = 0; j < comCount; j++)
                        {
                            var script = comps[j] as ScriptBehavior;

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
    }
}

