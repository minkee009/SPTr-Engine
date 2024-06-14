using SPTrEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTrApp.SPTrEngine
{
    public class Mesh : Component
    {
        public char MeshSet { get => _meshSet; set => _meshSet = value; }

        char _meshSet;

        protected Mesh() 
        {
            _meshSet = '.';
        }

        public static new Mesh CreateInstance(GameObject go)
        {
            var instance = new Mesh();

            instance.GameObject = go;
            instance.MeshSet = '.';

            go.Components.Add(instance);

            return instance;
        }
    }
}
