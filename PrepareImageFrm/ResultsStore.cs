﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace PrepareImageFrm
{
    internal class ResultsStore
    {
        private readonly List<ImageResult> f_Results;

        public ResultsStore()
        {
            f_Results = new List<ImageResult>();
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

        public int GetUndetectedCount => f_Results.Where(x => !x.IsCorrect).Count();

        public IEnumerable<string> GetStorageResult => f_Results.OrderBy(x=>x.FileName).Select(x => x.ToString());

        public void ClearStorage()
        {
            f_Results.Clear();
        }
    }
}
