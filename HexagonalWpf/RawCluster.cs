using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace HexagonalWpf
{
    internal class RawCluster
    {
        private readonly int _gaussianParam;
        private readonly int _binarizationThreshold;
        private readonly int _maxAspectRatio;
        private readonly int _minPerimeterLen;

        private readonly Cluster _cluster;
        public Cluster GetCluser => _cluster;

        public Hexagon Hexagon { get; private set; }

        public IEnumerable<ClusterElement> GetElements => _cluster.GetList;
        private readonly string _fFileName;
        private Image<Bgr, byte> _fCurrentImage;
        public string FileName => _fFileName;

        public RawCluster(string fName, int gaussianParam, int binarizationThreshold, int maxAspectRatio, int minPerimeterLen)
        {
            _fFileName = fName;
            _cluster = new Cluster(fName);
            _gaussianParam = gaussianParam;
            _binarizationThreshold = binarizationThreshold;
            _maxAspectRatio = maxAspectRatio;
            _minPerimeterLen = minPerimeterLen;
        }


        public async Task MakeCluster()
        {
            try
            {
                _fCurrentImage = await LoadFileAsync();
                var contours = FilterContours(ExtractContours());
                for (var i = 0; i < contours.Size; i++)
                {
                    var perimeter = CvInvoke.ArcLength(contours[i], true);
                    var approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                    _cluster.Add(new ClusterElement(i, CvInvoke.FitEllipse(contours[i])));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private bool IncludeContour(VectorOfVectorOfPoint list, int src)
        { //is internal contour?
            var ellipseOfSrc = CvInvoke.FitEllipse(list[src]);

            for (var i = 0; i < list.Size; i++)
            {
                if (list[i].Size < 10) continue;
                var tmp = CvInvoke.FitEllipse(list[i]);
                if (!(ellipseOfSrc.Size.Height + ellipseOfSrc.Size.Width <
                      tmp.Size.Height + tmp.Size.Width)) continue;
                if (Math.Sqrt(Math.Pow((ellipseOfSrc.Center.X - tmp.Center.X), 2) +
                              Math.Pow((ellipseOfSrc.Center.Y - tmp.Center.Y), 2)) <
                    (tmp.Size.Height + tmp.Size.Height) / 2) return true;
            }
            return false;
        }

        private async Task<Image<Bgr, byte>> LoadFileAsync()
        {
            var res = await Task.Run(() => new Image<Bgr, byte>(FileName));
            return res;
        }

        private VectorOfVectorOfPoint ExtractContours()
        {

            var temp = _fCurrentImage.SmoothGaussian(_gaussianParam).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(_binarizationThreshold), new Gray(255));
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
                if (GetAspectRatio(rct) < _maxAspectRatio / 100f && perimeter > _minPerimeterLen && perimeter < 500)
                    if (!IncludeContour(contours, i)) filteredContours.Push(contours[i]);
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

        public ClusterElement GetNearer(RotatedRect el)
        {
            return _cluster.GetNearer(el);
        }

        //public PointF GetCenter()
        //{
        //    return _cluster.GetCenter();
        //}

        public void CreateHexagon(ClusterElement el)
        {
            Hexagon = new Hexagon(el, _cluster.Get7(el.Element), _fFileName);
        }
    }
}
