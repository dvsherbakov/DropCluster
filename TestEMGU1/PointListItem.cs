using System;
using System.Collections.Generic;
using System.Drawing;

namespace TestEMGU1
{
    internal class PointListItem
    {
        private readonly int m_Id;
        private readonly PointF m_Pt;
        private readonly float m_Radius;

        private readonly List<int> links;

        public PointListItem(int i, PointF a, float radius)
        {
            m_Id = i;
            m_Pt = new PointF(a.X, a.Y);
            m_Radius = radius;
            links = new List<int>();
        }

        public double GetDistance(PointF a)
        {
            return Math.Sqrt(Math.Pow(m_Pt.X - a.X, 2) + Math.Pow(m_Pt.Y - a.Y, 2));
        }

        public bool IsTouched(PointListItem a)
        {
            return GetDistance(a.GetPoint()) - (m_Radius + a.GetRadius()) < 5;
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

        public void AddLink(int l)
        {
            if (!links.Contains(l)) links.Add(l);
        }
    }
}
