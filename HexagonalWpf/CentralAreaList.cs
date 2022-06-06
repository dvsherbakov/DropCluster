using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using OfficeOpenXml;

namespace HexagonalWpf
{
    internal class CentralAreaList
    {
        private readonly List<CentralArea> _centralAreas;
        private string startFile;

        public CentralAreaList()
        {
            _centralAreas = new List<CentralArea>();
        }

        public void AddDir(FileList fl)
        {
            _centralAreas.Clear();
            Task.Run(() =>
            {
                startFile = fl.GetFirst;
                foreach (var file in fl.GetList)
                {
                    var res = new Image<Bgr, ushort>(file);
                    var center = new Point(res.Size.Width / 2, res.Size.Height / 2);
                    var cl = new List<int>();
                    for (var y = center.Y - 10; y < center.Y + 10; y++)
                    {
                        for (var x = center.X - 10; x < center.X + 10; x++)
                        {
                            var pixel = res[x, y];
                            cl.Add((int)(new List<double>() { pixel.Green, pixel.Blue, pixel.Red }).Average());
                        }
                    }

                    _centralAreas.Add(new CentralArea(cl.Average(), file));

                    //
                    //double max = 0;

                    //for (var y = 0; y < res.Height; y++)
                    //{
                    //    for (var x = 0; x < res.Width; x++)
                    //    {

                    //        var pixel = res[x, y];
                    //        if (pixel.Green > max) max = pixel.Green;
                    //        pixel.Blue = 0;
                    //        pixel.Red *= 0.25;
                            

                    //        res[x, y] = pixel;
                    //    }
                    //}

                    //CvInvoke.Imwrite(@"E:\tmp\2\" + Path.GetFileName(file), res);
                }
            });
        }

        public async Task SaveExcelFile()
        {
            await Task.Run(() =>
            {
                var fileName = Path.GetDirectoryName(startFile) + "\\outAreas.xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                var file = new FileInfo(fileName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file))
                {
                    var xlsSheet = package.Workbook.Worksheets.Add("areas");
                    var row = 2;
                    foreach (var area in _centralAreas.OrderBy(x => x.Name.number))
                    {

                        xlsSheet.Cells[$"B{row}"].Value = area.Name.number;
                        xlsSheet.Cells[$"D{row}"].Value = area.AvgSpot;

                        row++;
                    }
                    package.Save();
                }
            });

        }
    }
}
