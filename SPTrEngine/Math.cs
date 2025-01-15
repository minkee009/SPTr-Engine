using Silk.NET.Maths;

namespace SPTrEngine.Math
{
    public struct Vector2
    {
        public float x, y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2 operator *(Vector2 vec, float scale)
        {
            return new Vector2(vec.x * scale, vec.y * scale);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x / v2.x, v1.y / v2.y);
        }
        public static Vector2 operator /(Vector2 vec, float scale)
        {
            return new Vector2(vec.x / scale, vec.y / scale);
        }


        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2);
        }


        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static readonly Vector2 up = new Vector2(0, 1);
        public static readonly Vector2 down = new Vector2(0, -1);
        public static readonly Vector2 right = new Vector2(1, 0);
        public static readonly Vector2 left = new Vector2(-1, 0);
        public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 one = new Vector2(1, 1);

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int((int)x, (int)y);
        }

        public static Vector2 Convert(Vector2D<float> vector2d) => new Vector2(vector2d.X, vector2d.Y);

        public static Vector2 Convert(Vector2D<int> vector2d) => new Vector2(vector2d.X, vector2d.Y);

        public Vector2 Normalized
        {
            get
            {
                Vector2 result = new Vector2(x, y);
                result.Normalize();
                return result;
            }
        }

        public float Magnitude => MathF.Sqrt(x * x + y * y);

        public void Normalize()
        {
            float num = Magnitude;
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = zero;
            }
        }
    }

    public struct Vector2Int
    {
        public int x, y;


        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2Int operator *(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2Int operator /(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x / v2.x, v1.y / v2.y);
        }

        public static bool operator ==(Vector2Int v1, Vector2Int v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Vector2Int v1, Vector2Int v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static Vector2Int Up => new Vector2Int(0, 1);
        public static Vector2Int Down => new Vector2Int(0, -1);
        public static Vector2Int Right => new Vector2Int(1, 0);
        public static Vector2Int Left => new Vector2Int(-1, 0);
        public static Vector2Int Zero => new Vector2Int(0, 0);
        public static Vector2Int One => new Vector2Int(1, 1);

        public static Vector2Int Convert(Vector2D<float> vector2d) => new Vector2Int((int)vector2d.X, (int)vector2d.Y);

        public static Vector2Int Convert(Vector2D<int> vector2d) => new Vector2Int(vector2d.X, vector2d.Y);

    }

    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static Vector3 operator *(Vector3 vec, float scale)
        {
            return new Vector3(vec.x * scale, vec.y * scale, vec.z * scale);
        }

        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        public static Vector3 operator /(Vector3 vec, float scale)
        {
            return new Vector3(vec.x / scale, vec.y / scale, vec.z / scale);
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return !(v1 == v2);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public float Magnitude => (float)System.Math.Sqrt(x * x + y * y + z * z);

        public float SqrMagnitude => x * x + y * y + z * z;

        public static readonly Vector3 zero = new Vector3(0, 0, 0);
        public static readonly Vector3 one = new Vector3(1, 1, 1);
        public static readonly Vector3 forward = new Vector3(0, 0, 1);
        public static readonly Vector3 back = new Vector3(0, 0, -1);
        public static readonly Vector3 up = new Vector3(0, 1, 0);
        public static readonly Vector3 down = new Vector3(0, -1, 0);
        public static readonly Vector3 right = new Vector3(1, 0, 0);
        public static readonly Vector3 left = new Vector3(-1, 0, 0);

        public void Normalize()
        {
            float num = Magnitude;
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = zero;
            }
        }
    }
}