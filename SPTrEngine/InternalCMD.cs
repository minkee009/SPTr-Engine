using SPTrEngine;
using SPTrEngine.Math.Vector;

namespace SPTrEngine
{
    public static class InternalCMD
    {
        [Experimental]
        public static void MainArgsCMD(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    var command = args[i].Substring(1);
                    switch (command)
                    {
                        //게임 오브젝트 생성
                        case "createGO":

                            if (i + 1 >= args.Length)
                                break;

                            string goName = "";
                            char goMesh = '.';
                            bool goEnabled = false;
                            int argsCount = 0;
                            Vector2 pos = new Vector2(0, 0);

                            for (int j = i + 1; j < args.Length; j++)
                            {
                                if (args[j][0] == '-')
                                    break;

                                if (j == i + 1)
                                {
                                    goName = args[j];
                                    argsCount++;
                                }
                                else if (j == i + 2)
                                {
                                    goMesh = args[j][0];
                                    argsCount++;
                                }
                                else if (j == i + 3)
                                {
                                    pos.x = int.Parse(args[j]);
                                    argsCount++;
                                }
                                else if (j == i + 4)
                                {
                                    pos.y = int.Parse(args[j]);
                                    argsCount++;
                                    break;
                                }
                            }

                            i += argsCount;

                            GameObject go = new GameObject();
                            go.Transform.Position = new Vector3(pos.x, pos.y,0f);

                            break;
                    }
                }
            }
        }
    }
}
