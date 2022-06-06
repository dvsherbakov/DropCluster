using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.IO;
using Emgu.CV.CvEnum;

namespace ProbeImage
{
    internal class Program
    {
        private static void Main()
        {

            const string iName = @"E:\+Data\Clusters\UF\от Т\31\Untitled_31_X115.tif";

                var img = new Image<Bgr, ushort>(iName); 

               
                double max = 0;

                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {

                        var pixel = img[x, y];
                        if (pixel.Green > max) max = pixel.Green;
                        pixel.Blue = 0;
                        pixel.Red *= 0.25;
                        if (pixel.Green > 15000)
                        {
                            pixel.Green *= 1.35;
                            pixel.Blue *= 1.35;
                        }

                        img[x, y] = pixel;
                    }
                }

                CvInvoke.Imwrite(@"E:\tmp\"+Path.GetFileName(iName), img);
                Console.WriteLine(max);
            

            Console.ReadKey();
        }

    }
}
