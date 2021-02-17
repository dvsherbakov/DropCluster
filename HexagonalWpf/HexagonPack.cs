using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace HexagonalWpf
{
    class HexagonPack
    {
        private List<Hexagon> f_Hexagon;
        public IEnumerable<Hexagon> HexList => f_Hexagon.OrderBy(x => x.FileName);
        private RelativePosition f_CurrentPosition;
        private string f_StartName;
        public static int f_Zoom;
        private static double ZoomKoef => (4.65 * f_Zoom + 5.9) / 305;

        public HexagonPack(string fileName, RelativePosition startPos)
        {
            f_Hexagon = new List<Hexagon>();
            f_CurrentPosition = startPos;
            f_StartName = fileName;
            f_Zoom = Properties.Settings.Default.CameraZoom;
        }

        public async Task PrepareFolderAsync()
        {
            var folderName = Path.GetDirectoryName(f_StartName);
            var fileExt = Path.GetExtension(f_StartName);
            var files = Directory.GetFiles(folderName, $"*{fileExt}");
            foreach (var file in files)
            {
                var rawCluster = new RawCluster(
                file,
                Properties.Settings.Default.GaussianParam,
                Properties.Settings.Default.BinarizationThreshold,
                Properties.Settings.Default.MaxAspectRatio,
                Properties.Settings.Default.MinPerimetherLen);
                await rawCluster.MakeCluster();
                var position = rawCluster.RelativeToPos(f_CurrentPosition);
                var pt = new Emgu.CV.Structure.RotatedRect(
                    new System.Drawing.PointF((float)(position.X ), (float)(position.Y )),
                    new System.Drawing.SizeF(),
                    0
                );
                var markedElement = rawCluster.GetNearer(pt);
                
                rawCluster.CreateHexagon(markedElement);
                f_Hexagon.Add(rawCluster.Hexagon);
                f_CurrentPosition = rawCluster.GetRelativePosition(rawCluster.Hexagon.Center.Element.Center);
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Operation finished!");
            });
        }

        public Hexagon GetFirst => f_Hexagon.OrderBy(x=>x.FileName).FirstOrDefault();

        public int Count => f_Hexagon.Count;

        public async Task SaveExcelFile()
        {
            var rnd = new Random();
            await Task.Run(() =>
            {
                string fileName = Path.GetDirectoryName(f_StartName) + "\\out.xlsx";
                var zm = ZoomKoef;
                FileInfo file = new FileInfo(fileName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file))
                {
                    var xlsSheet = package.Workbook.Worksheets.Add("tmp"+rnd.Next(999));
                    var row = 2;
                    foreach (var hexagon in f_Hexagon.OrderBy(x=>x.FileName))
                    {
                        
                       
                        
                            xlsSheet.Cells[$"B{row}"].Value = hexagon.FileName;
                            xlsSheet.Cells[$"C{row}"].Value = hexagon.Center.Element.Center.X / zm;
                            xlsSheet.Cells[$"D{row}"].Value = hexagon.Center.Element.Center.Y / zm;

                            xlsSheet.Cells[$"F{row}"].Value = (hexagon.AverageSize) / zm;
                            xlsSheet.Cells[$"G{row}"].Value = (hexagon.AverageLink()) / zm;

                            row++;
                        
                    }
                    package.Save();
                }
            });

        }
    }
}
