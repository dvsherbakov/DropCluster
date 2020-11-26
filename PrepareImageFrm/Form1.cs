using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace PrepareImageFrm
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> f_ImgInput;
        private int f_BinarizationThreshold = 60;
        private int f_GaussianParam = 5;
        private int f_MaxAspectRatio = 33;
        private int f_MinPerimeterLen = 120;
        private int f_Zoom = 112;
        private int f_ObjectCount;
        private string f_CurrentFile;
        private readonly ResultsStore f_Storage;


        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            f_Storage = new ResultsStore();

        }

        private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    RestoreDirectory = true
                };
                if (dialog.ShowDialog() != DialogResult.OK) return;

                await LoadFile(dialog.FileName);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private async Task LoadFile(string fileName)
        {
            f_CurrentFile = fileName;
            f_ImgInput = await LoadFileAsync(fileName);
            pictureBox2.Image = f_ImgInput.AsBitmap();
        }

        private void DetectShapesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (f_ImgInput == null) return;
            try
            {
                var tmpCnt = ExtractContours(f_ImgInput);
                var contours = FilterContours(tmpCnt);
                AddContoursToResCollection(f_CurrentFile, contours);

                for (var i = 0; i < contours.Size; i++)
                {
                    var perimeter = CvInvoke.ArcLength(contours[i], true);
                    var approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                    //var rct = CvInvoke.FitEllipse(contours[i]);
                    //Ellipse ellipse = new Ellipse(rct);
                    //f_ImgInput.Draw(ellipse, new Bgr(Color.Yellow), 2);
                    //CvInvoke.DrawContours(f_ImgInput, contours, i, new MCvScalar(150, 34, 98));
                }

                pictureBox1.Image = f_ImgInput.AsBitmap();
                //pictureBox2.Image = temp.AsBitmap();
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private float GetAspectRatio(RotatedRect rct)
        {
            try
            {
                return 1 - (rct.Size.Width / rct.Size.Height);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
                return 1.0f;
            }
        }

        private void SavePreparedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save($"{Path.GetFileNameWithoutExtension(f_CurrentFile)}.jpeg", ImageFormat.Jpeg);
        }

        private async Task PrepareFile(string filename)
        {
            try
            {
                f_CurrentFile = filename;
                var inputImage = await LoadFileAsync(filename);
                var contours = FilterContours(ExtractContours(inputImage));
                AddContoursToResCollection(filename, contours);
                inputImage.Dispose();
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"{Path.GetFileNameWithoutExtension(filename)}: {ex.Message}");

            }

        }

        private void AddContoursToResCollection(string fileName, VectorOfVectorOfPoint contours)
        {
            var sizes = new Dictionary<int, int>();
            var bLst = new List<int[]>();
            for (var i = 0; i < contours.Size; i++)
            {
                var brig = BrightnessMultyShear(contours[i]);
                bLst.Add(brig);
                var rct = CvInvoke.FitEllipse(contours[i]);
                var sz = (int)(rct.Size.Width + rct.Size.Height);
                sizes.Add(i, sz);
            }

            var result = f_Storage.AddToStore(new ImageResult(fileName, contours, bLst.ToArray(), f_ObjectCount));
            if (result.Pass == 1)
                tvResults.Nodes.Add(result.GetResultNode());
            else
            {
                tvResults.Nodes.Remove(
                    tvResults.Nodes.Find(result.FileName, false).FirstOrDefault() ??
                    throw new InvalidOperationException());
                tvResults.Nodes.Add(result.GetResultNode());
            }

            //var bigs = sizes.OrderByDescending(x => x.Value).Skip(10).Take(50).ToDictionary(x => x.Key, x => x.Value);
            for (var i = 0; i < contours.Size; i++)
            {
                var rct = CvInvoke.FitEllipse(contours[i]);
                var ellipse = new Ellipse(rct);
                f_ImgInput.Draw(ellipse, new Bgr(Color.Yellow), 2);
                CvInvoke.PutText(f_ImgInput, i.ToString(),
                    new Point((int) (rct.Center.X + 30), (int) (rct.Center.Y + 50)), FontFace.HersheyComplex, 1.5,
                    new Bgr(Color.AntiqueWhite).MCvScalar);
            }
        }

        private void GetBrightessDrops(VectorOfVectorOfPoint contours, string fileName, Dictionary<int, int> bigs)
        {
            var bList = new List<string>();
            foreach (var pair in bigs)
            {
                var normDict = NormalizeBrightness(BrightnessMultyShear(contours[pair.Key]));
                var hShear = string.Join(":", normDict.Select(x => x.Value.ToString(CultureInfo.InvariantCulture)).ToArray());
                var indexes = string.Join(":", normDict.Select(x => x.Key.ToString()).ToArray());
                bList.Add($"{pair.Key}:Br:{indexes}");
                bList.Add($"{pair.Key}:Br:{hShear}");
            }

            using (TextWriter tw = new StreamWriter($"{Path.GetFileNameWithoutExtension(fileName)}.csv"))
            {
                foreach (var s in bList)
                    tw.WriteLine(s);
            }

            foreach (var pair in bigs)
            {
                var rct = CvInvoke.FitEllipse(contours[pair.Key]);
                var ellipse = new Ellipse(rct);
                f_ImgInput.Draw(ellipse, new Bgr(Color.Yellow), 2);
                CvInvoke.PutText(f_ImgInput, pair.Key.ToString(), new Point((int)(rct.Center.X + 30), (int)(rct.Center.Y + 50)), FontFace.HersheyComplex, 1.5, new Bgr(Color.AntiqueWhite).MCvScalar);
            }
        }

        private async void DirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new FolderBrowserDialog
                {
                    SelectedPath = @"D:\+Data\Experiments"
                };
                if (dialog.ShowDialog() != DialogResult.OK) return;
                listBox1.Items.Add("Start directory encode");
                var files = Directory.GetFiles(dialog.SelectedPath);
                foreach (var file in files)
                {
                    //var tmpRes = 
                    await PrepareFile(file);
                    //listBox1.Items.Add(tmpRes);
                }
                listBox1.Items.Add("Finished directory encode");
                listBox1.Items.Add($"Undetected: {f_Storage.GetUndetectedCount}");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private void SaveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileName = $"{DateTime.Now.ToShortDateString().Replace('.', '_')}_{DateTime.Now.ToShortTimeString().Replace(':', '_')}.csv";
            var saveFile = new StreamWriter(fileName);
            var results = f_Storage.GetStorageResult(f_Zoom);
            foreach (var item in results)
            {
                saveFile.WriteLine(item);
            }
            saveFile.Flush();
            saveFile.Close();
            listBox1.Items.Add($"Log {fileName} saved");
        }

        private async Task<Image<Bgr, byte>> LoadFileAsync(string fileName)
        {
            var res = await Task.Run(() =>
            {
                f_CurrentFile = fileName;
                return new Image<Bgr, byte>(fileName);
            });
            return res;
        }

        private void HistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var img = new Bitmap(pictureBox2.Image).ToImage<Gray, byte>();
            var histogram = new Mat();
            CvInvoke.EqualizeHist(img, histogram);
            pictureBox2.Image = histogram.ToBitmap();
        }

        private void ClaSheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var img = new Bitmap(pictureBox2.Image).ToImage<Gray, byte>();
            var output = new Mat();
            CvInvoke.CLAHE(img, 50, new Size(8, 8), output);
            pictureBox2.Image = output.ToBitmap();
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

        private VectorOfVectorOfPoint ExtractContours(Image<Bgr, byte> inputImage)
        {

            var temp = inputImage.SmoothGaussian(f_GaussianParam).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(f_BinarizationThreshold), new Gray(255));
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

        private void GaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var img = new Bitmap(pictureBox2.Image).ToImage<Gray, byte>();

            var gaussian = img.SmoothGaussian(f_GaussianParam).Convert<Gray, byte>();
            pictureBox1.Image = gaussian.ToBitmap();
        }

        private void BinarizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (f_ImgInput == null) return;
            var temp = f_ImgInput.SmoothGaussian(f_GaussianParam).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(f_BinarizationThreshold), new Gray(255));
            pictureBox1.Image = temp.ToBitmap();
        }

        private void ApplyConvertParams(int bt, int gp, int mAs, int mp, int zm, int oc)
        {
            f_BinarizationThreshold = bt;
            f_GaussianParam = gp;
            f_MaxAspectRatio = mAs;
            f_MinPerimeterLen = mp;
            f_Zoom = zm;
            f_ObjectCount = oc;
        }

        private void DetectParamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dParams = new DetectParams(f_BinarizationThreshold, f_GaussianParam, f_MaxAspectRatio, f_MinPerimeterLen, f_Zoom, f_ObjectCount);
            dParams.OnApplyParam += ApplyConvertParams;
            dParams.ShowDialog();
        }

        private async void RepeatDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var files = f_Storage.GetUndetectedItems;
            foreach (var file in files)
            {
                await PrepareFile(file);
            }
            listBox1.Items.Add("Finished directory ReEncode");
            listBox1.Items.Add($"Undetected: {f_Storage.GetUndetectedCount}");
        }

        private async void OpenSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var name = tvResults.SelectedNode.Name;
                await LoadFile(name);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private void clearResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f_Storage.ClearStorage();
            tvResults.Nodes.Clear();
        }

        private int[] BrightnessHorShear(IInputArray contour)
        {
            var rct = CvInvoke.FitEllipse(contour);
            var y = (int)rct.Center.Y;
            //var xn = rct.Center.X;
            var x0 = (int)(rct.Center.X - (rct.Size.Width + rct.Size.Height) / 4) - 30;
            var x1 = (int)(rct.Center.X + (rct.Size.Width + rct.Size.Height) / 4) + 30;
            var lst = new List<int>();

            for (var x = x0; x <= x1; x++)
            {
                lst.Add(GetPixelBrightness(y, x));
                //if (Math.Abs(x - xn) <= 10)
                //{
                //    continue;
                //}
                //var pixel = new Bgr(Color.AntiqueWhite);
                //f_ImgInput[y, x] = pixel;
            }
            return lst.ToArray();
        }

        private int[] BrightnessVertShear(VectorOfPoint contour)
        {
            var rct = CvInvoke.FitEllipse(contour);
            var rad = (rct.Size.Width + rct.Size.Height) / 4;
            var x = (int)rct.Center.X;
            //var yn=(int)rct.Center.Y;
            var y0 = (int)(rct.Center.Y - rad) - 30;
            var y1 = (int)(rct.Center.Y + rad) + 30;
            var lst = new List<int>();

            for (var y = y0; y <= y1; y++)
            {
                
                lst.Add(GetPixelBrightness(y, x));
                //if (Math.Abs(y - yn) > 10)
                //{
                //    var pixel = new Bgr(Color.AntiqueWhite);
                //    f_ImgInput[y, x] = pixel;
                //}
            }

            return lst.ToArray();
        }

        private int[] BrightnessMultyShear(VectorOfPoint contour)
        {


            var rct = CvInvoke.FitEllipse(contour);
            var rad = (rct.Size.Width + rct.Size.Height) / 4;
            var yn = (int)rct.Center.Y;
            var y0 = (int)(rct.Center.Y - rad) - 15;
            var y1 = (int)(rct.Center.Y + rad) + 15;

            var xn = (int)rct.Center.X;
            var x0 = (int)(rct.Center.X - rad) - 15;
            var x1 = (int)(rct.Center.X + rad) + 15;

            var lstx = new List<int>();
            for (var x = x0; x <= x1; x++)
            {
                lstx.Add(GetPixelBrightness(yn, x));
            }

            var lsty = new List<int>();
            for (var y = y0; y <= y1; y++)
            {
                lsty.Add(GetPixelBrightness(y, xn));
            }

            var lst = new List<int>();
            for (var i = 0; i < Math.Min(lstx.Count, lsty.Count); i++)
            { lst.Add((int)((lstx[i] + lsty[i]) / 2.0)); }

            return lst.ToArray();
        }

        private Dictionary<int, double> NormalizeBrightness(int[] data)
        {
            var tr = 0d;
            var centerIndex = (int)(data.Length / 2.0);
            var t0 = data[centerIndex];
            for (var i = 0; i < 5; i++)
            {
                tr += data[i] + data[data.Length - 1 - i];
            }
            tr /= 10.0;

            var res = new Dictionary<int, double>();
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] - tr <= 0) res.Add(i - centerIndex, 0d);
                else res.Add(i - centerIndex, (data[i] - tr) / (t0 - tr));
            }
            return res;
        }

        private int GetPixelBrightness(int x, int y)
        {
            if (x > 0 && y > 0 && x < f_ImgInput.Size.Width && y < f_ImgInput.Size.Height)
            {
                var pixel = f_ImgInput[x, y];
                return (int)(pixel.Green);
                //return (int)(pixel.Red + pixel.Green + pixel.Blue);
            }

            return 0;
        }

        private void SaveDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f_Storage.SaveAllDetail();
        }
    }
}



