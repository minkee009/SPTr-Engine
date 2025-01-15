using SPTrEngine;
using SPTrEngine.Extensions.Kernel32;
using SPTrEngine.Math.Vector;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Timers;

namespace SPTrApp
{
    public class Player : ScriptBehavior
    {
        public float moveSpeed = 8f;
        public float footprintTime = 0;

        public GameObject? projectile;
        public Vector3 currentDir;


        public IEnumerator Attack()
        {
            Console.WriteLine("공격모션 호출");
            yield return new WaitForFixedTick();
            Console.WriteLine("공격모션 시작");
            
            yield return StartCoroutine("Bullet");
            Console.WriteLine("공격모션 끝");
        }

        public IEnumerator Bullet()
        {
            Console.WriteLine("총알출발!");
            yield return waitSec;

            yield return StartCoroutine("Smoke");
        }

        public IEnumerator Smoke()
        {
            Console.WriteLine("연기 나옴");
            yield return new WaitUntil(() => _spaceCount > 8);
            _spaceCount = 0;

            Console.WriteLine("연기 사라짐");
        }

        private int _spaceCount = 0;

        public override void Tick()
        {
            int h = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
            int v = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);

            Vector2 input = new Vector2(v, h).Normalized;

            Transform.Position += new Vector3(input.x, input.y, 0f) * moveSpeed * (float)Time.deltaTime;

            footprintTime += input.Magnitude > 0f ? (float)Time.deltaTime : 0;

            if (footprintTime > 0.7)
            {
                Kernel32.Beep(300, 25);
                footprintTime = 0;
            }

            if (input.Magnitude > 0f)
            {
                currentDir = new Vector3(input.x, input.y, 0f);
            }

            if (Input.GetKeyDown(ConsoleKey.Z))
            {
                StartCoroutine("Shoot");
            }
        }

        public IEnumerator Shoot()
        {
            if(projectile == null)
            {
                projectile = new GameObject("Player Projectile");
                projectile.AddComponent<Mesh>().MeshSet = 'o';
            }
            projectile.Enabled = true;
            projectile.Transform.Position = Transform.Position;

            float sec = 0f;
            Vector3 moveDir = currentDir;

            while (sec < 3f)
            {
                sec += (float)Time.deltaTime;

                projectile.Transform.Position += moveDir * 12f * (float)Time.deltaTime;

                yield return null;
            }

            projectile.Enabled = false;
        }
    }
}
