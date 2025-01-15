using SPTrEngine;
using SPTrEngine.Extensions.Kernel32;
using System.Numerics;
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


        public override void Tick()
        {
            int h = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
            int v = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);

            Vector2 input = Vector2.Normalize(new Vector2(v, h));

            Transform.Position += new Vector3(input.X, input.Y, 0f) * moveSpeed * (float)Time.deltaTime;

            footprintTime += input.Length() > 0f ? (float)Time.deltaTime : 0;

            if (footprintTime > 0.7)
            {
                Kernel32.Beep(300, 25);
                footprintTime = 0;
            }

            if (input.Length() > 0f)
            {
                currentDir = new Vector3(input.X, input.Y, 0f);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine("Shoot");
            }
        }

        public IEnumerator Shoot()
        {
            if (projectile == null)
            {
                projectile = new GameObject("Player Projectile");
                //projectile.AddComponent<MeshFilter>().MeshSet = 'o';
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
