using System;

namespace TestEMGU1
{
    internal class BallElement
    {
        private readonly float m_Rad;

        public BallElement(float x, float y, float r)
        {
            X = x;
            Y = y;
            m_Rad = r;
        }

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
