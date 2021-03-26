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
    class ClusterPack
    {
        private readonly List<Cluster> f_Clusters;
        public Cluster this[int index] => f_Clusters[index];
        private string f_PackId;
        public static int f_Zoom;
        public string CurrentClusterId { get; private set; }
        public string PrevClusterId { get; private set; }
        private static double ZoomKoef => (4.65 * f_Zoom + 5.9) / 305;

        public ClusterPack(int zoom)
        {
            f_Clusters = new List<Cluster>();
            CurrentClusterId = "start";
            f_Zoom = zoom;
        }

        public void CreateNewCluster(string clusterId)
        {
            f_PackId = Path.GetDirectoryName(clusterId);
            PrevClusterId = CurrentClusterId;
            CurrentClusterId = Path.GetFileNameWithoutExtension(clusterId);
            f_Clusters.Add(new Cluster(CurrentClusterId));
        }

        public void AddElementToCurrent(RotatedRect el)
        {
            if (f_Clusters == null) return;
            var id = PrevClusterId == "start"
                ? f_Clusters.FirstOrDefault(x => x.ClusterId == CurrentClusterId).Count
                : f_Clusters.FirstOrDefault(x => x.ClusterId == PrevClusterId).GetNearerId(el);
            f_Clusters.FirstOrDefault(x => x.ClusterId == CurrentClusterId)?.Add(new ClusterElement(id, el));
        }

        public void Clear()
        {
            f_Clusters.Clear();
            CurrentClusterId = "start";
        }

        private int GetMaxDropCount()
        {
            var res = 0;
            foreach (var itm in f_Clusters)
            {
                if (res < itm.Count) res = itm.Count;
            }

            return res;
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
                Color color = Color.FromArgb(1, red, green, blue);
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
                foreach (var it in f_Clusters)
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
                var tmp = new List<RotatedRect>();
                foreach (var it in f_Clusters)
                {
                    tmp.Add(it.GetList.FirstOrDefault(x => x.Id == i).Element);
                }
                dict.Add(i, tmp);
            }

            return dict;
        }

        public async Task SaveExcelFile()
        {
            await Task.Run(() =>
           {
               string fileName = f_PackId.Split('\\').LastOrDefault() + ".xlsx";
               var zm = ZoomKoef;
               FileInfo file = new FileInfo(fileName);
               ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

               using (var package = new ExcelPackage(file))
               {
                   var dict = GetPackDict();

                   foreach (var cluster in f_Clusters)
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
