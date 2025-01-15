using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SPTrEngine
{
    public class Mesh
    {
        public float[] Vertices => _vertices;
        public int[] Indices => _indices;
        public Vector3[] Normals => _normals;
        public Vector3[] Tangents => _tangents;
        public Vector2[] UVs => _uvs;
        public Vector4[] Colors => _colors;

        public Mesh(float[] vertices, int[] indices, Vector3[] normals, Vector3[] tangents, Vector2[] uvs, Vector4[] colors)
        {
            _vertices = vertices;
            _indices = indices;
            _normals = normals;
            _tangents = tangents;
            _uvs = uvs;
            _colors = colors;
        }

        private float[] _vertices;
        private int[] _indices;
        private Vector3[] _normals;
        private Vector3[] _tangents;
        private Vector2[] _uvs;
        private Vector4[] _colors;
    }

    public class MeshFilter : Component
    {
        public Mesh? Mesh { get => _mesh; set => _mesh = value; }

        private Mesh? _mesh;

        protected MeshFilter() 
        {

        }

        public static new MeshFilter CreateInstance(GameObject gameObject)
        {
            var instance = new MeshFilter();

            instance.GameObject = gameObject;

            return instance;
        }
    }
}
