using Silk.NET.Maths;

namespace SPTrEngine.Math.Vector
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

        public static Vector2 Up => new Vector2(0, 1);
        public static Vector2 Down => new Vector2(0, -1);
        public static Vector2 Right => new Vector2(1, 0);
        public static Vector2 Left => new Vector2(-1, 0);
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 One => new Vector2(1, 1);

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
                this = Zero;
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
}