using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.UI;
using Emgu.CV.Structure;

namespace ImageProbe
{
    class Program
    {
        static void Main(string[] args)
        {
            string iName = @"D:\+Data\Experiments\14.08.20\Стабильные\3 800 150hz 30x 79dg C.jpg";
            string oFile = @"out.jpg";

            UMat image=null;
            CvInvoke.Imread(iName, Emgu.CV.CvEnum.ImreadModes.Color).CopyTo(image);
        }
    }
}
