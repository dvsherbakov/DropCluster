using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using OfficeOpenXml;

namespace PrepareImageFrm
{
    internal class ClusterPack
    {
        private readonly List<Cluster> _clusters;
        public Cluster this[int index] => _clusters[index];
        private string _packId;
        public static int Zoom;
        public string CurrentClusterId { get; private set; }
        public string PrevClusterId { get; private set; }
        private static double ZoomKoef => (4.65 * Zoom + 5.9) / 305;

        public ClusterPack(int zoom)
        {
            _clusters = new List<Cluster>();
            CurrentClusterId = "start";
            Zoom = zoom;
        }

        public void CreateNewCluster(string clusterId)
        {
            _packId = Path.GetDirectoryName(clusterId);
            PrevClusterId = CurrentClusterId;
            CurrentClusterId = Path.GetFileNameWithoutExtension(clusterId);
            _clusters.Add(new Cluster(CurrentClusterId));
        }

        public void AddElementToCurrent(RotatedRect el, int[] profile)
        {
            if (_clusters == null) return;
            var id = PrevClusterId == "start"
                ? _clusters.FirstOrDefault(x => x.ClusterId == CurrentClusterId).Count
                : _clusters.FirstOrDefault(x => x.ClusterId == PrevClusterId).GetNearerId(el);
            _clusters.FirstOrDefault(x => x.ClusterId == CurrentClusterId)?.Add(new ClusterElement(id, el, profile));
        }

        public void Clear()
        {
            _clusters.Clear();
            CurrentClusterId = "start";
        }

        private int GetMaxDropCount()
        {
            return _clusters.Select(itm => itm.Count).Prepend(0).Max();
        }

        private Color[] GenerateColorTable()
        {
            var rand = new Random();
            var res = new List<Color>();
            for (var i = 0; i <= GetMaxDropCount(); i++)
            {
                var red = rand.Next(255);
                var green = rand.Next(255);
                var blue = rand.Next(255);
                var color = Color.FromArgb(1, red, green, blue);
                res.Add(color);
            }
            return res.ToArray();
        }

        public Mat Trajectories()
        {

            var res = new Mat(new Size(1000, 1000), DepthType.Cv8U, 3);
            res.SetTo(new MCvScalar(0));
            var colors = GenerateColorTable();

            var pList = new List<List<PointF>>();
            for (var i = 0; i < GetMaxDropCount(); i++)
            {
                var tmp = new List<PointF>();
                foreach (var it in _clusters)
                {
                    tmp.Add(it.GetList.FirstOrDefault(x => x.Id == i).Element.Center);
                }
                pList.Add(tmp);
            }

            var idx = 0;
            foreach (var lst in pList)
            {
                CvInvoke.Polylines(res, Array.ConvertAll(lst.ToArray(), Point.Round), false, new Bgr(colors[idx]).MCvScalar, 2);
                idx++;
            }
            return res;
        }

        private Dictionary<int, List<RotatedRect>> GetPackDict()
        {
            var dict = new Dictionary<int, List<RotatedRect>>();
            for (var i = 0; i < GetMaxDropCount(); i++)
            {
                var tmp = _clusters.Select(it => it.GetList.FirstOrDefault(x => x.Id == i).Element).ToList();
                dict.Add(i, tmp);
            }

            return dict;
        }

        private void ReOrderPack()
        {
            for (var i = 1; i < _clusters.Count; i++)
            {
                var prev = new Cluster("Unknown");
                foreach (var tmp in _clusters[i - 1].GetList)
                {
                    prev.Add(tmp);
                };
                //var rect = _clusters[i].Edges;
                foreach (var elem in _clusters[i].GetList)
                {
                    //var relative = elem.GetRelativeElement(rect);
                    elem.Id = prev.Count > 0 ? prev.GetNearerId(elem.Element) : _clusters[i].GenerateNextId();
                    prev.RemoveById(elem.Id);
                }
            }
        }

