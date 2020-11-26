using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    class ClusterPack
    {
        private List<Cluster> f_Clusters;
        public string CurrentClusterId { get; private set; }
        public string PrevClusterId { get; private set; }

        public ClusterPack()
        {
            f_Clusters = new List<Cluster>();
            CurrentClusterId = "start";
        }

        public void CreateNewCluster(string clusterId)
        {
            PrevClusterId = CurrentClusterId;
            CurrentClusterId = clusterId;
            f_Clusters.Add(new Cluster(clusterId));
        }

        public void AddElementToCurrent(RotatedRect el)
        {
            if (f_Clusters == null) return;
            var id = CurrentClusterId == "start"
                ? f_Clusters.FirstOrDefault(x => x.ClusterId == "start").Count
                : f_Clusters.FirstOrDefault(x => x.ClusterId == PrevClusterId).GetNearerId(el);
            f_Clusters.FirstOrDefault(x => x.ClusterId == PrevClusterId)?.Add(new ClusterElement(id, el));
        }

    }
}
