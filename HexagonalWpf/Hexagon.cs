﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalWpf
{
    class Hexagon
    {
        private readonly ClusterElement f_Center;
        public ClusterElement Center => f_Center;
        private readonly List<ClusterElement> f_List;
        private readonly double f_AverageSize;
        public double AverageSize => f_AverageSize;
        public string FileName { get; set; }
        public IEnumerable<ClusterElement> HList => f_List;

        public Hexagon(ClusterElement center, List<ClusterElement> list, string fileName)
        {
            FileName = fileName;
            f_AverageSize = list.Average(x => x.Diametr); 
            f_Center = center;
            _ = list.Remove(center);
            f_List = new List<ClusterElement>();
            f_List.AddRange(list);
        }

        public double AverageLink()
        {
            var tmpList = new List<ClusterElement>();
            tmpList.AddRange(f_List);
            var resList = new List<double>();
            resList.AddRange(tmpList.Select(x => (double)x.Range(f_Center.Element)));
            var fst = tmpList.FirstOrDefault();
            var last = fst;
            tmpList.Remove(fst);
            while (tmpList.Count > 0)
            {
                var t = tmpList.OrderBy(x => x.Range(fst.Element)).FirstOrDefault();
                resList.Add(t.Range(fst.Element));
                fst = t;
                tmpList.Remove(fst);
            }
            resList.Add(last.Range(fst.Element));
            Debug.WriteLine($"Diametr: {f_AverageSize}, Distance: {resList.Average()}");
            return resList.Average();
        }


    }
}
