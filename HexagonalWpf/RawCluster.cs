using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalWpf
{
    class RawCluster
    {
        private readonly int f_GaussianParam;
        private readonly int f_BinarizationThreshold;
        private readonly int f_MaxAspectRatio;
        private readonly int f_MinPerimeterLen;

        private Cluster f_Cluster;
        public IEnumerable<ClusterElement> GetElements => f_Cluster.GetList;
        private readonly string f_FileName;
        private Image<Bgr, byte> f_CurrentImage;
        public string FileName => f_FileName;

        public RawCluster(string fName, int gaussianParam, int binarizationThreshold, int maxAspectRatio, int minPerimeterLen)
        {
            f_FileName = fName;
            f_Cluster = new Cluster(fName);
            f_GaussianParam = gaussianParam;
            f_BinarizationThreshold = binarizationThreshold;
            f_MaxAspectRatio = maxAspectRatio;
            f_MinPerimeterLen = minPerimeterLen;
        }


        public async Task MakeCluster()
        {
            try
            {
                f_CurrentImage = await LoadFileAsync();
                var contours = FilterContours(ExtractContours());
                for (var i = 0; i < contours.Size; i++)
                {
                    var perimeter = CvInvoke.ArcLength(contours[i], true);
                    var approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                    f_Cluster.Add(new ClusterElement(i, CvInvoke.FitEllipse(contours[i])));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task<Image<Bgr, byte>> LoadFileAsync()
        {
            var res = await Task.Run(() =>
            {

                return new Image<Bgr, byte>(FileName);
            });
            return res;
        }

        private VectorOfVectorOfPoint ExtractContours()
        {

            var temp = f_CurrentImage.SmoothGaussian(f_GaussianParam).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(f_BinarizationThreshold), new Gray(255));
            var contours = new VectorOfVectorOfPoint();
            var m = new Mat();
            CvInvoke.FindContours(image: temp, contours, m, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);
            for (var i = 0; i < contours.Size; i++)
            {
                var perimeter = CvInvoke.ArcLength(contours[i], true);
                var approx = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(contours[i], approx, 0.1 * perimeter, true);
            }
            temp.Dispose();
            m.Dispose();
            return contours;
        }

        private VectorOfVectorOfPoint FilterContours(VectorOfVectorOfPoint contours)
        {
            var filteredContours = new VectorOfVectorOfPoint();
            for (var i = 0; i < contours.Size; i++)
            {
                if (contours[i].Size < 5) continue;
                var rct = CvInvoke.FitEllipse(contours[i]);
                var perimeter = CvInvoke.ArcLength(contours[i], true);
                if ((GetAspectRatio(rct) < f_MaxAspectRatio / 100f) && (perimeter > f_MinPerimeterLen))
                    filteredContours.Push(contours[i]);
            }
            return filteredContours;
        }

        private float GetAspectRatio(RotatedRect rct)
        {
            try
            {
                return 1 - (rct.Size.Width / rct.Size.Height);
            }
            catch (Exception)
            {
                //listBox1.Items.Add(ex.Message);
                return 1.0f;
            }
        }
    }
}
