using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace ProbeImage
{
    class Program
    {
        static void Main(string[] args)
        {
            var iName = @"D:\+Data\Experiments\14.08.20\Стабильные\2 750_150 hz 30x 77dg C\750_150_30_00139.jpg";
            Image<Bgr, Byte> img = new Image<Bgr, byte>(iName);//.Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            double cannyThreshold = 180.0;
            double cannyThresholdLinking = 120.0;
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

            VectorOfVectorOfPoint conturs = new VectorOfVectorOfPoint();
            Mat hierarchi = new Mat();
            CvInvoke.FindContours(cannyEdges, conturs, hierarchi, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < conturs.Size; i++)
            {
                double perimetr = CvInvoke.ArcLength(conturs[0], true);
                VectorOfPoint aproximation =  new VectorOfPoint();
                CvInvoke.ApproxPolyDP(conturs[i], aproximation, 0.04 * perimetr, true);
                CvInvoke.DrawContours(img, conturs, i, new MCvScalar(0, 0, 255), 2);
                Console.WriteLine(conturs[i].Size);
            }
            CvInvoke.Imwrite("cntrs.jpg", cannyEdges);
            CvInvoke.Imwrite("test.jpg", img);
            Console.ReadKey();
        }

        class FindContours
        {
            /// <summary>
            /// Method used to process the image and set the output result images.
            /// </summary>
            /// <param name="colorImage">Source color image.</param>
            /// <param name="thresholdValue">Value used for thresholding.</param>
            /// <param name="processedGray">Resulting gray image.</param>
            /// <param name="processedColor">Resulting color image.</param>
            public void IdentifyContours(Bitmap colorImage, int thresholdValue, bool invert, out Bitmap processedGray, out Bitmap processedColor)
            {

                #region Conversion To grayscale
                Image<Gray, byte> grayImage = new Image<Gray, byte>(colorImage);
                Image<Bgr, byte> color = new Image<Bgr, byte>(colorImage);

                #endregion


                #region  Image normalization and inversion (if required)
                grayImage = grayImage.ThresholdBinary(new Gray(thresholdValue), new Gray(255));
                if (invert)
                {
                    grayImage._Not();
                }
                #endregion

                #region Extracting the Contours
                using (MemStorage storage = new MemStorage())
                {

                    for (Contour<Point> contours = grayImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, storage); contours != null; contours = contours.HNext)
                    {

                        Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.015, storage);
                        if (currentContour.BoundingRectangle.Width > 20)
                        {
                            CvInvoke.cvDrawContours(color, contours, new MCvScalar(255), new MCvScalar(255), -1, 2, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                            color.Draw(currentContour.BoundingRectangle, new Bgr(0, 255, 0), 1);
                        }
                    }

                }
                #endregion


                #region Asigning output
                processedColor = color.ToBitmap();
                processedGray = grayImage.ToBitmap();
                #endregion
            }
        }
    }
}
