using System;
using Emgu.CV.Structure;

namespace HexagonalWpf
{
    internal class ClusterElement
    {
        public int Id { get; }
        public RotatedRect Element { get; }
        public ClusterElement(int id, RotatedRect rect)
        {
            Id = id;
            Element = rect;
        }

        public float Range(RotatedRect el)
        {
            return (float)Math.Sqrt(Math.Pow(Element.Center.X - el.Center.X, 2) + Math.Pow(Element.Center.Y - el.Center.Y, 2));
        }

        public double Diameter => (Element.Size.Width + Element.Size.Height) / 2;
    }
}
