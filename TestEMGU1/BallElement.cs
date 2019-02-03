using System;

namespace TestEMGU1
{
    internal class BallElement
    {
        private readonly double _area;
        private readonly float _rad;

        public BallElement(double a, float x, float y, float r)
        {
            _area = a;
            X = x;
            Y = y;
            _rad = r;
        }

        public double Area() { return _area; }
        public float Radius() { return _rad; }
        private float X { get; }
        private float Y { get; }

        public float Range(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y -y, 2));
        }
    }
}
