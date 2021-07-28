using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HexagonalWpf
{
    internal class Hexagon
    {
        private readonly ClusterElement _center;
        public ClusterElement Center => _center;
        private readonly List<ClusterElement> _list;
        private readonly double _averageSize;
        public double AverageSize => _averageSize;
        public string FileName { get; set; }
        public IEnumerable<ClusterElement> HList => _list;

        public Hexagon(ClusterElement center, ICollection<ClusterElement> list, string fileName)
        {
            FileName = fileName;
            _averageSize = list.Average(x => x.Diameter); 
            _center = center;
            _ = list.Remove(center);
            _list = new List<ClusterElement>();
            _list.AddRange(list);
        }

        public double AverageLink()
        {
            var tmpList = new List<ClusterElement>();
            tmpList.AddRange(_list);
            var resList = new List<double>();
            resList.AddRange(tmpList.Select(x => (double)x.Range(_center.Element)));
            var fst = tmpList.FirstOrDefault();
            var last = fst;
            tmpList.Remove(fst);
            while (tmpList.Count > 0)
            {
                var t = tmpList.OrderBy(x => x.Range(fst.Element)).FirstOrDefault();
                resList.Add(t.Range(fst.Element));
                fst = t;
                _ = tmpList.Remove(fst);
            }
            resList.Add(last.Range(fst.Element));
            Debug.WriteLine($"Diameter: {_averageSize}, Distance: {resList.Average()}");
            return resList.Average();
        }


    }
}
