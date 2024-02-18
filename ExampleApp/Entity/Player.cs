using SPTrEngine;
using SPTrEngine.Math;
using System.Collections;

namespace SPTrApp
{
    public class Player : GameObject
    {
        public float moveSpeed = 8f;
        public WaitForSeconds waitSec = new WaitForSeconds(3);

        public Player(char mesh)
        {
            _mesh = mesh;
            _enabled = true;
        }

        public IEnumerator Attack()
        {
            Console.WriteLine("공격모션 호출");
            yield return new WaitForFixedTick();
            Console.WriteLine("공격모션 시작");
            
            yield return waitSec;
            Console.WriteLine("공격모션 끝");
        }

        public override void Tick()
        {
            int h = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
            int v = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);

            position += new Vector2(v, h).Normalized * moveSpeed * (float)Time.deltaTime;

            if(Input.GetKey(KeyCode.Space))
                StartCoroutine("Attack");
        }
    }
}
