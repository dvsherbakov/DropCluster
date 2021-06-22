using System;
using System.Linq;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    internal class ClusterElement
    {
        public int Id { get; set;}
        public int ClusterNo { get; set; }
        public RotatedRect Element { get; }
        public int[] Profile;

        public double AverageBrightness => Profile.Average();

        public float Diam => (Element.Size.Height + Element.Size.Width) / 2;

        public ClusterElement(int id, RotatedRect rect, int[] profile)
        {
            Id = id;
            Element = rect;
            Profile = profile;
        }

        public ClusterElement(int id, int cn, RotatedRect rect, int[] profile)
        {
            Id = id;
            Element = rect;
            ClusterNo = cn;
            Profile = profile;
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
            var res = new ClusterElement(Id, ClusterNo,  tmpElement, Profile);

            return res;
        }
    }
}
