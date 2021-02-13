using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    internal class Cluster
    {
        private readonly List<ClusterElement> f_Cluster;
        public string ClusterId { get; }

        public int Count => f_Cluster.Count;
        public Cluster(string fName)
        {
            f_Cluster =new List<ClusterElement>();
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

    }
}
