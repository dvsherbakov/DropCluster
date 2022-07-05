using System;
using OfficeOpenXml;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emgu.CV.Structure;

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
            var prev = new Cluster(_clusters.ToList()[id].ClusterId);
            foreach (var e in _clusters.ToList()[id].GetList)
            {
                prev.Add(e);
            }

            return prev;
        }

        public void Renumbering()
        {
            if (_clusters.Count < 2)
            {
                return;
            }

            for (var i = 1; i < _clusters.Count; i++)
            {
                var prev = CloneCluster(i - 1);

                prev.SetCorrection(_clusters[i].CenterPosition);


                foreach (var elem in _clusters[i].GetList.OrderBy(x => x.Element.Center.Y)
                             .ThenBy(y => y.Element.Center.X))
                {
                    //elem.Id = prev.Count > 0 ? prev.GetNearerId(elem.Element) : _clusters[i].GenerateNextId();
                    //var a = prev.GetRelativeNearerId(elem.Element.Center);
                    //var c = prev.GetList.FirstOrDefault(x => x.Id == a);
                    //Debug.WriteLine($"{elem.Element.Center.X}-{elem.Element.Center.Y}  |  {c.Element.Center.X}-{c.Element.Center.Y} ");
                    elem.Id = prev.Count > 0
                        ? prev.GetRelativeNearerId(elem.Element.Center)
                        : _clusters[i].GenerateNextId();
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

        public ClusterElement SetCurrentSelected(RotatedRect el)
        {
            if (_clusters.Count <= 0 || CurrentId == "") return null;
            var index = _clusters.IndexOf(_clusters.FirstOrDefault(x => x.ClusterId == CurrentId));
            var elem = _clusters[index].GetNearer(el);
            _clusters[index].SelectedElement = elem;
            return elem;
        }

        public ClusterElement GetCurrentSelected()
        {
            if (_clusters.Count <= 0 || CurrentId == "") return null;
            return _clusters.FirstOrDefault(x => x.ClusterId == CurrentId).SelectedElement;
        }

        public void FindPastSelected()
        {
            var first = _clusters.IndexOf(_clusters.FirstOrDefault(x => x.SelectedElement != null));

            for (var i = first; i < _clusters.Count - 1; i++)
            {
                //var nearer = 
                if (_clusters[i].SelectedElement != null)
                    _clusters[i + 1].SelectedElement = _clusters[i + 1].GetList
                        .OrderBy(x => x.Range(_clusters[i].SelectedElement.Element))
                        .FirstOrDefault();
                //_clusters[i + 1].SelectedElement = nearer.OrderBy(x =>
                //        Math.Abs((int)x.Shear.AvgBrightest() - _clusters[i].SelectedElement.Shear.AvgBrightest()))
                //    .FirstOrDefault();
            }
        }

        private int MaxId => _clusters.Select(x => x.MaxId).OrderByDescending(x => x).FirstOrDefault();

        public async Task SaveShearInfo()
        {
            await Task.Run(() =>
            {
                var fileName = Path.GetDirectoryName(Id) + "\\outShear.xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                var file = new FileInfo(fileName);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file))
                {
                    foreach (var cluster in _clusters)
                    {
                        var xlsSheet = package.Workbook.Worksheets.Add(
                            Regex.Replace(Path.GetFileNameWithoutExtension(cluster.ClusterId), "[ _-]", string.Empty));
                        var column = 5;
                        foreach (var item in cluster.GetList.OrderBy(x => x.Id))
                        {
                            xlsSheet.Cells[4, column].Value = item.Id;
                            var profile = item.Shear.GetProfile();
                            for (var i = 0; i < profile.Length; i++)
                            {
                                xlsSheet.Cells[6 + i, column].Value = profile[i];
                            }

                            column++;
                        }
                    }

                    package.Save();
                }
            });
        }

        public void saveOneDropHaul()
        {
            var first = _clusters.IndexOf(_clusters.FirstOrDefault(x => x.SelectedElement != null));
            var dropIndex = _clusters[first].SelectedElement.Id;
            var fileName = Path.GetDirectoryName(Id) + "\\oneDrop.xlsx";

            var file = new FileInfo(fileName);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(file))
            {
                var xlsSheet = package.Workbook.Worksheets.Add($"drop{dropIndex}");

                xlsSheet.Cells[2, 5].Value = "#";
                xlsSheet.Cells[2, 7].Value = "Центральная область";
                xlsSheet.Cells[2, 8].Value = "Среднее";
                xlsSheet.Cells[2, 9].Value = "Диаметр";
                xlsSheet.Cells[2, 10].Value = "Диаметр свечения";

                var row = 4;
                for (var i = first; i < _clusters.Count; i++)
                {
                    if (_clusters[i].SelectedElement == null) continue;
                    xlsSheet.Cells[row, 5].Value = new CustomFileName(_clusters[i].ClusterId).number;
                    xlsSheet.Cells[row, 7].Value = _clusters[i].SelectedElement.Shear.GetAvgCenterSpot();
                    xlsSheet.Cells[row, 8].Value = _clusters[i].SelectedElement.Shear.AvgBrightest();
                    xlsSheet.Cells[row, 9].Value = _clusters[i].SelectedElement.Diameter;
                    xlsSheet.Cells[row, 10].Value = _clusters[i].SelectedElement.Shear.AvgDiam*2;
                    row++;
                }

                package.Save();
            }
        }

        public async void SaveBrightestSpot()
        {
            await Task.Run(() =>
            {
                var fileName = Path.GetDirectoryName(Id) + "\\outBrightestSpot.xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                var file = new FileInfo(fileName);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file))
                {
                    var xlsSheet = package.Workbook.Worksheets.Add("bg");
                    var row = 4;
                    foreach (var cluster in _clusters.OrderBy(x => x.CustomName.number))
                    {
                        xlsSheet.Cells[row, 4].Value = cluster.CustomName.number;

                        foreach (var item in cluster.GetList.OrderBy(x => x.Id))
                        {
                            var lv = item.Shear.AvgDiam;
                            xlsSheet.Cells[row, item.Id + 6].Value = item.Shear.GetAvgCenterSpot();
                        }

                        row++;
                    }

                    package.Save();
                }
            });
        }

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

                const double zm = 0.8529; //ZoomKoef;
                var file = new FileInfo(fileName);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(file))
                {
                    var clusterNo = 1;

                    var xlsDistribution = package.Workbook.Worksheets.Add(Id);

                    foreach (var it in _clusters.OrderBy(x => x.CustomName.number))
                    {
                        xlsDistribution.Cells[clusterNo + 1, 1].Value = (float)clusterNo / 2;

                        for (var i = 0; i <= MaxId; i++)
                        {
                            if (it.GetList.Count(x => x.Id == i) <= 0) continue;

                            var drop = it.GetList.FirstOrDefault(x => x.Id == i);

                            if (drop != null)
                                xlsDistribution.Cells[clusterNo + 1, i + 3].Value = drop.Diameter / zm; //ZoomKoef;
                        }

                        clusterNo++;
                    }

                    package.Save();
                }
            });
        }

        public void SaveAvgDiameters()
        {
            var fileName = Path.GetDirectoryName(Id) + "\\out.xlsx";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            const double zm = 0.8529;
            var file = new FileInfo(fileName);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file))
            {
                var clusterNo = 1;

                var xlsDistribution = package.Workbook.Worksheets.Add(Id);

                foreach (var it in _clusters.OrderBy(x => x.CustomName.number))
                {
                    xlsDistribution.Cells[clusterNo + 1, 1].Value = (float)clusterNo / 2;
                    xlsDistribution.Cells[clusterNo + 1, 3].Value = it.AvgDiam / zm;
                    //xlsDistribution.Cells[clusterNo + 1, 5].Value = it.AvgDist() / zm;
                    xlsDistribution.Cells[clusterNo + 1, 5].Value = it.Count;
                    clusterNo++;
                }

                package.Save();
            }
        }

        public void Clear()
        {
            _clusters.Clear();
        }

        public int Count => _clusters.Count;

        public string GetCurrentNumberId()
        {
            var custom = new CustomFileName(CurrentId);
            return custom.number.ToString();
        }
    }
}