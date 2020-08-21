using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace PrepareImageFrm
{
    class ResultsStore
    {
        private readonly List<ImageResult> results;

        public ResultsStore()
        {
            results = new List<ImageResult>();
        }

        public bool Exists(string fileName)
        {
            return results.Where(x => x.FileName == fileName).Any();
        }

        public void AddToStore(ImageResult result)
        {
            if (!Exists(result.FileName))
            {
                results.Add(result);
            }
            else
            {
                results.Where(x => x.FileName == result.FileName).FirstOrDefault().UpdateContours(result.GetContours);
            }
        }
    }
}
