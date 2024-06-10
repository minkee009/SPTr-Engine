using SPTrEngine;
using SPTrEngine.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPTrApp.SPTrEngine
{
    public class Transform : Component
    {
        public Transform? Parent { get => _parent; }
        public Vector3 Position { get; set; }
        //public Quaternion rotation { get; set; }
        public Vector3 Scale { get; set; }

        //public Vector3 LocalPosition { get; set; }
        //public Quaternion LocalRotation { get; set; }
        //public Vector3 LocalScale { get; set; }

        public IList<Transform> Childs { get => _childs; }

        private List<Transform> _childs;
        private Transform? _parent;

        //버그픽스 : 테스트 필요
        protected Transform(GameObject go) : base(go)
        {
            Position = Vector3.zero;
            Scale = Vector3.one;

            _childs = new List<Transform>();
            _parent = null;
        }

        /// <summary>
        /// 게임오브젝트의 트랜스폼 컴포넌트 인스턴스를 직접 생성합니다.
        /// 단 게임오브젝트의 트랜스폼 컴포넌트 인스턴스가 이미 존재하는 경우에는 생성하지 않습니다.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Transform CreateInstance(GameObject go)
        {
            if (go.Transform != null)
                return go.Transform;

            return new Transform(go);
        }

        public bool SetParent(Transform parent)
        {
            if (parent == this)
                return false;

            //부모 트랜스폼에 찾아가 자신을 지우는 과정 포함
            _parent?.RemoveChild(this);

            _parent = parent;

            return true;
        }

        public bool AddChild(Transform child)
        {
            return false;
        }

        public bool RemoveChild(Transform child)
        {
            return false;
        }
    }
}
