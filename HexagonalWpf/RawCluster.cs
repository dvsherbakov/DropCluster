using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private Image<Bgr, ushort> _fCurrentImage;
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


        public Task MakeCluster()
        {
            try
            {
                _fCurrentImage = LoadFileAsync();
                var contours = FilterContours(ExtractContours());
                for (var i = 0; i < contours.Size; i++)
                {
                    var perimeter = CvInvoke.ArcLength(contours[i], true);
                    var approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                    _cluster.Add(new ClusterElement(i, CvInvoke.FitEllipse(contours[i]), GetComplexShear(contours[i])));
                }

                //foreach (var item in _cluster.GetList)
                //{
                //    Debug.WriteLine(item.Id);
                //    Debug.WriteLine(string.Join(";", item.Shear.GetProfileX()));
                //    Debug.WriteLine(string.Join(";", item.Shear.GetProfileY()));
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        private static bool IncludeContour(VectorOfVectorOfPoint list, int src)
        { //is internal contour?
            var ellipseOfSrc = CvInvoke.FitEllipse(list[src]);

            for (var i = 0; i < list.Size; i++)
            {
                if (list[i].Size < 10 || list[i].Size > 500) continue;
                var tmp = CvInvoke.FitEllipse(list[i]);
                if (!(ellipseOfSrc.Size.Height + ellipseOfSrc.Size.Width <
                      tmp.Size.Height + tmp.Size.Width)) continue;
                if (Math.Sqrt(Math.Pow((ellipseOfSrc.Center.X - tmp.Center.X), 2) +
                              Math.Pow((ellipseOfSrc.Center.Y - tmp.Center.Y), 2)) <
                    (tmp.Size.Height + tmp.Size.Height) / 2) return true;
            }
            return false;
        }

        private Image<Bgr, ushort> LoadFileAsync()
        {
            var res =  new Image<Bgr, ushort>(FileName);
            return res;
        }

        private VectorOfVectorOfPoint ExtractContours()
        {

            var temp = _fCurrentImage.SmoothGaussian(_gaussianParam).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(_binarizationThreshold), new Gray(255)).Dilate(2);
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
                if (contours[i].Size < 5|| contours[i].Size >500) continue;

                var rct = CvInvoke.FitEllipse(contours[i]);
                var perimeter = CvInvoke.ArcLength(contours[i], true);
                if (!(GetAspectRatio(rct) < _maxAspectRatio / 100f) || !(perimeter > _minPerimeterLen) ||
                    !(perimeter < 500)) continue;
                if (!IncludeContour(contours, i)) filteredContours.Push(contours[i]);
            }
            return filteredContours;
        }

        private static float GetAspectRatio(RotatedRect rct)
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
        
        public void CreateHexagon(ClusterElement el)
        {
            Hexagon = new Hexagon(el, _cluster.Get7(el.Element), _fFileName);
        }

        private int GetPixelBrightness(int x, int y)
        {
            if (_fCurrentImage == null) return 0;
            if (x <= 0 || y <= 0 || x >= _fCurrentImage.Size.Width || y >= _fCurrentImage.Size.Height) return 0;
            var pixel = _fCurrentImage[x, y];
            return (int)(new List<double>() { pixel.Green, pixel.Blue, pixel.Red }).Average();
        }

        private OctoShear GetComplexShear(IInputArray contour)
        {
            const int size = 80;
            var rct = CvInvoke.FitEllipse(contour);
            var cb = GetPixelBrightness((int)rct.Center.Y, (int)rct.Center.X);
            var result = new OctoShear(size, cb, rct);

            for (var i = 0; i < size; i++)
            {
                result.Dict[1][i] = GetPixelBrightness((int)(rct.Center.Y + i), (int)rct.Center.X);
                result.Dict[2][i] = GetPixelBrightness((int)(rct.Center.Y + (i * 0.7071)), (int)(rct.Center.X - (i * 0.7071)));
                result.Dict[3][i] = GetPixelBrightness((int)rct.Center.Y, (int)(rct.Center.X - i));
                result.Dict[4][i] = GetPixelBrightness((int)(rct.Center.Y - (i * 0.7071)), (int)(rct.Center.X - (i * 0.7071)));
                result.Dict[5][i] = GetPixelBrightness((int)(rct.Center.Y - i), (int)rct.Center.X);
                result.Dict[6][i] = GetPixelBrightness((int)(rct.Center.Y - (i * 0.7071)), (int)(rct.Center.X + (i * 0.7071)));
                result.Dict[7][i] = GetPixelBrightness((int)rct.Center.Y, (int)(rct.Center.X + i));
                result.Dict[8][i] = GetPixelBrightness((int)(rct.Center.Y + (i * 0.7071)), (int)(rct.Center.X + (i * 0.7071)));
            }
            return result;
        }
    }
}
