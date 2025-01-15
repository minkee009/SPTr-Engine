using System.Numerics;

namespace SPTrEngine
{
    public class Transform : Component
    {
        private List<Transform> _childs;
        private Transform? _parent;

        public Transform? Parent { get => _parent; }
        public Vector3 Position { get; set; }
        //public Quaternion rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }
        //public Vector3 LocalPosition { get; set; }
        //public Quaternion LocalRotation { get; set; }
        //public Vector3 LocalScale { get; set; }

        public IReadOnlyList<Transform> Childs { get => _childs; }

        protected Transform()
        {
            _childs = new List<Transform>();
            _parent = null;
        }

        /// <summary>
        /// 게임오브젝트의 트랜스폼 컴포넌트 인스턴스를 직접 생성합니다.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static new Transform CreateInstance(GameObject gameObject)
        {
            var instance = new Transform();

            instance.Position = new Vector3(0, 0, 0);
            instance.Scale = new Vector3(1, 1, 1);
            instance.Rotation = new Quaternion(0, 0, 0, 1);

            instance.GameObject = gameObject;

            return instance;
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
