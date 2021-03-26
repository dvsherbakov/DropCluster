using System;
using Emgu.CV.Structure;

namespace PrepareImageFrm
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

        public ClusterElement GetRelativeElement(ClusterRect edges)
        {
            var tmpElement = Element;
            tmpElement.Center.X -= edges.X1;
            tmpElement.Center.Y -= edges.Y1;
            ClusterElement res = new ClusterElement(Id, tmpElement);

            return res;
        }
    }
}
