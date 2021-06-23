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
        private long f_FileSize;
        private readonly int f_ObjectCount;
        public int Pass { get; private set; }
        public int[][] Brightness { get; }

        public float[] Distance => GetDistanceBeforeCenter();

        public bool IsCorrect => f_ObjectCount > 0 ? GetContours.Size == f_ObjectCount : GetContours.Size > 0;

        public ImageResult(string fileName, long fSize, VectorOfVectorOfPoint listOfContours, int[][] brightness, int objectCount)
        {
            FileName = fileName;
            f_FileSize = fSize;
            Pass = 1;
            f_ObjectCount = objectCount;
            GetContours = new VectorOfVectorOfPoint();
            Brightness = brightness;
            for (var i = 0; i < listOfContours.Size; i++)
            {
                GetContours.Push(listOfContours[i]);
            }
        }

        private PointF GetCenter(int i) => GetContours.Size < i ? new PointF() : CvInvoke.FitEllipse(GetContours[i]).Center;

        private string GetCenters()
        {
            if (GetContours.Size == 0) return "Not centers";
            var cycles = GetContours.Size <= 20 ? GetContours.Size : 20;
            var res = "Centers: ";
            for (var i = 0; i < cycles; i++)
            {
                res += $"{GetCenter(i).X}:{GetCenter(i).Y}:";
            }

            if (GetContours.Size <= 20) return res;
            else return res + " ...";
        }

        private SizeF GetSize(int i) => GetContours.Size < i ? new SizeF() : CvInvoke.FitEllipse(GetContours[i]).Size;

        private string GetSizes(double zm)
        {
            if (GetContours.Size == 0) return "Not sizes";
            var res = "Sizes: ";
            var cycles = GetContours.Size <= 20 ? GetContours.Size : 20;
            for (var i = 0; i < cycles; i++)
            {
                res += $"{GetSize(i).Width / zm}:{GetSize(i).Height / zm}:";
            }
            if (GetContours.Size <= 20) return res;
            else return res + " ...";
        }

        private double GetPerimeter(int i) => GetContours.Size < i ? 0 : CvInvoke.ArcLength(GetContours[i], true);

        private string GetPerimeters()
        {
            if (GetContours.Size == 0) return "Not perimeters";
            var res = "Perimeters: ";
            var cycles = GetContours.Size <= 20 ? GetContours.Size : 20;
            for (var i = 0; i < cycles; i++)
            {
                res += $"{GetPerimeter(i)}: ";
            }
            if (GetContours.Size <= 20) return res;
            else return res + " ...";
        }

        private float[] GetDistanceBeforeCenter()
        {
            var res = new List<float>();
            if (f_ObjectCount == 0) return new float[] { 0f };
            if (GetContours.Size != f_ObjectCount) res.Add(0f);
            if (f_ObjectCount == 2)
            {
                var rctA = CvInvoke.FitEllipse(GetContours[0]);
                var rctB = CvInvoke.FitEllipse(GetContours[1]);
                res.Add(GetDistance(rctA.Center, rctB.Center));
            }
            else
            {
                //получаем список объектов
                var objList = new List<Emgu.CV.Structure.RotatedRect>();
                for (var i = 0; i < GetContours.Size; i++)
                {
                    objList.Add(CvInvoke.FitEllipse(GetContours[i]));
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
            GetContours.Clear();
            for (var i = 0; i < newList.Size; i++)
            {
                GetContours.Push(newList[i]);
            }
            Pass++;
        }

        private static string GetDistanceString(IEnumerable<float> data, double zm = 1)
        {
            return data.Aggregate("", (current, t) => current + $"{t / zm}:");
        }

        public VectorOfVectorOfPoint GetContours { get; }

        public TreeNode GetResultNode()
        {
            var res = new TreeNode(Path.GetFileNameWithoutExtension(FileName))
            {
                Name = FileName ?? string.Empty,
                ForeColor = IsCorrect ? Color.DarkGreen : Color.OrangeRed
            };
            res.Nodes.Add($"Pass: {Pass}");
            if (GetContours.Size <= 0) return res;
            res.Nodes.Add(GetPerimeters());
            res.Nodes.Add(GetDistanceString(GetDistanceBeforeCenter()));
            res.Nodes.Add(GetCenters());
            res.Nodes.Add(GetSizes(1));
            res.Nodes.Add($"Count: {GetContours.Size}");
            return res;
        }

        private static double ZoomKoef(double zm) => (4.65 * zm + 5.9) / 305;

        public string ToString(double zm) => IsCorrect ? $"{Path.GetFileNameWithoutExtension(FileName)}:{Pass}:{GetDistanceString(GetDistanceBeforeCenter(), ZoomKoef(zm))}:{GetCenters()}:{GetSizes(ZoomKoef(zm))}"
                : $"{FileName}:No two contours";

        public static void SaveDetailFile(string fName)
        {

        }
    }
}