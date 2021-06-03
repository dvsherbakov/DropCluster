using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace colorcanell
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"D:\+Data\Experiments\02.06.2021\02");
            GetAvgChannels(files[1]);
        }

       

        static void GetAvgChannels(string fileName)
        {

            
        }
    }

    class PictResult
    {
        Bitmap f_Bitmap;

        public PictResult(string fileName)
        {
            using (Stream BitmapStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image img = Image.FromStream(BitmapStream);

                f_Bitmap = new Bitmap(img);
            }
        }

        private void GetArea(int x, int y)
        {
            int accR = 0, accG = 0, accB = 0;
            for (var cy = y; cy<y+5; cy++)
            {
                for (var cx = x; cx<x+5; cx++)
                {
                    accR += f_Bitmap.GetPixel(cx, cy).R;
                }
            }
        }
    }

    class Channels
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
