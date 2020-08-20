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
        Image<Bgr, byte> imgInput;
        string currentFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    RestoreDirectory = true
                };
                if (dialog.ShowDialog()== DialogResult.OK)
                {
                    imgInput = new Image<Bgr, byte>(dialog.FileName);
                    pictureBox1.Image = imgInput.Bitmap;
                    currentFile = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Insert(0, ex.Message);
            }
        }

        private void detectShapesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgInput==null)
            {
                return;
            } else
            {
                try
                {
                    var temp = imgInput.SmoothGaussian(15).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(90),new Gray(255));
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    Mat m = new Mat();
                    CvInvoke.FindContours(image: temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);
                    for (int i = 0; i < contours.Size; i++)
                    {
                        double perimeter = CvInvoke.ArcLength(contours[i], true);
                        VectorOfPoint approx = new VectorOfPoint();
                        CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                        CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(0, 0, 255));
                        var rct = CvInvoke.FitEllipse(contours[i]);
                        var aspct = getAspectRatio(rct);
                        if (aspct < 0.33f)
                            listBox1.Items.Insert(0, $"X: {rct.Center.X}, Y: {rct.Center.Y}, Width: {rct.Size.Width}, Height: {rct.Size.Height}, Aspect: {getAspectRatio(rct)}");
                    }
                    if (contours.Size == 2)
                    {
                        listBox1.Items.Insert(0, getDistanceBeforeCntr(contours[0], contours[1]));
                    }
                    else
                    {
                        listBox1.Items.Insert(0, $"{currentFile}: Contours count not is two");
                    }

                    pictureBox2.Image = imgInput.Bitmap;
                    //pictureBox2.Image = temp.Bitmap;
                }
                catch (Exception ex)
                {
                    listBox1.Items.Insert(0, ex.Message);
                }
            }
        }
        private float getAspectRatio(RotatedRect rct)
        {
            try
            {
                return 1 - (rct.Size.Width / rct.Size.Height);
            }
            catch (Exception ex)
            {
                listBox1.Items.Insert(0, ex.Message);
                return 1.0f;
            }
        }

        private float getDistanceBeforeCntr(VectorOfPoint a, VectorOfPoint b)
        {
            var rctA = CvInvoke.FitEllipse(a);
            var rctB = CvInvoke.FitEllipse(b);
            return getDistance(rctA.Center, rctB.Center);
        }

        private float getDistance(PointF a, PointF b)
        {

            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        }

        private void savePreparedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image.Save(@"ouput.jpeg", ImageFormat.Jpeg);
        }

        private string PrepareFile(string filename)
        {
            string res;
            try
            {
                imgInput = new Image<Bgr, byte>(filename);
                pictureBox1.Image = imgInput.Bitmap;
                currentFile = filename;

                var temp = imgInput.SmoothGaussian(15).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(80), new Gray(255));
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat m = new Mat();
                CvInvoke.FindContours(image: temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);
                for (int i = 0; i < contours.Size; i++)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.03 * perimeter, true);
                    CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(0, 0, 255));
                }

                if (contours.Size == 2)
                {
                    var rctA = CvInvoke.FitEllipse(contours[0]);
                    var rctB = CvInvoke.FitEllipse(contours[1]);
                    var aspctA = getAspectRatio(rctA);
                    var aspctB = getAspectRatio(rctB);
                    if ((aspctA < 0.33f) && (aspctB < 0.33f)) {
                        res =
                            $"{filename}:{rctA.Center.X}:{rctA.Center.Y}:{rctA.Size.Width}:{rctA.Size.Height}" +
                        $":{rctB.Center.X}:{rctB.Center.Y}:{rctB.Size.Width}:{rctB.Size.Height}" +
                        $":{getDistanceBeforeCntr(contours[0], contours[1])}";
                    }
                    else
                    {
                        res = $"{currentFile}: Contours count not is two";
                    }
                }
                else
                {
                    res = $"{currentFile}: Contours count not is two";
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"{filename}: {ex.Message}");
                res = $"{filename}: {ex.Message}";
            }

            return res;
        }

        private void dirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var files = Directory.GetFiles(dialog.SelectedPath);
                    foreach (var file in files)
                    {
                        var tmpRes = PrepareFile(file);
                        listBox1.Items.Add(tmpRes);
                    }

                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Insert(0, ex.Message);
            }
        }

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter SaveFile = new System.IO.StreamWriter($"{DateTime.Now.ToShortDateString()}.csv");
            foreach (var item in listBox1.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
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