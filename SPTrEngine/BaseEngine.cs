using System.Diagnostics;
using SPTrEngine.Math.Vector;

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

    public class BaseEngine
    {
        public static BaseEngine instance = new BaseEngine();
        public static List<GameObject> objects = new List<GameObject>();

        public bool IsRunning => _isRunning;

        public long FrameCount => _frameCount;

        public Vector2Int ScreenSize => _screenSize;
        public char[,] Screen => _screen;

        private bool _isExit = false;
        private bool _isRunning = false;

        private char[,] _screen;
        private char[,] _clearedScreen;
        private Vector2Int _screenSize;
        private string _lastScreenString = "";
        private double _accumlator = 0;

        private long _frameCount = 0;

        BaseEngine(int screenSizeW = 10, int screenSizeH = 10)
        {
            SetScreenSize( screenSizeW, screenSizeH );
        }

        public void Run()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _isRunning = true;

            while (!_isExit)
            {
                var currentTime = stopwatch.Elapsed.TotalSeconds;
                Time.deltaTime = currentTime - Time.time;
                Time.time = currentTime;

                _accumlator += Time.deltaTime;

                Input.SetInput();

                //fixedTick
                while (_accumlator > 0.0)
                {
                    foreach (var obj in objects)
                    {
                        if (obj.Enabled)
                            obj.FixedTick();
                    }
                    _accumlator -= Time.fixedDeltaTime;
                }

                //tick
                foreach (var obj in objects)
                {
                    if(obj.Enabled)
                        obj.Tick();
                }

                //after tick
                foreach (var obj in objects)
                {
                    if (obj.Enabled)
                        obj.AfterTick();
                }

                //사운드 처리

                //화면 처리
                Render();

                _frameCount++;
            }
        }

        public void Render()
        {
            //화면 초기화
            Array.Copy(_clearedScreen,_screen,_clearedScreen.Length);

            //렌더링 오브젝트 정보입력 (추후에 Sorting Order 추가하기)
            foreach (var obj in objects)
            {
                var posToInt = obj.position.ToVector2Int();
                if (obj.Mesh != '.' 
                    && posToInt.x < ScreenSize.x && posToInt.x >= 0
                    && posToInt.y < ScreenSize.y && posToInt.y >= 0)
                {
                    _screen[posToInt.y,posToInt.x] = obj.Mesh;
                }
            }

            //최종 출력
            var finalString = "";
            for (int i = _screenSize.y - 1; i > -1; i--)
            {
                for (int j = 0; j < _screenSize.x; j++)
                {
                    finalString += _screen[i, j] + " ";
                    if (j == _screenSize.x - 1)
                        finalString += "\n";
                }
            }

            if (finalString != _lastScreenString)
            {
                Console.Clear();
                Console.WriteLine(finalString);
            }

            _lastScreenString = finalString;
        }

        public void SetScreenSize(int x, int y)
        {
            _screenSize = new Vector2Int(x, y);
            _screen = new char[_screenSize.y, _screenSize.x];
            _clearedScreen = new char[_screenSize.y, _screenSize.x];

            for (int i = _screenSize.y - 1; i > -1; i--)
            {
                for (int j = 0; j < _screenSize.x; j++)
                {
                    _clearedScreen[i, j] = '.';
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

