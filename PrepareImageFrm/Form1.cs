using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
        private string f_CurrentFile;
        private bool f_ShowInImgBox;
        private List<ImageResult> f_ImageResult;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            f_ImageResult = new List<ImageResult>();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    RestoreDirectory = true
                };
                if (dialog.ShowDialog() != DialogResult.OK) return;
                f_ShowInImgBox = true;
                f_CurrentFile = dialog.FileName;
                OpenFileAsync();
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private void DetectShapesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (f_ImgInput == null)
            {
                return;
            }
            else
            {
                try
                {
                    //var temp = f_ImgInput.Convert<Gray, byte>().ThresholdBinaryInv(new Gray(80), new Gray(255));
                    //VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    //Mat m = new Mat();
                    //CvInvoke.FindContours(image: temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);
                    
                    var tmpCnt =ExtractContours();
                    var contours = FilterContours(tmpCnt);
                    if (contours.Size == 2)
                    {
                        listBox1.Items.Add($"{Path.GetFileNameWithoutExtension(f_CurrentFile)}:{GetDistanceBeforeCenter(contours[0], contours[1])}");
                    }
                    else
                    {
                        listBox1.Items.Add( $"{Path.GetFileNameWithoutExtension(f_CurrentFile)}: Contours count not is two");
                    }
                    for (int i = 0; i < contours.Size; i++)
                    {
                        double perimeter = CvInvoke.ArcLength(contours[i], true);
                        VectorOfPoint approx = new VectorOfPoint();
                        CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                        CvInvoke.DrawContours(f_ImgInput, contours, i, new MCvScalar(0, 0, 255));
                    }
                    pictureBox1.Image = f_ImgInput.AsBitmap();
                    //pictureBox2.Image = temp.AsBitmap();
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add( ex.Message);
                }
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

        float GetDistanceBeforeCenter(IInputArray a, IInputArray b)
        {
            var rctA = CvInvoke.FitEllipse(a);
            var rctB = CvInvoke.FitEllipse(b);
            return GetDistance(rctA.Center, rctB.Center);
        }

        float GetDistance(PointF a, PointF b)
        {

            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        }

        private void SavePreparedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save(@"output.jpeg", ImageFormat.Jpeg);
        }

        private void PrepareFile(string filename)
        {
            //string res;
            try
            {
                f_CurrentFile = filename;
                //OpenFileAsync();
                f_ImgInput = new Image<Bgr, byte>(filename);
                var tmpCnt = ExtractContours();
                var contours = FilterContours(tmpCnt);
                if (contours.Size == 2)
                {
                    listBox1.Items.Add($"{Path.GetFileNameWithoutExtension(f_CurrentFile)}:{GetDistanceBeforeCenter(contours[0], contours[1])}");
                }
                else
                {
                    listBox1.Items.Add($"{Path.GetFileNameWithoutExtension(f_CurrentFile)}: Contours count not is two");
                }
                f_ImgInput.Dispose();
                //var temp = f_ImgInput.SmoothGaussian(15).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(80), new Gray(255));
                //var contours = new VectorOfVectorOfPoint();
                //var m = new Mat();
                //CvInvoke.FindContours(image: temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);
                //temp.Dispose();
                //var filteredContours = new VectorOfVectorOfPoint();
                //for (var i = 0; i < contours.Size; i++)
                //{
                //    var perimeter = CvInvoke.ArcLength(contours[i], true);
                //    var approx = new VectorOfPoint();
                //    CvInvoke.ApproxPolyDP(contours[i], approx, 0.1 * perimeter, true);
                //    CvInvoke.DrawContours(f_ImgInput, contours, i, new MCvScalar(0, 0, 255));
                //    if (contours[i].Size < 5) continue;
                //    var rct = CvInvoke.FitEllipse(contours[i]);
                //    if (GetAspectRatio(rct)<0.33f)
                //        filteredContours.Push(contours[i]);
                //}
                ////contours.
                //if (filteredContours.Size == 2)
                //{
                //    var rectA = new RotatedRect();
                //    var rectB = new RotatedRect();
                //    if (filteredContours[0].Size>=5) rectA = CvInvoke.FitEllipse(filteredContours[0]);
                //    if (filteredContours[1].Size>=5) rectB = CvInvoke.FitEllipse(filteredContours[1]);

                //    var aspectA = GetAspectRatio(rectA);
                //    var aspectB = GetAspectRatio(rectB);
                //    if ((aspectA < 0.33f) && (aspectB < 0.33f)) {
                //        res =
                //            $"{filename}:{rectA.Center.X}:{rectA.Center.Y}:{rectA.Size.Width}:{rectA.Size.Height}" +
                //        $":{rectB.Center.X}:{rectB.Center.Y}:{rectB.Size.Width}:{rectB.Size.Height}" +
                //        $":{GetDistanceBeforeCenter(contours[0], contours[1])}";
                //    }
                //    else
                //    {
                //        res = $"{filename}: Contours count not is two";
                //    }
                //}
                //else
                //{
                //    res = $"{filename}: Contours count not is two";
                //}
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"{Path.GetFileNameWithoutExtension(filename)}: {ex.Message}");
                //res = $"{filename}: {ex.Message}";
            }

           // return res;
        }

        private void DirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var files = Directory.GetFiles(dialog.SelectedPath);
                    foreach (var file in files)
                    {
                        //var tmpRes = 
                            PrepareFile(file);
                        //listBox1.Items.Add(tmpRes);
                    }

                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        void SaveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFile = new System.IO.StreamWriter($"{DateTime.Now.ToShortDateString()}.csv");
            foreach (var item in listBox1.Items)
            {
                saveFile.WriteLine(item.ToString());
            }
        }

        void OpenFileAsync()
        {
            try
            {
                var bw = new BackgroundWorker
                {
                    WorkerReportsProgress = false,
                    WorkerSupportsCancellation = false,
                   
                };
                bw.DoWork += Bw_DoWork;
                bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
                bw.RunWorkerAsync();
            }
            catch (Exception e)
            {
                listBox1.Items.Add(e.Message);
                
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (f_ShowInImgBox)
                pictureBox2.Image = f_ImgInput.AsBitmap();
            else
            {
                var tmpCnt = ExtractContours();
                var contours = FilterContours(tmpCnt);
                if (contours.Size == 2)
                {
                    listBox1.Items.Add($"{Path.GetFileNameWithoutExtension(f_CurrentFile)}:{GetDistanceBeforeCenter(contours[0], contours[1])}");
                }
                else
                {
                    listBox1.Items.Add( $"{Path.GetFileNameWithoutExtension(f_CurrentFile)}: Contours count not is two");
                }
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            f_ImgInput = new Image<Bgr, byte>(f_CurrentFile);
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
                if ((GetAspectRatio(rct) < 0.33f) && (perimeter>150.0f))
                    filteredContours.Push(contours[i]);
            }
            return filteredContours;
        }

        private VectorOfVectorOfPoint ExtractContours()
        {
            
            var temp = f_ImgInput.Convert<Gray, byte>().ThresholdBinaryInv(new Gray(90),new Gray(255));
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
            var gaussian = img.SmoothGaussian(15).Convert<Gray, byte>();
            pictureBox2.Image = gaussian.ToBitmap();
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