using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    class Cluster
    {
        private List<ClusterElement> f_Cluster;
        private readonly string f_FileName;
        public string ClusterId => f_FileName;
        public int Count => f_Cluster.Count;
        public Cluster(string fName)
        {
            this.f_Cluster =new List<ClusterElement>();
            f_FileName = fName;
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
