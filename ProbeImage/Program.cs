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

            double cannyThreshold = 90;//180.0;
            double cannyThresholdLinking = 60;//120.0;
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

            VectorOfVectorOfPoint conturs = new VectorOfVectorOfPoint();
            Mat hierarchi = new Mat();
            CvInvoke.FindContours(cannyEdges, conturs, hierarchi, RetrType.External, ChainApproxMethod.LinkRuns);

            for (int i = 0; i < conturs.Size; i++)
            {
                double perimetr = CvInvoke.ArcLength(conturs[0], true);
                VectorOfPoint aproximation = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(conturs[i], aproximation, 0.04 * perimetr, true);
                if (conturs[i].Size > 10 )
                {
                    var rct = CvInvoke.FitEllipse(conturs[i]);
                    CvInvoke.DrawContours(img, conturs, i, new MCvScalar(0, 0, 255), 2);
                    //CvInvoke.Rectangle(img, rct, new MCvScalar(255, 0, 0));
                    //CvInvoke.IsContourConvex(conturs[i]);
                    var vertices = rct.GetVertices();
                    for (int t = 0; t < 4; t++)
                        CvInvoke.Line(img, 
                            new Point((int)vertices[t].X, (int)vertices[t].Y), 
                            new Point((int)vertices[(t + 1) % 4].X, (int)vertices[(t + 1) % 4].Y), 
                            new MCvScalar(0, 255, 0));
                }
                Console.WriteLine(conturs[i].Size);
            }
            CvInvoke.Imwrite("cntrs.jpg", cannyEdges);
            CvInvoke.Imwrite("test.jpg", img);
            Console.ReadKey();
        }

    } 
}
