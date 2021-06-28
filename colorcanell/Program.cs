using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using OfficeOpenXml;

namespace colorcanell
{
    internal class Program
    {
        private static void Main()
        {
            var res = new GroupResult(@"D:\+Data\Experiments\02.06.2021\02");
            res.SaveResult(@"D:\+Data\Experiments\02.06.2021\02\res.xlsx");
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
                FileName = Path.GetFileName(fileName);
                _bitmap = new Bitmap(img);
            }
        }

        public Channels GetArea(int x, int y)
        {
            int accR = 0, accG = 0, accB = 0;
            for (var cy = y; cy < y + 5; cy++)
            {
                for (var cx = x; cx < x + 5; cx++)
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
            var coords = new[] { new Size(50, 50), new Size(_bitmap.Size.Width - 60, 50), new Size(_bitmap.Size.Width - 60, _bitmap.Size.Height - 60), new Size(50, _bitmap.Size.Height - 60) };
            int accR = 0, accG = 0, accB = 0;
            foreach (var c in coords)
            {
                var tmp = GetArea(c.Width, c.Height);
                accR += tmp.R;
                accG += tmp.G;
                accB += tmp.B;
            }

            return new Channels { R = (byte)(accR / 4), G = (byte)(accG / 4), B = (byte)(accB / 4) };
        }

        public ResultInfo GetResultInfo => new ResultInfo(FileName, GetAverage());
    }

    internal class Channels
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }

    internal class ResultInfo : Channels
    {
        public string FileName { get; set; }

        public ResultInfo(string fileName, Channels ch)
        {
            FileName = fileName;
            R = ch.R;
            G = ch.G;
            B = ch.B;
        }
    }

    internal class GroupResult
    {
        private readonly List<ResultInfo> _channelsList;
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
            _channelsList.Add(pe.GetResultInfo);
            Console.WriteLine(fName);
        }

        public void SaveResult(string fName)
        {
            var file = new FileInfo(fName);
            ExcelPackage.LicenseContext = System.ComponentModel.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(file))
            {
                var xlsSheet = package.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension("test"));
                var row = 2;
                foreach (var values in _channelsList)
                {
                    xlsSheet.Cells[row, 2].Value = values.FileName;
                    xlsSheet.Cells[row, 3].Value = values.R;
                    xlsSheet.Cells[row, 4].Value = values.G;
                    xlsSheet.Cells[row, 5].Value = values.B;

                    row++;
                }
                package.Save();
            }
        }
    }
}
