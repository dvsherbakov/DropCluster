using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace PrepareImageFrm
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> f_ImgInput;
        private int f_BinarizationThreshold = 130;
        private int f_GaussianParam = 5;
        private int f_MaxAspectRatio = 33;
        private int f_MinPerimeterLen = 150;
        private int f_Zoom = 100;
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

                    CvInvoke.DrawContours(f_ImgInput, contours, i, new MCvScalar(0, 0, 255));
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
            pictureBox1.Image.Save(@"output.jpeg", ImageFormat.Jpeg);
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
            var result = f_Storage.AddToStore(new ImageResult(fileName, contours));
            if (result.Pass == 1)
                tvResults.Nodes.Add(result.GetResultNode());
            else
            {
                tvResults.Nodes.Remove(
                    tvResults.Nodes.Find(result.FileName, false).FirstOrDefault() ??
                    throw new InvalidOperationException());
                tvResults.Nodes.Add(result.GetResultNode());
            }
        }

        private async  void DirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new FolderBrowserDialog {
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
            CvInvoke.CLAHE(img, 50, new Size(8,8), output);
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
                if ((GetAspectRatio(rct) < f_MaxAspectRatio/100f) && (perimeter> f_MinPerimeterLen))
                    filteredContours.Push(contours[i]);
            }
            return filteredContours;
        }

        private VectorOfVectorOfPoint ExtractContours(Image<Bgr, byte> inputImage)
        {
            
            var temp = inputImage.SmoothGaussian(f_GaussianParam).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(f_BinarizationThreshold), new Gray(255));
            var contours = new VectorOfVectorOfPoint();
            var m = new Mat();
            CvInvoke.FindContours(image: temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);
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

        private void ApplyConvertParams(int bt, int gp, int mAs, int mp, int zm)
        {
            f_BinarizationThreshold = bt;
            f_GaussianParam = gp;
            f_MaxAspectRatio = mAs;
            f_MinPerimeterLen = mp;
            f_Zoom = zm;
        }
        
        private void DetectParamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dParams = new DetectParams(f_BinarizationThreshold, f_GaussianParam, f_MaxAspectRatio, f_MinPerimeterLen, f_Zoom);
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