using Emgu.CV;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace PrepareImageFrm
{
    public class ImageResult
    {
        public string FileName { get; }
        private readonly List<VectorOfPoint> contours;
        public int Pass { get; private set; }
        public float Distance { 
            get => GetDistanceBeforeCenter();
        }

        public bool IsCorrect { get => contours.Count == 2; }

        public ImageResult(string fileName, VectorOfVectorOfPoint listOfContours)
        {
            FileName = fileName;
            Pass = 1;
            contours = new List<VectorOfPoint>();
            for (int i = 0; i < listOfContours.Size; i++)
            {
                contours.Add(listOfContours[i]);
            }
        }

        public PointF GetCenter(int i)
        {
            if (contours.Count < i) return new PointF();
            return CvInvoke.FitEllipse(contours[i]).Center;
        }

        private float GetDistanceBeforeCenter()
        {
            if (contours.Count != 2) return 0f;
            var rctA = CvInvoke.FitEllipse(contours[0]);
            var rctB = CvInvoke.FitEllipse(contours[1]);
            return GetDistance(rctA.Center, rctB.Center);
        }

        private float GetDistance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public void UpdateContours(List<VectorOfPoint> newList )
        {
            contours.Clear();
            contours.AddRange(newList);
        }

        public List<VectorOfPoint> GetContours { get => contours; }

    }
}