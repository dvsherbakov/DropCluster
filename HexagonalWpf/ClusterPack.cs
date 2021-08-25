﻿using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HexagonalWpf
{
    internal class ClusterPack
    {
        public string Id { get; set; }
        public string CurrentId { get; set; }
        private readonly List<Cluster> _clusters;

        private static double ZoomKoef => (4.65 * 112 + 5.9) / 305;

        public ClusterPack(string path)
        {
            Id = path;
            _clusters = new List<Cluster>();
        }

        public void Add(Cluster c)
        {
            _clusters.Add(c);
            CurrentId = c.ClusterId;
        }

        private Cluster CloneCluster(int id)
        {
            var prev = new Cluster(_clusters.OrderBy(x => x.ClusterId).ToList()[id].ClusterId);
            foreach (var e in _clusters.OrderBy(x => x.ClusterId).ToList()[id].GetList)
            {
                prev.Add(e);
            }

            return prev;
        }

        public void Renumbering()
        {
            if (_clusters.Count < 2) return;

            for (var i = 1; i < _clusters.Count; i++)
            {
                var prev = CloneCluster(i - 1);

                foreach (var elem in _clusters[i].GetList)
                {
                    //elem.Id = prev.Count > 0 ? prev.GetNearerId(elem.Element) : _clusters[i].GenerateNextId();
                    elem.Id = prev.Count > 0 ? prev.GetNearerId(elem.Element) : _clusters[i].GenerateNextId();
                    prev.RemoveById(elem.Id);
                }
            }
        }

        public Cluster GetById(string id)
        {
            return _clusters.FirstOrDefault(x => x.ClusterId == id);
        }


        public Cluster PrevById(string id)
        {
            var index = _clusters.FindIndex(x => x.ClusterId == id);
            if (index > 0) index--;
            CurrentId = _clusters[index].ClusterId;
            return _clusters[index];
        }

        public Cluster NextById(string id)
        {
            var index = _clusters.FindIndex(x => x.ClusterId == id);
            if (index < _clusters.Count - 1) index++;
            CurrentId = _clusters[index].ClusterId;
            return _clusters[index];
        }

        public Cluster First()
        {
            CurrentId = _clusters[0].ClusterId;
            return _clusters[0];
        }

        private int MaxId => _clusters.Select(x => x.MaxId).OrderByDescending(x => x).FirstOrDefault();

        public async void SaveResult()
        {
            var startName = Id;
            await Task.Run(() =>
            {
                var fileName = Path.GetDirectoryName(startName) + "\\out.xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                var zm = ZoomKoef;
                var file = new FileInfo(fileName);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(file))
                {
                    var clusterNo = 1;

                    var xlsDistribution = package.Workbook.Worksheets.Add(Id);

                    foreach (var it in _clusters.OrderBy(x => x.ClusterId))
                    {
                        xlsDistribution.Cells[clusterNo + 1, 1].Value = (float)clusterNo / 2;

                        for (var i = 0; i <= MaxId; i++)
                        {
                            if (it.GetList.Count(x => x.Id == i) <= 0) continue;

                            var drop = it.GetList.FirstOrDefault(x => x.Id == i);

                            if (drop != null) xlsDistribution.Cells[clusterNo + 1, i + 3].Value = drop.Diameter / ZoomKoef;

                        }

                        clusterNo++;
                    }

                    package.Save();
                }
            });

        }

        public void Clear()
        {
            _clusters.Clear();
        }

        public int Count => _clusters.Count;
    }
}
