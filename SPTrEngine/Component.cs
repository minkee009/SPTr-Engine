using SPTrEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTrApp.SPTrEngine
{

    public class Component
    {
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


        private GameObject _gameObject;

        public Transform Transform => _gameObject.Transform;

        protected Component()
        {
            //throw new NotImplementedException();   
        }

        protected Component(GameObject go)
        {
            _gameObject = go;
        }

    }
}
