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

        protected Mesh(GameObject go) : base(go) 
        {

        }

        public static Mesh CreateInstance(GameObject go, char mesh = '.')
        {
            var instance = new Mesh(go);

            instance.MeshSet = mesh;

            go.Components.Add(instance);

            return instance;
        }
    }
}
