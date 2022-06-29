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
        public CustomFileName CustomName { get; }
        public PointF StartPosition { get; private set; }
        public PointF CenterPosition { get; private set; }

        public ClusterElement SelectedElement { get; set; }

        public PointF Correction { get; set; }

        public int Count => _cluster.Count;
        public Cluster(string fName)
        {
            _cluster = new List<ClusterElement>();
            SelectedElement = null;
            ClusterId = fName;
            CustomName = new CustomFileName(fName);
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
            return _cluster == null || _cluster.Count <= 0 ? -1 : _cluster.OrderBy(x => x.Range(el)).FirstOrDefault().Id;
        }

        public ClusterElement GetNearer(RotatedRect el)
        {
            if (_cluster == null || _cluster.Count <= 0) return new ClusterElement(-1, new RotatedRect());
            // var tmp = _cluster.OrderBy(x => x.Range(el));
            return _cluster.OrderBy(x => x.Range(el)).FirstOrDefault();
        }

        public int GetRelativeNearerId(PointF el)
        {
            if (_cluster == null || _cluster.Count <= 0) return -1;
            return _cluster.OrderBy(x => x.RangeCenter(el, Correction)).FirstOrDefault().Id;
        }

        public ClusterElement GetRelativeNearer(PointF el)
        {
            if (_cluster == null || _cluster.Count <= 0) return new ClusterElement(-1, new RotatedRect());
            return _cluster.OrderBy(x => x.RangeCenter(el, CenterPosition)).FirstOrDefault();
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

        public List<ClusterElement> GetCenterArrangedList()
        {
            return _cluster.OrderBy(x => x.RangeF(CenterPosition)).ToList();
        }

        public PointF RelativeCenterPos(PointF pos)
        {
            return new PointF(pos.X - Correction.X, pos.Y - Correction.Y);
        }

        public void SetCorrection(PointF c)
        {
            Correction = new PointF(CenterPosition.X - c.X, CenterPosition.Y - c.Y);
        }

        public int MaxId => _cluster.Select(x => x.Id).OrderByDescending(x => x).FirstOrDefault();

        public double AvgDiam => _cluster.Count > 0 ? _cluster.Average(x => x.Diameter) : 0;

        public double AvgDist()
        {
            var ranges = (from circle in _cluster from dCircle in _cluster where circle.Range(dCircle.Element) > 0.00001 select circle.Range(dCircle.Element)).ToList();

            return ranges.Count > 0 ? ranges.Average() : 0;
        }
    }
}
