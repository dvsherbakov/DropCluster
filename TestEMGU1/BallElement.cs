using System;

namespace TestEMGU1
{
    internal class BallElement
    {
        private readonly double m_Area;
        private readonly float m_Rad;

        public BallElement(double a, float x, float y, float r)
        {
            m_Area = a;
            X = x;
            Y = y;
            m_Rad = r;
        }

        public double Area() { return m_Area; }
        public float Radius() { return m_Rad; }
        private float X { get; }
        private float Y { get; }

        public float Range(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y -y, 2));
        }

        public float GetX()
        {
            return X;
        }

        public float GetY()
        {
            return Y;
        }
    }
}
