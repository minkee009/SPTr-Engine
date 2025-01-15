using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTrEngine
{
    public class Mesh : Component
    {
        public char MeshSet { get => _meshSet; set => _meshSet = value; }

        char _meshSet;

        protected Mesh() 
        {
            _meshSet = '.';
        }

        public static new Mesh CreateInstance(GameObject gameObject)
        {
            var instance = new Mesh();

            instance.MeshSet = '.';
            instance.GameObject = gameObject;

            return instance;
        }
    }
}
