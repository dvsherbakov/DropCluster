using System;
using System.Drawing;

namespace TestEMGU1
{
    internal class PointListItem
    {
        private readonly int m_Id;
        private readonly PointF m_Pt;
        private readonly float m_Radius;

        public PointListItem(int i, PointF a, float radius)
        {
            m_Id = i;
            m_Pt = new PointF(a.X, a.Y);
            m_Radius = radius;
        }

        public double GetDistance(PointF a)
        {
            return Math.Sqrt(Math.Pow(m_Pt.X - a.X, 2) + Math.Pow(m_Pt.Y - a.Y, 2));
        }

        public bool IsTouched(PointListItem a)
        {
            return GetDistance(a.GetPoint()) - (m_Radius + a.GetRadius()) * 0.525 < 0;
        }

        public PointF GetPoint()
        {
            return m_Pt;
        }

        public int Id()
        {
            return m_Id;
        }

        public float GetRadius()
        {
            return m_Radius;
        }
    }
}
