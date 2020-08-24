using Emgu.CV;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace PrepareImageFrm
{
    public class ImageResult
    {
        public string FileName { get; }
        private readonly VectorOfVectorOfPoint f_Contours;
        public int Pass { get; private set; }
        public float Distance => GetDistanceBeforeCenter();

        public bool IsCorrect => f_Contours.Size == 2;

        public ImageResult(string fileName, VectorOfVectorOfPoint listOfContours)
        {
            FileName = fileName;
            Pass = 1;
            f_Contours = new VectorOfVectorOfPoint();
            for (var i = 0; i < listOfContours.Size; i++)
            {
                f_Contours.Push(listOfContours[i]);
            }
        }

        private PointF GetCenter(int i) => f_Contours.Size < i ? new PointF() : CvInvoke.FitEllipse(f_Contours[i]).Center;

        private string GetСenters()
        {
            if (f_Contours.Size == 0) return "Not centers";
            var res = "Centers: ";
            for (var i = 0; i < f_Contours.Size; i++)
            {
                res += $"{GetCenter(i).X}:{GetCenter(i).Y}:";
            }
            return res;
        }

        private SizeF GetSize(int i) => f_Contours.Size < i ? new SizeF() : CvInvoke.FitEllipse(f_Contours[i]).Size;

        private string GetSizes()
        {
            if (f_Contours.Size == 0) return "Not sizes";
            var res = "Sizes: ";
            for (var i = 0; i < f_Contours.Size; i++)
            {
                res += $"{GetSize(i).Width}:{GetSize(i).Height}:";
            }
            return res;
        }

        private double GetPerimeter(int i) => f_Contours.Size < i ? 0 : CvInvoke.ArcLength(f_Contours[i], true);

        private string GetPerimeters()
        {
            if (f_Contours.Size == 0) return "Not perimeters";
            var res = "Perimeters: ";
            for (var i = 0; i < f_Contours.Size; i++)
            {
                res += $"{GetPerimeter(i)}: ";
            }
            return res;
        }

        private float GetDistanceBeforeCenter()
        {
            if (f_Contours.Size != 2) return 0f;
            var rctA = CvInvoke.FitEllipse(f_Contours[0]);
            var rctB = CvInvoke.FitEllipse(f_Contours[1]);
            return GetDistance(rctA.Center, rctB.Center);
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
            res.Nodes.Add(GetDistanceBeforeCenter().ToString(CultureInfo.InvariantCulture));
            res.Nodes.Add(GetСenters());
            res.Nodes.Add(GetSizes());
            return res;
        }

        public override string ToString() => IsCorrect ? $"{Path.GetFileNameWithoutExtension(FileName)}:{Pass}:{GetDistanceBeforeCenter().ToString(CultureInfo.InvariantCulture)}:{GetСenters()}:{GetSizes()}"
                : $"{FileName}:No two contours";
    }
}