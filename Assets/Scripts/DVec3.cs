using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class DVec3
    {
        public double x, y, z;
        public DVec3() : this(0) { }
        public DVec3(double a, double b, double c)
        {
            x = a;
            y = b;
            z = c;
        }

        public DVec3(Vector3 value) : this(value.x, value.y, value.z) { }

        public DVec3(float a, float b, float c) : this((double)a, (double)b, (double)c) { }

        public DVec3(double content) : this(content, content, content) { }

        public DVec3(float content) : this((double)content) { }

        public DVec3(DVec3 other) : this(other.x, other.y, other.z) { }

        public static DVec3 operator +(DVec3 a) => a;
        public static DVec3 operator -(DVec3 a) => new DVec3(-a.x, -a.y, -a.z);

        public static DVec3 operator +(DVec3 a, DVec3 b)
            => new DVec3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static DVec3 operator -(DVec3 a, DVec3 b)
            => new DVec3(a.x - b.x, a.y - b.y, a.z - b.z);

        public static DVec3 operator *(DVec3 a, DVec3 b)
            => new DVec3(a.x * b.x, a.y * b.y, a.z * b.z);

        public static DVec3 operator /(DVec3 a, DVec3 b)
            => new DVec3(a.x / b.x, a.y / b.y, a.z / b.z);

        public static DVec3 operator +(DVec3 a, double b)
            => new DVec3(a.x + b, a.y + b, a.z + b);

        public static DVec3 operator -(DVec3 a, double b)
            => new DVec3(a.x - b, a.y - b, a.z - b);

        public static DVec3 operator *(DVec3 a, double b)
            => new DVec3(a.x * b, a.y * b, a.z * b);

        public static DVec3 operator /(DVec3 a, double b)
            => new DVec3(a.x / b, a.y / b, a.z / b);

        public Vector3 AsVector()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }

        public double LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }

        public DVec3 Normalized()
        {
            return this / Length();
        }

        public override string ToString()
        {
            return "DVec3("+x+","+y+","+z+")";
        }
    }
}