        public async Task SaveDetailInfo()
        {
            await Task.Run(() =>
            {
                ReOrderPack();
                //var tmp = _clusters[0].GetList;
                var linearList = new List<ClusterElement>();
                foreach (var c in _clusters)
                {
                    foreach (var it in c.ClusterList)
                    {
                        linearList.Add(it);
                    }
                }

                var fileName = _packId.Split('\\').LastOrDefault() + ".xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                const double zm = 0.8529; //ZoomKoef;
                var file = new FileInfo(fileName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var numbers = new HashSet<ushort>();

                foreach (var item in _clusters.SelectMany(cluster => cluster.GetList))
                {
                    numbers.Add((ushort)item.Id);
                }

                using (var package = new ExcelPackage(file))
                {
                    var xlsDistribution = package.Workbook.Worksheets.Add("Distribution");
                    var xlsSizes = package.Workbook.Worksheets.Add("Sizes");

                    var dColumn = 3;
                    for (var ds = Math.Ceiling(linearList.Max(x => x.Diam));
                        ds > linearList.Min(x => x.Diam);
                        ds -= 0.5)
                    {
                        xlsDistribution.Cells[2, dColumn].Value = ds;
                        xlsDistribution.Cells[3, dColumn].Value =
                            linearList.Count(x => x.Diam < ds && x.Diam > ds - 0.5);
                        dColumn++;
                    }

                    dColumn = 3;
                    for (var br = Math.Ceiling(linearList.Max(x => x.AverageBrightness));
                        br > linearList.Min(x => x.AverageBrightness);
                        br -= 1000)
                    {
                        xlsDistribution.Cells[7, dColumn].Value = br;
                        xlsDistribution.Cells[8, dColumn].Value = linearList.Count(x => x.AverageBrightness < br && x.AverageBrightness > br - 1000);
                        dColumn++;
                    }

                    var row = 12;

                    //foreach (var item in linearList.Where(x=>x.Diam>38&&x.Diam<39&&x.AverageBrightness<27000&&x.AverageBrightness>25000).OrderBy(y=>y.AverageBrightness))
                    foreach (var item in linearList.Where(x=>x.AverageBrightness < 27950 && x.AverageBrightness > 27700).OrderBy(x=>x.Diam))
                    {
                        xlsDistribution.Cells[row, 2].Value = item.ClusterNo;
                        xlsDistribution.Cells[row, 3].Value = item.Id;
                        xlsDistribution.Cells[row, 4].Value = item.Element.Center.X;
                        xlsDistribution.Cells[row, 5].Value = item.Element.Center.Y;
                        xlsDistribution.Cells[row, 6].Value = item.AverageBrightness;
                        xlsDistribution.Cells[row, 7].Value = item.Diam / zm;

                        for (var q = 0; q < item.Profile.Length; q++)
                        {
                            xlsDistribution.Cells[row, 9 + q].Value = item.Profile[q];
                        }

                        row++;
                    }
                    //foreach (var cluster in _clusters)
                    //{
                    //    var col = 3;
                    //    foreach (var c in numbers)
                    //    {
                    //        xlsSizes.Cells[1, col].Value = c;
                    //        if (cluster.GetList.Count(y => y.Id == c) > 0)
                    //        {
                    //            var candidate = cluster.GetList.FirstOrDefault(x => x.Id == c);
                    //            if (candidate != null)
                    //                xlsSizes.Cells[row, col].Value =
                    //                    ((candidate.Element.Size.Width + candidate.Element.Size.Height) / 2) / zm;
                    //        }
                    //        col++;
                    //    }

                    //    var xlsSheet = package.Workbook.Worksheets.Add(cluster.ClusterId);

                    //    xlsSizes.Cells[row, 2].Value = cluster.ClusterId;

                    //    var rowId = 2;
                    //    foreach (var item in cluster.GetList.OrderByDescending(x => (x.Element.Size.Width + x.Element.Size.Height )))
                    //    {

                    //        xlsSheet.Cells[rowId, 2].Value = item.Id;
                    //        xlsSheet.Cells[rowId, 3].Value = item.Element.Center.X;
                    //        xlsSheet.Cells[rowId, 4].Value = item.Element.Center.Y;
                    //        xlsSheet.Cells[rowId, 5].Value = ((item.Element.Size.Width + item.Element.Size.Height) / 2) / zm;


                    //        xlsSheet.Cells[rowId, 8].Value = (item.Element.Size.Width + item.Element.Size.Height) / 2;

                    //        for (var p = 0; p < item.Profile.Length; p++)
                    //        {
                    //            xlsSheet.Cells[rowId, 10 + p].Value = item.Profile[p];
                    //        }
                    //        rowId++;
                    //    }

                    //    row++;
                    //}
                    package.Save();
                }
            });
        }

        public async Task SaveExcelFile()
        {
            await Task.Run(() =>
           {
               var fileName = _packId.Split('\\').LastOrDefault() + ".xlsx";
               var zm = ZoomKoef;
               var file = new FileInfo(fileName);
               ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

               using (var package = new ExcelPackage(file))
               {
                   //var dict = GetPackDict();

                   foreach (var cluster in _clusters)
                   {
                       var xlsSheet = package.Workbook.Worksheets.Add(cluster.ClusterId);
                       var row = 2;
                       foreach (var item in cluster.GetList.OrderBy(x => x.Id))
                       {
                           xlsSheet.Cells[$"B{row}"].Value = item.Id;
                           xlsSheet.Cells[$"C{row}"].Value = item.Element.Center.X / zm;
                           xlsSheet.Cells[$"D{row}"].Value = item.Element.Center.Y / zm;

                           xlsSheet.Cells[$"F{row}"].Value = ((item.Element.Size.Width + item.Element.Size.Height) / 2) / zm;

                           row++;
                       }
                   }
                   package.Save();
               }
           });

        }
    }
}
