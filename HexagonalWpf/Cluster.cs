using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV.Structure;

namespace HexagonalWpf
{
    class Cluster
    {
        private readonly List<ClusterElement> f_Cluster;
        public string ClusterId { get; }

        public int Count => f_Cluster.Count;
        public Cluster(string fName)
        {
            f_Cluster = new List<ClusterElement>();
            ClusterId = fName;
        }

        public void Add(ClusterElement el)
        {
            f_Cluster.Add(el);
        }

        public IEnumerable<ClusterElement> GetList => f_Cluster;

        public int GetNearerId(RotatedRect el)
        {
            if (f_Cluster == null || f_Cluster.Count <= 0) return -1;
            var tmp = f_Cluster.OrderBy(x => x.Range(el));
            return f_Cluster.OrderBy(x => x.Range(el)).FirstOrDefault().Id;
        }

        public ClusterElement GetNearer(RotatedRect el)
        {
            if (f_Cluster == null || f_Cluster.Count <= 0) return new ClusterElement(-1, new RotatedRect());
           // var tmp = f_Cluster.OrderBy(x => x.Range(el));
            return f_Cluster.OrderBy(x => x.Range(el)).FirstOrDefault();
        }

        public List<ClusterElement> Get7(RotatedRect el)
        {
            if (f_Cluster == null || f_Cluster.Count <= 0) return null;
            return f_Cluster.OrderBy(x => x.Range(el)).Take(7).ToList();
        }

        public PointF GetCenter()
        {
            if (f_Cluster == null || f_Cluster.Count <= 0) return new PointF(0f, 0f);
            var cx = (f_Cluster.Min(x => x.Element.Center.X) + f_Cluster.Max(x => x.Element.Center.X)) / 2f;
            var cy = (f_Cluster.Min(x => x.Element.Center.Y) + f_Cluster.Max(x => x.Element.Center.Y)) / 2f;
            return new PointF(cx, cy);
        }

        public RelativePosition GetRelativePos(PointF position)
        {
            var minX = f_Cluster.Min(x => x.Element.Center.X);
            var maxX = f_Cluster.Max(x => x.Element.Center.X);
            var sizeX = maxX - minX;
            var minY = f_Cluster.Min(x => x.Element.Center.Y);
            var maxY = f_Cluster.Max(x => x.Element.Center.Y);
            var sizeY = maxY - minY;
            var center = GetCenter();
            var dx = position.X - center.X;
            var dy = position.Y - center.Y;
            return new RelativePosition { Rx = dx / sizeX, Ry = dy / sizeY };
        }

        public PointF RelativeToPos(RelativePosition position)
        {
            var minX = f_Cluster.Min(x => x.Element.Center.X);
            var maxX = f_Cluster.Max(x => x.Element.Center.X);
            var sizeX = maxX - minX;
            var minY = f_Cluster.Min(x => x.Element.Center.Y);
            var maxY = f_Cluster.Max(x => x.Element.Center.Y);
            var sizeY = maxY - minY;
            var center = GetCenter();
            return new PointF(
                (float)(center.X + position.Rx * sizeX),
                (float)(center.Y + position.Ry * sizeY)
                );

        }
    }
}
