using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEMGU1
{
    internal class Vector
    {
        private double X { get; }
        private double Y { get; }
        private int F { get; }
        private int L { get; }

        public Vector(PointF A, PointF B, int f, int l)
        {
            X = A.X - B.X;
            Y = A.Y - B.Y;
            F = f;
            L = l;
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vector Subtract(Vector b)
        {
            return new Vector(X-b.X, Y - b.Y);
        }

        public Vector Add(Vector b)
        {
            return new Vector(X + b.X, Y + b.Y);
        }

        public double ScalarMul(Vector b)
        {
            return X * b.X + Y * b.Y;
        }

        public bool Identify(int f, int l)
        {
            return (f == F && l == L) || (f == L && l == F);
        }
    }
}
