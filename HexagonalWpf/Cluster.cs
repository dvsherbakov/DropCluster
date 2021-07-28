using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV.Structure;

namespace HexagonalWpf
{
    internal class Cluster
    {
        private readonly List<ClusterElement> _cluster;
        public string ClusterId { get; }

        public int Count => _cluster.Count;
        public Cluster(string fName)
        {
            _cluster = new List<ClusterElement>();
            ClusterId = fName;
        }

        public void Add(ClusterElement el)
        {
            _cluster.Add(el);
        }

        public IEnumerable<ClusterElement> GetList => _cluster;

        public int GetNearerId(RotatedRect el)
        {
            if (_cluster == null || _cluster.Count <= 0) return -1;
            return _cluster.OrderBy(x => x.Range(el)).FirstOrDefault().Id;
        }

        public ClusterElement GetNearer(RotatedRect el)
        {
            if (_cluster == null || _cluster.Count <= 0) return new ClusterElement(-1, new RotatedRect());
           // var tmp = _cluster.OrderBy(x => x.Range(el));
            return _cluster.OrderBy(x => x.Range(el)).FirstOrDefault();
        }

        public List<ClusterElement> Get7(RotatedRect el)
        {
            if (_cluster == null || _cluster.Count <= 0) return null;
            return _cluster.OrderBy(x => x.Range(el)).Take(7).ToList();
        }

        public PointF GetCenter()
        {
            if (_cluster == null || _cluster.Count <= 0) return new PointF(0f, 0f);
            var cx = (_cluster.Min(x => x.Element.Center.X) + _cluster.Max(x => x.Element.Center.X)) / 2f;
            var cy = (_cluster.Min(x => x.Element.Center.Y) + _cluster.Max(x => x.Element.Center.Y)) / 2f;
            return new PointF(cx, cy);
        }

        public RelativePosition GetRelativePos(PointF position)
        {
            var minX = _cluster.Min(x => x.Element.Center.X);
            var maxX = _cluster.Max(x => x.Element.Center.X);
            var sizeX = maxX - minX;
            var minY = _cluster.Min(x => x.Element.Center.Y);
            var maxY = _cluster.Max(x => x.Element.Center.Y);
            var sizeY = maxY - minY;
            var center = GetCenter();
            var dx = position.X - center.X;
            var dy = position.Y - center.Y;
            return new RelativePosition { Rx = dx / sizeX, Ry = dy / sizeY };
        }

        public PointF RelativeToPos(RelativePosition position)
        {
            var minX = _cluster.Min(x => x.Element.Center.X);
            var maxX = _cluster.Max(x => x.Element.Center.X);
            var sizeX = maxX - minX;
            var minY = _cluster.Min(x => x.Element.Center.Y);
            var maxY = _cluster.Max(x => x.Element.Center.Y);
            var sizeY = maxY - minY;
            var center = GetCenter();
            return new PointF(
                (float)(center.X + position.Rx * sizeX),
                (float)(center.Y + position.Ry * sizeY)
                );

        }
    }
}