//let src = cv.imread('canvasInput');
//let dst = cv.Mat.zeros(src.rows, src.cols, cv.CV_8UC3);
//cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
//cv.threshold(src, src, 100, 200, cv.THRESH_BINARY);
//let contours = new cv.MatVector();
//let hierarchy = new cv.Mat();
//cv.findContours(src, contours, hierarchy, cv.RETR_CCOMP, cv.CHAIN_APPROX_SIMPLE);
//let hull = new cv.Mat();
//let defect = new cv.Mat();
//let cnt = contours.get(0);
//let lineColor = new cv.Scalar(255, 0, 0);
//let circleColor = new cv.Scalar(255, 255, 255);
//cv.convexHull(cnt, hull, false, false);
//cv.convexityDefects(cnt, hull, defect);
//for (let i = 0; i < defect.rows; ++i)
//{
//    let start = new cv.Point(cnt.data32S[defect.data32S[i * 4] * 2],
//                             cnt.data32S[defect.data32S[i * 4] * 2 + 1]);
//    let end = new cv.Point(cnt.data32S[defect.data32S[i * 4 + 1] * 2],
//                           cnt.data32S[defect.data32S[i * 4 + 1] * 2 + 1]);
//    let far = new cv.Point(cnt.data32S[defect.data32S[i * 4 + 2] * 2],
//                           cnt.data32S[defect.data32S[i * 4 + 2] * 2 + 1]);
//    cv.line(dst, start, end, lineColor, 2, cv.LINE_AA, 0);
//    cv.circle(dst, far, 3, circleColor, -1);
//}
//cv.imshow('canvasOutput', dst);
//src.delete(); dst.delete(); hierarchy.delete(); contours.delete(); hull.delete(); defect.delete();