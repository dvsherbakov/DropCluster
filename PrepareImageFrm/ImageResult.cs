using Emgu.CV;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PrepareImageFrm
{
    public class ImageResult
    {
        public string FileName { get; }
        private int f_ObjectCount { get; }
        private readonly VectorOfVectorOfPoint f_Contours;
        public int Pass { get; private set; }
        private int[][] f_Bribrightness { get; set; }
        public float[] Distance => GetDistanceBeforeCenter();

        public bool IsCorrect => f_ObjectCount > 0 ? f_Contours.Size == f_ObjectCount : f_Contours.Size > 0;

        public ImageResult(string fileName, VectorOfVectorOfPoint listOfContours, int[][] bribrightness, int objectCount)
        {
            FileName = fileName;
            Pass = 1;
            f_ObjectCount = objectCount;
            f_Contours = new VectorOfVectorOfPoint();
            f_Bribrightness = bribrightness;
            for (var i = 0; i < listOfContours.Size; i++)
            {
                f_Contours.Push(listOfContours[i]);
            }
        }

        private PointF GetCenter(int i) => f_Contours.Size < i ? new PointF() : CvInvoke.FitEllipse(f_Contours[i]).Center;

        private string GetСenters()
        {
            if (f_Contours.Size == 0) return "Not centers";
            var cycles = f_Contours.Size <= 20 ? f_Contours.Size : 20;
            var res = "Centers: ";
            for (var i = 0; i < cycles; i++)
            {
                res += $"{GetCenter(i).X}:{GetCenter(i).Y}:";
            }

            if (f_Contours.Size <= 20) return res;
            else return res + " ...";
        }

        private SizeF GetSize(int i) => f_Contours.Size < i ? new SizeF() : CvInvoke.FitEllipse(f_Contours[i]).Size;

        private string GetSizes(double zm)
        {
            if (f_Contours.Size == 0) return "Not sizes";
            var res = "Sizes: ";
            var cycles = f_Contours.Size <= 20 ? f_Contours.Size : 20;
            for (var i = 0; i < cycles; i++)
            {
                res += $"{GetSize(i).Width / zm}:{GetSize(i).Height / zm}:";
            }
            if (f_Contours.Size <= 20) return res;
            else return res + " ...";
        }

        private double GetPerimeter(int i) => f_Contours.Size < i ? 0 : CvInvoke.ArcLength(f_Contours[i], true);

        private string GetPerimeters()
        {
            if (f_Contours.Size == 0) return "Not perimeters";
            var res = "Perimeters: ";
            var cycles = f_Contours.Size <= 20 ? f_Contours.Size : 20;
            for (var i = 0; i < cycles; i++)
            {
                res += $"{GetPerimeter(i)}: ";
            }
            if (f_Contours.Size <= 20) return res;
            else return res + " ...";
        }

        private float[] GetDistanceBeforeCenter()
        {
            var res = new List<float>();
            if (f_ObjectCount == 0) return new float[] { 0f };
            if (f_Contours.Size != f_ObjectCount) res.Add(0f);
            if (f_ObjectCount == 2)
            {
                var rctA = CvInvoke.FitEllipse(f_Contours[0]);
                var rctB = CvInvoke.FitEllipse(f_Contours[1]);
                res.Add(GetDistance(rctA.Center, rctB.Center));
            }
            else
            {
                //получаем список объектов
                var objList = new List<Emgu.CV.Structure.RotatedRect>();
                for (int i = 0; i < f_Contours.Size; i++)
                {
                    objList.Add(CvInvoke.FitEllipse(f_Contours[i]));
                }
                //Берем пары объектов с похожими X координатами для определения расстояния
                while (objList.Count >= 1)
                {
                    var fst = objList[0]; objList.Remove(fst);
                    var scd = objList.OrderBy(x => Math.Abs(x.Center.X - fst.Center.X)).FirstOrDefault();
                    objList.Remove(scd);
                    res.Add(GetDistance(fst.Center, scd.Center));
                }
            }
            return res.ToArray();
        }

        private static float GetDistance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public void UpdateContours(VectorOfVectorOfPoint newList)
        {
            f_Contours.Clear();
            for (var i = 0; i < newList.Size; i++)
            {
                f_Contours.Push(newList[i]);
            }
            Pass++;
        }

        private string GetDistanceString(float[] data, double zm = 1)
        {
            var res = "";
            for (int i = 0; i < data.Length; i++)
            {
                res += $"{data[i] / zm}:";
            }
            return res;
        }

        public VectorOfVectorOfPoint GetContours => f_Contours;

        public TreeNode GetResultNode()
        {
            var res = new TreeNode(Path.GetFileNameWithoutExtension(FileName))
            {
                Name = FileName,
                ForeColor = IsCorrect ? Color.DarkGreen : Color.OrangeRed
            };
            res.Nodes.Add($"Pass: {Pass}");
            if (f_Contours.Size <= 0) return res;
            res.Nodes.Add(GetPerimeters());
            res.Nodes.Add(GetDistanceString(GetDistanceBeforeCenter()));
            res.Nodes.Add(GetСenters());
            res.Nodes.Add(GetSizes(1));
            res.Nodes.Add($"Count: {f_Contours.Size}");
            return res;
        }

        private double ZoomKoef(double zm) => (4.65 * zm + 5.9) / 305;

        public string ToString(double zm) => IsCorrect ? $"{Path.GetFileNameWithoutExtension(FileName)}:{Pass}:{GetDistanceString(GetDistanceBeforeCenter(), ZoomKoef(zm))}:{GetСenters()}:{GetSizes(ZoomKoef(zm))}"
                : $"{FileName}:No two contours";

        public List<String> GetDetailList()
        {
            var res = new List<string>();
            var count = Math.Min(f_Contours.Size, f_Bribrightness.Length);
            for (var i = 0; i < count; i++)
            {
                var rct = CvInvoke.FitEllipse(f_Contours[i]);
                var diametr = (rct.Size.Width + rct.Size.Height) / 2;
                var br_string = "";
                for (var j = 0; j < f_Bribrightness[i].Length; j++)
                {
                    br_string += $":{f_Bribrightness[i][j]}";
                }
                var tmpStr = $"{(int)rct.Center.X}:{(int)rct.Center.Y}:{diametr}:{br_string}";
                res.Add(tmpStr);
            }
            return res;
        }

        public void SaveDetailFile(string fileName)
        {
            var list = GetDetailList();
            using (TextWriter tw = new StreamWriter(Path.GetFileNameWithoutExtension(fileName) + ".csv"))
            {
                foreach (string s in list)
                    tw.WriteLine(s);
            }
        }
    }
}