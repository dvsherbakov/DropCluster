using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    class Cluster
    {
        private List<ClusterElement> f_Cluster;

        public Cluster()
        {
            this.f_Cluster =new List<ClusterElement>();
        }

        public void Add(ClusterElement el)
        {
            f_Cluster.Add(el);
        }

        public List<ClusterElement> GetList => f_Cluster;

        public int GetNearerId(RotatedRect el)
        {
            if (f_Cluster!=null && f_Cluster.Count > 0)
                return f_Cluster.OrderBy(x => x.Range(el)).FirstOrDefault().Id;
            return -1;
        }
    }
}
