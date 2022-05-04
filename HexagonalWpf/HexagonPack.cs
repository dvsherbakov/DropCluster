using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HexagonalWpf
{
    class HexagonPack
    {
        private readonly List<Hexagon> _hexagon;
        public IEnumerable<Hexagon> HexList => _hexagon.OrderBy(x => x.FileName);
        private RelativePosition _currentPosition;
        private readonly string _startName;
        public static int Zoom;
        private static double ZoomKoef => (4.65 * Zoom + 5.9) / 305;

        public HexagonPack(string fileName, RelativePosition startPos)
        {
            _hexagon = new List<Hexagon>();
            _currentPosition = startPos;
            _startName = fileName;
            Zoom = Properties.Settings.Default.CameraZoom;
        }

        public async Task PrepareFolderAsync()
        {
            var folderName = Path.GetDirectoryName(_startName);
            var fileExt = Path.GetExtension(_startName);
            var files = Directory.GetFiles(folderName ?? string.Empty, $"*{fileExt}");
            foreach (var file in files)
            {
                var rawCluster = new RawCluster(
                file,
                Properties.Settings.Default.GaussianParam,
                Properties.Settings.Default.BinarizationThreshold,
                Properties.Settings.Default.MaxAspectRatio,
                Properties.Settings.Default.MinPerimetherLen);
                await rawCluster.MakeCluster();
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Operation finished!");
            });
        }

       // public Hexagon GetFirst => _hexagon.OrderBy(x => x.FileName).FirstOrDefault();

       // public int Count => _hexagon.Count;

        public async Task SaveExcelFile()
        {
            var rnd = new Random();
            await Task.Run(() =>
            {
                var fileName = Path.GetDirectoryName(_startName) + "\\out.xlsx";
                var zm = ZoomKoef;
                var file = new FileInfo(fileName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file))
                {
                    var xlsSheet = package.Workbook.Worksheets.Add("tmp" + rnd.Next(999));
                    var row = 2;
                    foreach (var hexagon in _hexagon.OrderBy(x => x.FileName))
                    {

                        xlsSheet.Cells[$"B{row}"].Value = hexagon.FileName;
                        xlsSheet.Cells[$"C{row}"].Value = hexagon.Center.Element.Center.X / zm;
                        xlsSheet.Cells[$"D{row}"].Value = hexagon.Center.Element.Center.Y / zm;

                        xlsSheet.Cells[$"F{row}"].Value = hexagon.AverageSize / zm;
                        xlsSheet.Cells[$"G{row}"].Value = hexagon.AverageLink() / zm;

                        row++;

                    }
                    package.Save();
                }
            });

        }
    }
}
