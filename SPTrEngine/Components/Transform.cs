using SPTrEngine.Math.Vector;

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

        //public Vector3 LocalPosition { get; set; }
        //public Quaternion LocalRotation { get; set; }
        //public Vector3 LocalScale { get; set; }

        public IList<Transform> Childs { get => _childs; }

        protected Transform()
        {
            _childs = new List<Transform>();
            _parent = null;
        }

        /// <summary>
        /// 게임오브젝트의 트랜스폼 컴포넌트 인스턴스를 직접 생성합니다.
        /// 단 게임오브젝트의 트랜스폼 컴포넌트 인스턴스가 이미 존재하는 경우에는 생성하지 않습니다.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static new Transform CreateInstance(GameObject go)
        {
            if (go.Transform != null)
                return go.Transform;

            var instance = new Transform();

            instance.Position = Vector3.zero;
            instance.Scale = Vector3.one;

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
