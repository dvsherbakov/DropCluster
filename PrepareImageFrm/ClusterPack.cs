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
using IronXL;

namespace PrepareImageFrm
{
    class ClusterPack
    {
        private readonly List<Cluster> f_Clusters;
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
            
            Mat res = new Mat(new Size(1000, 1000), DepthType.Cv8U, 3);
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

                var fName = f_PackId.Split('\\').LastOrDefault();
                var zm = ZoomKoef;
                WorkBook xlsWorkbook = WorkBook.Create(ExcelFileFormat.XLS);
                xlsWorkbook.Metadata.Author = "MetallHell";

                var dict = GetPackDict();

                foreach (var pair in dict)
                {
                    WorkSheet xlsSheet = xlsWorkbook.CreateWorkSheet(pair.Key.ToString());
                    var row = 2;
                    foreach (var d in pair.Value)
                    {
                        xlsSheet[$"B{row}"].Value = d.Center.X / zm;
                        xlsSheet[$"C{row}"].Value = d.Center.Y / zm;

                        xlsSheet[$"F{row}"].Value = ((d.Size.Width + d.Size.Height) / 2) / zm;

                        row++;
                    }

                    xlsWorkbook.SaveAs($"{fName}.xlsx");
                }
            });

        }
    }
}
