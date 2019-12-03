using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEMGU1
{
    class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(PointF A, PointF B)
        {
            X = A.X - B.X;
            Y = A.Y - B.Y;
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
    }
}
