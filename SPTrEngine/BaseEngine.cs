using System.Diagnostics;
using System.Runtime.InteropServices;
using SPTrEngine.Math.Vector;
using System.Text;
using SPTrEngine.Extensions.Kernel32;
using SPTrApp.SPTrEngine;
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

        public IConsoleScreen EngineScreen => _consoleRenderer;

        public EngineState State => _state;

        private EngineState _state;

        private bool _isExit = false;

        private ConsoleRenderer _consoleRenderer;

        private double _accumlator = 0;



        BaseEngine()
        {
            _consoleRenderer = new ConsoleRenderer();
        }

        public void Run()
        {
            Kernel32.Beep(200, 55);
            Kernel32.Beep(350, 30);
            Kernel32.Beep(240, 55);
            Stopwatch stopwatch = Stopwatch.StartNew();

            _consoleRenderer.CreateScreenHandle();

            while (!_isExit)
            {
                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                _accumlator += Time.deltaTime;

                _state = EngineState.CheckInput;
                Input.SetInput();

                //fixedTick
                _state = EngineState.FixedTick;
                while (_accumlator > 0.0)
                {
                    foreach (var obj in objects)
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
                    _accumlator -= Time.fixedDeltaTime;
                }

                //tick
                _state = EngineState.Tick;
                foreach (var obj in objects)
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
                foreach (var obj in objects)
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
                _consoleRenderer.Render(objects);
                foreach (var obj in objects)
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
    }
}

