﻿using SPTrEngine.Tools;
using SPTrEngine.Math.Vector;
using System.Reflection;
using System.Collections;
using SPTrApp.SPTrEngine;
using System.Runtime.CompilerServices;
using static SPTrApp.SPTrEngine.Component;

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
        public IList<Component> Components { get => _components; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                foreach(var component in Components)
                {
                    if (component is ScriptBehavior)
                        ((ScriptBehavior)component).Enabled = value;
                }
            }
        }

        public string Hash => _hash;


        public GameObject()
        {
            name = $"[{BaseEngine.objects.Count}]GameObject";
            _hash = HashMaker.ComputeSHA256(name);
            _components = new List<Component>();
            Transform = Transform.CreateInstance(this);
            Components.Add(Transform);
            BaseEngine.objects.Add(this);
        }

        public GameObject(string name)
        {
            this.name = name;
            _hash = HashMaker.ComputeSHA256(name);
            _components = new List<Component>();
            Transform = Transform.CreateInstance(this);
            Components.Add(Transform);
            BaseEngine.objects.Add(this);
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

        public static GameObject? FindObjectByName(string name)
        {
            GameObject? findObj = null;
            string toHash = HashMaker.ComputeSHA256(name);

            foreach (var obj in BaseEngine.objects)
            {
                if (obj._hash == toHash)
                {
                    findObj = obj;
                    break;
                }
            }

            return findObj;
        }

        public static GameObject? FindObjectByTag(string tag)
        {
            GameObject? findObj = null;
            
            foreach(var obj in BaseEngine.objects)
            {
                if (obj.tag == tag)
                {
                    findObj = obj;
                    break;
                }
            }

            return findObj;
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
        public T AddComponent<T>() where T : Component //, new()
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
                if (_components[i] is T)
                    return (T)_components[i];
            }
            T instance = new()
            {
                GameObject = this
            };

            _components.Add(instance);
            if (instance is ISPTrLoop loop)
            {
                loop.OnInitialized();
            }

            return instance;
        }

        public void Destroy(Component component)
        {

        }
    }
}
