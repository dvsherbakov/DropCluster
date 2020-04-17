using System;
using TestEMGU1.Helpers;

namespace TestEMGU1
{
    internal class BallElement
    {
        public BallElement(float x, float y, float r)
        {
            X = x;
            Y = y;
            Radius = r;
        }

        public float X { get; }

        public float Y { get; }

        public float Radius { get; }

        public float GetRange(float x, float y)
        {
            return (float) PointExtensions.GetDistance(X, Y, x, y);
        }
    }
}