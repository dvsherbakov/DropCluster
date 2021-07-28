using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalWpf
{
    internal class ClusterPack
    {
        public string Id { get; set; }
        private List<Cluster> _cluster;

        public ClusterPack(string path)
        {
            _cluster = new List<Cluster>();
        }

        public void Add(Cluster c)
        {
            _cluster.Add(c);
        }
    }
}
