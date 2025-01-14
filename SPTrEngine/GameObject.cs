﻿using SPTrEngine.Tools;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace SPTrEngine
{
    public sealed class GameObject : IDisposable, IEquatable<GameObject>
    {
        public string name = "";
        public string tag = "";
        public int layer = 0;

        private bool _enabled = true;
        private List<Component> _components;
        private string _hash;

        public Transform Transform { get; }
        public IReadOnlyList<Component> Components { get => _components; }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public string Hash => _hash;


        public GameObject()
        {
            Random hashPONum = new Random();
            name = $"[{BaseEngine.Objects.Count}]GameObject";
            _hash = HashMaker.ComputeSHA256(name + hashPONum.Next() + BaseEngine.Objects.Count);
            _components = new List<Component>();
            Transform = Transform.CreateInstance(this);
            _components.Add(Transform);
            BaseEngine.instance.RegisterGameObject(this);
        }

        public GameObject(string name)
        {
            Random hashPONum = new Random();
            this.name = name;
            _hash = HashMaker.ComputeSHA256(name + hashPONum.Next() + BaseEngine.Objects.Count);
            _components = new List<Component>();
            Transform = Transform.CreateInstance(this);
            _components.Add(Transform);
            BaseEngine.instance.RegisterGameObject(this);
        }


        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        public void Dispose()
        {

        }

        public bool Equals(GameObject? other)
        {
            return _hash == other?._hash;
        }

        public static GameObject? Find(string name)
        {
            GameObject? findObj = null;

            foreach(var obj in BaseEngine.Objects)
            {
                if(obj.name == name)
                {
                    findObj = obj;
                    break;
                }
            }

            return findObj;
        }

        public static GameObject[]? FindByTag(string tag)
        {
            List<GameObject> findObjs = new List<GameObject>();
            
            foreach(var obj in BaseEngine.Objects)
            {
                if (obj.tag == tag)
                {
                    findObjs.Add(obj);
                }
            }

            return findObjs.Count > 0 ? findObjs.ToArray() : null;
        }
        public Component? GetComponent(Type type)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].GetType() == type)
                    return _components[i];
            }

            return null;
        }

        public T? GetComponent<T>() where T : Component
        {
            return GetComponent(typeof(T)) as T;
        }

        public bool TryGetComponent<T>(out T? component) where T : Component
        {
            component = null;
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T validComponent)
                {
                    component = validComponent;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Component를 추가합니다. 리플렉션을 사용하는 함수입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
        }


        /// <summary>
        /// 타입을 사용하여 Component를 추가합니다. 리플렉션을 사용하는 함수입니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Component AddComponent(Type type)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].GetType() == type)
                    return _components[i];
            }

            Component? instance;

            try
            {
                MethodInfo? createInstance = type.GetMethod("CreateInstance");

                if(createInstance != null) 
                {
                    instance = (Component?)createInstance.Invoke(null, new object[] { this });
                }
                else
                {
                    instance = (Component?)(type.GetConstructor(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                        null, 
                        Type.EmptyTypes,
                        null)?.Invoke(null));
                }
            }
            catch
            {
                throw new NullReferenceException();
            }

            if(instance == null)
                throw new NullReferenceException();

            instance.GameObject = this;

            _components.Add(instance);
            if (instance is ISPTrLoop loop)
            {
                loop.OnInitialized();
            }

            return instance;
        }

        /// <summary>
        /// ScriptBehavior를 추가합니다. (리플렉션을 사용하지 않습니다.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddScript<T>() where T : ScriptBehavior, new()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T validComponent)
                    return validComponent;
            }
            T instance = new()
            {
                GameObject = this
            };

            _components.Add(instance);
            instance.OnInitialized();

            return instance;
        }

        public static void Destroy(Component component)
        {

        }

        public static void Destroy(GameObject gameObject)
        {

        }
    }
}
