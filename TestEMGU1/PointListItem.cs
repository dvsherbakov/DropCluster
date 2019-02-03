using System;
using System.Drawing;

namespace TestEMGU1
{
    internal class PointListItem
    {
        private readonly int _id;
        private PointF _pt;

        public PointListItem(int i, PointF a)
        {
            _id = i;
            _pt = new PointF(a.X, a.Y);
        }

        public double GetDistance(PointF a)
        {
            return Math.Sqrt(Math.Pow(_pt.X - a.X, 2) + Math.Pow(_pt.Y - a.Y, 2));
        }

        public PointF GetPoint()
        {
            return _pt;
        }

        public int Id()
        {
            return _id;
        }
    }
}
