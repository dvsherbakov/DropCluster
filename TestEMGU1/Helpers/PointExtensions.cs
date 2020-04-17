using System;
using System.Drawing;

namespace TestEMGU1.Helpers
{
    public static class PointExtensions
    {
        public static double GetDistance(PointF source, PointF destination)
        {
            return GetDistance(source.X, source.Y, destination.X, destination.Y);
        }

        public static double GetDistance(float sourceX, float sourceY, float destinationX, float destinationY)
        {
            return Math.Sqrt(Math.Pow(sourceX - destinationX, 2) + Math.Pow(sourceY - destinationY, 2));
        }
    }
}