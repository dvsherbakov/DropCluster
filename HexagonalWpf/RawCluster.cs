using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalWpf
{
    class RawCluster
    {
        private Cluster f_Cluster;
        private string f_FileName;
        public string FileName => f_FileName;

        public RawCluster(string fName)
        {
            f_FileName = fName;
            f_Cluster = new Cluster(fName);
        }
    }
}
