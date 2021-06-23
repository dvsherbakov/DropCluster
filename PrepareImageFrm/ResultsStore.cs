using System;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PrepareImageFrm
{
    internal class ResultsStore
    {
        private readonly List<ImageResult> _results;
        private readonly SplineInterpolator _si;

        public int Length => _results.Count();
        public ImageResult this[int index] => _results[index];

        public ResultsStore()
        {
            _results = new List<ImageResult>();
            var known = new Dictionary<double, double>
            {
                {50.0, 7.74},
                {56.0, 8.63},
                {60.0, 9.11},
                {70.0, 10.77},
                {80.0, 12.25},
                {90.0, 13.89},
                {100.0, 15.47},
                {112.0, 17.31},
            };
            _si = new SplineInterpolator(known.OrderBy(x => x.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        private bool Exists(string fileName)
        {
            return _results.Any(x => x.FileName == fileName);
        }

        public ImageResult AddToStore(ImageResult result)
        {
            if (!Exists(result.FileName))
            {
                _results.Add(result);
                return result;
            }

            _results.FirstOrDefault(x => x.FileName == result.FileName)?.UpdateContours(result.GetContours);
            return _results.FirstOrDefault(x => x.FileName == result.FileName);
        }

        public IEnumerable<string> GetUndetectedItems => _results.Where(x => !x.IsCorrect).Select(x => x.FileName);

        public int GetUndetectedCount => _results.Count(x => !x.IsCorrect);

        public IEnumerable<string> GetStorageResult(int zm)
        {
            //return _results.OrderBy(x => x.FileName).Select(x => x.ToString(_si.GetValue(zm)));
            return _results.OrderBy(x => x.FileName).Select(x => x.ToString(zm));
        }

        public int[][] GetResult(int pointer)
        {
            return _results[pointer].Brightness;
        }

        public void ClearStorage()
        {
            _results.Clear();
        }

        public void SaveAllDetail()
        {
            foreach (var item in _results)
                ImageResult.SaveDetailFile(item.FileName);
        }

        public async Task SaveExcelBrightness()
        {
            if (Length > 0)
            {
                await Task.Run(() =>
                {//Выгрузка результатов сканирования яркости
                    var fileName = new DirectoryInfo(_results.FirstOrDefault().FileName).Parent.Name + ".xlsx";

                    var file = new FileInfo(fileName);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(file))
                    {

                        foreach (var res in _results)
                        {//по всем снимкам
                            var xlsSheet = package.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension(res.FileName));
                            var row = 2;

                            foreach (var drop in res.Brightness)
                            {
                                for (var k = (drop.Length / 2) - 1; k >= 0; k--)
                                {
                                    var value = (drop[drop.Length/2 + k] + drop[drop.Length/2 - k]) / 2;
                                    xlsSheet.Cells[k + 3, row + 3].Value = value;
                                }
                                row++;
                            }
                        }
                        package.Save();
                    }
                });
            }
        }
    }
}
