using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    internal class Cluster
    {
        private readonly List<ClusterElement> f_Cluster;
        public string ClusterId { get; }
        public ClusterElement this[int index] => f_Cluster[index];
        public ClusterRect Edges { get => GetEdges(); }
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

        private ClusterRect GetEdges()
        {
            var x1 = f_Cluster.Min(x => x.Element.Center.X);
            var x2 = f_Cluster.Max(x => x.Element.Center.X);
            var y1 = f_Cluster.Min(x => x.Element.Center.Y);
            var y2 = f_Cluster.Max(x => x.Element.Center.Y);

            return new ClusterRect(x1, y1, x2, y2);
        }
        public IEnumerable<ClusterElement> GetList => f_Cluster;



        public int GetNearerId(RotatedRect el)
        {
            if (f_Cluster == null || f_Cluster.Count <= 0) return -1;
            var tmp = f_Cluster.OrderBy(x => x.Range(el));
            return f_Cluster.OrderBy(x => x.Range(el)).FirstOrDefault().Id;
        }

        public int GetRelativeNearerId(ClusterElement relateElement)
        {
            if (f_Cluster == null || f_Cluster.Count <= 0) return -1;
            return f_Cluster.OrderBy(x => x.GetRelativeElement(Edges).Range(relateElement.Element)).FirstOrDefault().Id;
        }

    }
}
