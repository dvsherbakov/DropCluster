using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PrepareImageFrm
{
    internal class ResultsStore
    {
        private readonly List<ImageResult> f_Results;
        private readonly SplineInterpolator f_Si;

        public int Length => f_Results.Count();
        public ImageResult this[int index] => f_Results[index];

        public ResultsStore()
        {
            f_Results = new List<ImageResult>();
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
            f_Si = new SplineInterpolator(known.OrderBy(x => x.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        private bool Exists(string fileName)
        {
            return f_Results.Any(x => x.FileName == fileName);
        }

        public ImageResult AddToStore(ImageResult result)
        {
            if (!Exists(result.FileName))
            {
                f_Results.Add(result);
                return result;
            }

            f_Results.FirstOrDefault(x => x.FileName == result.FileName)?.UpdateContours(result.GetContours);
            return f_Results.FirstOrDefault(x => x.FileName == result.FileName);
        }

        public IEnumerable<string> GetUndetectedItems => f_Results.Where(x => !x.IsCorrect).Select(x => x.FileName);

        public int GetUndetectedCount => f_Results.Count(x => !x.IsCorrect);

        public IEnumerable<string> GetStorageResult(int zm)
        {
            //return f_Results.OrderBy(x => x.FileName).Select(x => x.ToString(f_Si.GetValue(zm)));
            return f_Results.OrderBy(x => x.FileName).Select(x => x.ToString(zm));
        }

        public void ClearStorage()
        {
            f_Results.Clear();
        }

        public void SaveAllDetail()
        {
            foreach (var item in f_Results)
                ImageResult.SaveDetailFile(item.FileName);
        }

        public async Task SaveExcelBrightness()
        {
            if (Length > 0)
            {
                await Task.Run(() =>
                {//Выгрузка результатов сканирования яркости
                    var fileName = new DirectoryInfo(f_Results.FirstOrDefault().FileName).Parent.Name + ".xlsx";

                    var file = new FileInfo(fileName);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(file))
                    {

                        foreach (var res in f_Results)
                        {//по всем снимкам
                            var xlsSheet = package.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension(res.FileName));
                            var row = 2;

                            foreach (var drop in res.Bribrightness)
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
