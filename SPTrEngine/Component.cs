using System;

namespace SPTrEngine
{
    public class Component
    { 
        private GameObject _gameObject;

        public GameObject GameObject
        {
            get
            {
                return _gameObject;
            }

            set
            {
                _gameObject ??= value;
            }
        }

        public Transform Transform => _gameObject.Transform;

#pragma warning disable CS8618
        protected Component()
        {

        }
#pragma warning restore CS8618

        /// <summary>
        /// 사용자가 직접 컴포넌트의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Component CreateInstance(GameObject gameObject)
        {
            var instance = new Component();

            instance.GameObject = gameObject;

            return instance;
        }
    }
}
