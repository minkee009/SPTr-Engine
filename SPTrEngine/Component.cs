using SPTrEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SPTrApp.SPTrEngine
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
                if (_gameObject == null)
                    _gameObject = value;
            }
        }

        public Transform Transform => _gameObject.Transform;

        protected Component()
        {

        }

        public static object CreateInstance(GameObject gameObject)
        {
            var instance = new Component();

            instance.GameObject = gameObject;

            return instance;
        }
    }
}
