using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    internal class Cluster
    {
        public readonly List<ClusterElement> ClusterList;
        public string ClusterId { get; }
        private readonly int _clusterNo = 0;
        public int ClusterNo => _clusterNo;
        public ClusterElement this[int index] => ClusterList[index];
        public ClusterRect Edges => GetEdges();
        public int Count => ClusterList.Count;
        
        public Cluster(string fName)
        {
            ClusterList = new List<ClusterElement>();
            ClusterId = fName;
             int.TryParse(new Regex(@"[^\d]").Replace(fName.Split('_').LastOrDefault() ?? string.Empty, string.Empty), out _clusterNo);
        }

        public void Add(ClusterElement el)
        {
            el.ClusterNo = ClusterNo;
            ClusterList.Add(el);
        }

        public void RemoveById(int id)
        {
            var can = ClusterList.FirstOrDefault(x => x.Id == id);
            if (can != null)
            {
                ClusterList.Remove(can);
            }
        }

        private ClusterRect GetEdges()
        {
            var x1 = ClusterList.Min(x => x.Element.Center.X);
            var x2 = ClusterList.Max(x => x.Element.Center.X);
            var y1 = ClusterList.Min(x => x.Element.Center.Y);
            var y2 = ClusterList.Max(x => x.Element.Center.Y);

            return new ClusterRect(x1, y1, x2, y2);
        }
        public IEnumerable<ClusterElement> GetList => ClusterList;



        public int GetNearerId(RotatedRect el)
        { 
            if (ClusterList == null || ClusterList.Count <= 0) return -1;
            var tmp = ClusterList.OrderBy(x => x.Range(el));
            return ClusterList.OrderBy(x => x.Range(el)).FirstOrDefault().Id;
        }

        public int GetRelativeNearerId(ClusterElement relateElement)
        {
            if (ClusterList == null || ClusterList.Count <= 0) return -1;
            return ClusterList.OrderBy(x => x.GetRelativeElement(Edges).Range(relateElement.Element)).FirstOrDefault().Id;
        }

        public int GenerateNextId() => new HashSet<int>(ClusterList.Select(x=>x.Id)).Max()+1;

    }
}
