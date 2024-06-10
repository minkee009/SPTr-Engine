﻿using SPTrEngine.Tools;
using SPTrEngine.Math.Vector;
using System.Reflection;
using System.Collections;
using SPTrApp.SPTrEngine;
using System.Runtime.CompilerServices;

namespace SPTrEngine
{
    public sealed class GameObject : IDisposable, IEquatable<GameObject>
    {
        public string name = "";
        public string tag = "";
        public int layer = 0;

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

        private bool _enabled = true;
        private List<Component> _components = new List<Component>();
        private string _hash;
        private Dictionary<string, Coroutine> _activatedCoroutines = new Dictionary<string, Coroutine>();
        private static List<string> _needStopRoutines = new List<string>(8);


        public GameObject()
        {
            name = $"[{BaseEngine.objects.Count}]GameObject";
            _hash = HashMaker.ComputeSHA256(name);
            Transform = Transform.CreateInstance(this);
            Components.Add(Transform);
            BaseEngine.objects.Add(this);
        }

        public GameObject(string name)
        {
            this.name = name;
            _hash = HashMaker.ComputeSHA256(name);
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

        public T? GetComponent<T>() where T : Component
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                    return (T)_components[i];
            }

            return null;
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

        public bool TryGetComponent<T>(out T? component) where T : Component
        {
            component = default(T);
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                {
                    component = (T)_components[i];
                    return true;
                }
            }

            return false;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            for(int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                    return (T)_components[i];
            }

            T instance = new T();
            instance.GameObject = this;

            _components.Add(instance);
            if (instance is ISPTrLoop)
            {
                ((ISPTrLoop)instance).OnInitialized();
            }
               

            return instance;
        }
    }
}
