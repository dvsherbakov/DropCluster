using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Schema;
using Emgu.CV.Structure;

namespace HexagonalWpf
{
    internal class Cluster
    {
        private readonly List<ClusterElement> _cluster;
        public string ClusterId { get; }
        public PointF StartPosition { get; private set; }
        public PointF CenterPosition { get; private set; }
        public int Count => _cluster.Count;
        public Cluster(string fName)
        {
            _cluster = new List<ClusterElement>();
            ClusterId = fName;
        }

        public void Add(ClusterElement el)
        {
            _cluster.Add(el);
            GetStartPosition();
            GetCenterPosition();
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

        internal void RemoveById(int id)
        {
            _ = _cluster.Remove(_cluster.FirstOrDefault(x => x.Id == id));
        }

        public int GenerateNextId() => new HashSet<int>(_cluster.Select(x => x.Id)).Max() + 1;

        public void GetStartPosition()
        {
            var x = _cluster.Select(t => t.Element.Center.X).Min();
            var y = _cluster.Select(t => t.Element.Center.Y).Min();
            StartPosition = new PointF(x, y);
        }

        public void GetCenterPosition()
        {
            if (_cluster == null || _cluster.Count <= 0) return;
            var cx = (_cluster.Min(x => x.Element.Center.X) + _cluster.Max(x => x.Element.Center.X)) / 2f;
            var cy = (_cluster.Min(x => x.Element.Center.Y) + _cluster.Max(x => x.Element.Center.Y)) / 2f;
            CenterPosition = new PointF(cx, cy);
        }

        public PointF RelativeCenterPos(PointF pos)
        {
            return new PointF(pos.X - CenterPosition.X, pos.Y - CenterPosition.Y);
        }
    }
}
