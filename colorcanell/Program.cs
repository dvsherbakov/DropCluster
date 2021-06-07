using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace colorcanell
{
    internal class Program
    {
        private static void Main()
        {
            var files = Directory.GetFiles(@"D:\+Data\Experiments\02.06.2021\02");
            foreach (var fileName in files)
            {
                GetAvgChannels(fileName);
            }
            
        }


        private static void GetAvgChannels(string fileName)
        {
            var pe = new PictResult(fileName);
            var rgb = pe.GetAverage();
        }
    }

    internal class PictResult
    {
        private readonly Bitmap _bitmap;
        public string FileName { get; set; }

        public PictResult(string fileName)
        {
            using (Stream bitmapStream = File.Open(fileName, FileMode.Open))
            {
                var img = Image.FromStream(bitmapStream);
                FileName = fileName;
                _bitmap = new Bitmap(img);
            }
        }

        public Channels GetArea(int x, int y)
        {
            int accR = 0, accG = 0, accB = 0;
            for (var cy = y; cy<y+5; cy++)
            {
                for (var cx = x; cx<x+5; cx++)
                {
                    accR += _bitmap.GetPixel(cx, cy).R;
                    accG += _bitmap.GetPixel(cx, cy).G;
                    accB += _bitmap.GetPixel(cx, cy).B;
                }
            }
            return new Channels
            {
                R = (byte)(accR / 25),
                G = (byte)(accG / 25),
                B = (byte)(accB / 25)
            };
        }

        public Channels GetAverage()
        {
            var coords = new[] {new Size(50, 50), new Size(_bitmap.Size.Width-60, 50), new Size(_bitmap.Size.Width - 60, _bitmap.Size.Height-60), new Size(50, _bitmap.Size.Height - 60) };
            int accR = 0, accG = 0, accB = 0;
            foreach (var c in coords)
            {
                var tmp = GetArea(c.Width, c.Height);
                accR += tmp.R;
                accG += tmp.G;
                accB += tmp.B;
            }

            return new Channels{R=(byte)(accR/4), G=(byte)(accG/4), B=(byte)(accB/4)};
        }
    }

    internal class Channels
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }

    internal class ResultInfo : Channels
    {
        private string _fileName { get; set; }

        public ResultInfo(string fileName, Channels ch)
        {
            _fileName = fileName;
            R = ch.R;
            G = ch.G;
            B = ch.B;
        }
    }

    internal class GroupResult
    {
        private readonly  List<ResultInfo> _channelsList;
        public GroupResult(string pathName)
        {
            _channelsList = new List<ResultInfo>();
            var files = Directory.GetFiles(pathName);
            foreach (var fileName in files)
            {
               GetAvg(fileName);
            }
        }

        private void GetAvg(string fName)
        {
            var pe = new PictResult(fName);
            //return pe.GetAverage();
        }
    }
}
