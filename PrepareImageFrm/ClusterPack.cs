using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

        public void AddElementToCurrent(RotatedRect el)
        {
            if (_clusters == null) return;
            var id = PrevClusterId == "start"
                ? _clusters.FirstOrDefault(x => x.ClusterId == CurrentClusterId).Count
                : _clusters.FirstOrDefault(x => x.ClusterId == PrevClusterId).GetNearerId(el);
            _clusters.FirstOrDefault(x => x.ClusterId == CurrentClusterId)?.Add(new ClusterElement(id, el));
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
                var tmp = _clusters.Select(it => it.GetList.FirstOrDefault(x => x.Id == i).Element.Center).ToList();
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
