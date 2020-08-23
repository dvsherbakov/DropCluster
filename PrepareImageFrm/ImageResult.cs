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
        private readonly List<VectorOfPoint> f_Contours;
        public int Pass { get; private set; }
        public float Distance { 
            get => GetDistanceBeforeCenter();
        }

        private bool IsCorrect => f_Contours.Count == 2;

        public ImageResult(string fileName, VectorOfVectorOfPoint listOfContours)
        {
            FileName = fileName;
            Pass = 1;
            f_Contours = new List<VectorOfPoint>();
            for (var i = 0; i < listOfContours.Size; i++)
            {
                f_Contours.Add(listOfContours[i]);
            }
        }

        public PointF GetCenter(int i) => f_Contours.Count < i ? new PointF() : CvInvoke.FitEllipse(f_Contours[i]).Center;

        private SizeF GetSize(int i) => f_Contours.Count < i ? new SizeF() : CvInvoke.FitEllipse(f_Contours[i]).Size;

        private double GetPerimeter(int i) => f_Contours.Count < i ? 0 : CvInvoke.ArcLength(f_Contours[i], true);

        private string GetPerimeters()
        {
            if (f_Contours.Count == 0) return "Not perimeters";
            else
            {
                var res = "Perimeters: ";
                for (var i = 0; i < f_Contours.Count; i++)
                {
                    res += $"{GetPerimeter(i)}: ";
                }
                return res;
            }
        }

        private float GetDistanceBeforeCenter()
        {
            if (f_Contours.Count != 2) return 0f;
            var rctA = CvInvoke.FitEllipse(f_Contours[0]);
            var rctB = CvInvoke.FitEllipse(f_Contours[1]);
            return GetDistance(rctA.Center, rctB.Center);
        }

        private static float GetDistance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public void UpdateContours(IEnumerable<VectorOfPoint> newList )
        {
            f_Contours.Clear();
            f_Contours.AddRange(newList);
            Pass++;
        }

        public IEnumerable<VectorOfPoint> GetContours => f_Contours;

        public TreeNode GetResultNode()
        {
            var res = new TreeNode(Path.GetFileNameWithoutExtension(FileName))
            {
                Name = FileName,
                ForeColor = IsCorrect ? Color.LightGreen : Color.OrangeRed
            };
            //res.Nodes.Add(IsCorrect ? "Correct" : "Not detected");
            res.Nodes.Add(GetPerimeters());
            res.Nodes.Add(GetDistanceBeforeCenter().ToString(CultureInfo.InvariantCulture));
            return res;
        }
    }
}