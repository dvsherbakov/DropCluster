using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace TestEMGU1
{
    public partial class FmMain : Form
    {
        private bool IsStop { get; set; }
        private CircleF[] _circles;
        private int _mouseClickNo;
        private readonly List<PointListItem> _listCircles = new List<PointListItem>();
        private readonly List<PointF> _polygon = new List<PointF>();
        private float _yScale;
        private float _xScale;
        private readonly List<int> _clickedPoint = new List<int>();
        private PointF[] _canArea = new PointF[0];

        public FmMain()
        {
            InitializeComponent();
            _mouseClickNo = 0;
            Width = Properties.Settings.Default.Width;
            Height = Properties.Settings.Default.Height;
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            tbDropCount.Value = Properties.Settings.Default.DropCount;
            tbMinimalDist.Value = Properties.Settings.Default.MinimalDist;
            tbGradient.Value = Properties.Settings.Default.GradientZone;
            tbCurvature.Value = Properties.Settings.Default.Curvature;
            tbMinRadius.Value = Properties.Settings.Default.MinRadius;
            tbMinRadius.Value = Properties.Settings.Default.MinRadius;
            tbMaxRadius.Value = Properties.Settings.Default.MaxRadius;
            tbRadCount.Text = Properties.Settings.Default.RadCount;
            //pictureBox1.BackColor = Color.FromArgb(255, 128, 128);
            TransparencyKey = Color.FromArgb(255, 128, 128);
            IsStop = false;

            if (IntPtr.Size != 8)
            {
                throw new Exception(@"Change VS options to ensure 64bit IIS Express");
            }
            InitAngleList();
            FillGradeList();
        }

        private void PrepareDir()
        {
            var fInfo = new FileInfo(tbSingleFile.Text);

            if (fInfo.Directory == null) return;
            var dirName = fInfo.Directory.FullName;
            // listBox2.Items.Clear();
            var d = new DirectoryInfo(dirName); //Assuming Test is your Folder

            var files = d.GetFiles("c_*.jpg");
            foreach (var f in files) f.Delete();

            files = d.GetFiles("*.jpg");
            progressBar1.Value = 0;
            progressBar1.Maximum = files.Length;
            var strList = new List<string>();
            var dropCount = tbDropCount.Value;
            Parallel.ForEach(files.OrderBy(x => x.Name), file =>
            {
                if (IsStop) return;
                var lst = PreparePicture(file.FullName, dropCount);
                Invoke(new AddMessageDelegate(IncValue));
                var tstr = lst.Aggregate("", (current, t) => current + t.Radius() + ":");
                strList.Add(file.Name + ":" + tstr);
            });

            using (var sw = new StreamWriter(dirName + @"\stat.lst"))
            {
                foreach (var t in strList.OrderBy(x => x))
                    sw.WriteLine(t.Replace(',', '.'));
            }
        }

        private delegate void AddMessageDelegate();

        private void IncValue()
        {
            progressBar1.Value++;
        }

        private IEnumerable<BallElement> PreparePicture(string filename, int dropCount)
        {
            var lst = new List<BallElement>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var bmp = new Bitmap(filename);
            //var g = Graphics.FromImage(bmp);

            var img = new Image<Bgr, byte>(bmp);
            var uimage = new UMat();

            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            _circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1, 15, 75, 40, 25, 85);

            var circleImage = img.Copy(); //CopyBlank();

            foreach (var circle in _circles.OrderByDescending(x => x.Radius).Take(dropCount))
            {
                circleImage.Draw(circle, new Bgr(Color.Brown), 2);
                lst.Add(new BallElement(circle.Area, circle.Center.X, circle.Center.Y, circle.Radius));
            }
            var f = new FileInfo(filename);
            var fn = f.DirectoryName + @"\c_" + f.Name;
            circleImage.Save(fn);

            return lst;
        }


        private void FmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Counter++;
            Properties.Settings.Default.Width = Width;
            Properties.Settings.Default.Height = Height;
            Properties.Settings.Default.Left = Left;
            Properties.Settings.Default.Top = Top;
            Properties.Settings.Default.DropCount = tbDropCount.Value; //default 15
            Properties.Settings.Default.MinimalDist = tbMinimalDist.Value; //default 20
            Properties.Settings.Default.GradientZone = tbGradient.Value; //default 130
            Properties.Settings.Default.Curvature = tbCurvature.Value; //default 40
            Properties.Settings.Default.MinRadius = tbMinRadius.Value; //default 25
            Properties.Settings.Default.MinRadius = tbMinRadius.Value; //default 25
            Properties.Settings.Default.MaxRadius = tbMaxRadius.Value; //default 65
            Properties.Settings.Default.RadCount = tbRadCount.Text; //default 65
            Properties.Settings.Default.Save();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var opf = new OpenFileDialog();
            if (opf.ShowDialog() == DialogResult.OK)
            {
                tbSingleFile.Text = opf.FileName;
            }
            TestPicture(tbSingleFile.Text);
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = tbMinimalDist.Value.ToString();
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = tbGradient.Value.ToString();
        }

        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            label3.Text = tbCurvature.Value.ToString();
        }

        private void TrackBar4_Scroll(object sender, EventArgs e)
        {
            label4.Text = tbMinRadius.Value.ToString();
        }

        private void TrackBar5_Scroll(object sender, EventArgs e)
        {
            label5.Text = tbMaxRadius.Value.ToString();
        }

        private void TestPicture(string filename)
        {
            var bmp = new Bitmap(filename);
            _listCircles.Clear();

            var img = new Image<Bgr, byte>(bmp);
            var uimage = new UMat();

            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);
            var minDist = tbMinimalDist.Value;
            var param1 = tbGradient.Value;
            var param2 = tbCurvature.Value;
            var minRadius = tbMinRadius.Value;
            var maxRadius = tbMaxRadius.Value;

            _circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1, minDist, param1,
                param2, minRadius, maxRadius);

            var circleImage = img.Copy(); //CopyBlank();
            pbOnePict.SizeMode = PictureBoxSizeMode.Zoom;
            pbOnePict.Image = circleImage.Bitmap;
            var pInfo = pbOnePict.GetType().GetProperty("ImageRectangle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pInfo != null)
            {
                var rectangle = (Rectangle)pInfo.GetValue(pbOnePict, null);
                _yScale = rectangle.Height / (float)circleImage.Bitmap.Size.Height;
                _xScale = rectangle.Width / (float)circleImage.Bitmap.Size.Width;
            }
            for (var i = 0; i < _circles.Length; i++)
            {
                var circle = _circles[i];
                circleImage.Draw(circle, new Bgr(Color.Brown), 2);

                circleImage.Draw(i.ToString(), new Point((int)circle.Center.X, (int)circle.Center.Y), FontFace.HersheyComplex, 2.0, new Bgr(Color.Brown));
                var fPt = new PointF(circle.Center.X * _xScale, circle.Center.Y * _yScale);
                _listCircles.Add(new PointListItem(i, fPt));
            }
        }

        private static double Angle_point(PointF a, PointF b, PointF c)
        {
            double x1 = a.X - b.X, x2 = c.X - b.X;
            double y1 = a.Y - b.Y, y2 = c.Y - b.Y;
            var d1 = Math.Sqrt(x1 * x1 + y1 * y1);
            var d2 = Math.Sqrt(x2 * x2 + y2 * y2);
            return Math.Acos((x1 * x2 + y1 * y2) / (d1 * d2));
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            PrepareDir();
        }

        private void TbDropCount_Scroll(object sender, EventArgs e)
        {
            lbdropCount.Text = tbDropCount.Value.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            IsStop = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var point1 = 0; var point2 = 0; var point3 = 0;
            try
            {
                point1 = int.Parse(tbPoint1.Text);
                point2 = int.Parse(tbPoint2.Text);
                point3 = int.Parse(tbPoint3.Text);
            }
            finally
            {
                var angle = Angle_point(_circles[point1].Center, _circles[point2].Center, _circles[point3].Center);
                angle = angle * 180 / Math.PI;
                if (angle > 180)
                {
                    angle = 360 - angle;
                }
                var fi = new FileInfo(tbSingleFile.Text);
                var lvItem = new ListViewItem(fi.Name);
                lvItem.SubItems.Add(point1.ToString());
                lvItem.SubItems.Add(_circles[point1].Radius.ToString("F3"));
                lvItem.SubItems.Add(point2.ToString());
                lvItem.SubItems.Add(_circles[point2].Radius.ToString("F3"));
                lvItem.SubItems.Add(point3.ToString());
                lvItem.SubItems.Add(_circles[point3].Radius.ToString("F3"));
                lvItem.SubItems.Add(angle.ToString("F8"));

                listView1.Items.Add(lvItem);
            }
            tbPoint1.Text = "";
            tbPoint2.Text = "";
            tbPoint3.Text = "";
        }

        private void InitAngleList()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("Имя файла", 180, HorizontalAlignment.Center);
            listView1.Columns.Add("Первая точка", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Radius", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("Центр", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Radius", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("Третья точка", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Radius", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("Угол", 150, HorizontalAlignment.Center);
        }

        private void BtDelItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                listView1.Items.Remove(eachItem);
            }
        }

        private static string SubItemToString(ListViewItem item)
        {
            return item.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate("", (current, si) => current + (si.Text + ":"));
        }

        private void BtSaveToFile_Click(object sender, EventArgs e)
        {
            var fi = new FileInfo(tbSingleFile.Text);
            var filePath = fi.DirectoryName + @"\result_" + DateTime.Now.ToShortDateString() + "_.txt";
            StreamWriter sw;
            using (sw = new StreamWriter(filePath))
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    sw.WriteLine(SubItemToString(item).Replace(',', '.'));
                }
            }
            sw.Close();
        }

        private void PbOnePict_Click(object sender, EventArgs e)
        {
            var t = (MouseEventArgs)e;
            var pInfo = pbOnePict.GetType().GetProperty("ImageRectangle",
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            if (pInfo == null) return;
            var rectangle = (Rectangle)pInfo.GetValue(pbOnePict, null);
            if (cbAngles.Checked)
            {
                if (t.Button == MouseButtons.Left)
                {
                    var cp = new PointF(t.X - rectangle.X, t.Y);
                    var et = _listCircles.OrderBy(x => x.GetDistance(cp)).FirstOrDefault();
                    if (et == null) return;
                    switch (_mouseClickNo)
                    {
                        case 0:
                            tbPoint1.Text = et.Id().ToString();
                            break;
                        case 1:
                            tbPoint2.Text = et.Id().ToString();
                            break;
                        case 2:
                            tbPoint3.Text = et.Id().ToString();
                            break;
                        default:
                            _mouseClickNo = 0;
                            break;
                    }
                    _mouseClickNo++;
                    if (_mouseClickNo > 2) _mouseClickNo = 0;
                }
                else
                {
                    Button2_Click(sender, e);
                }
            }
            else
            {
                if (t.Button == MouseButtons.Left)
                {
                    var cp = new PointF(t.X - rectangle.X, t.Y);
                    var et = _listCircles.OrderBy(x => x.GetDistance(cp)).FirstOrDefault();
                    if (et == null) return;
                    _polygon.Add(_circles[et.Id()].Center);
                    _clickedPoint.Add(et.Id());
                }
                else
                {
                    using (var g = pbOnePict.CreateGraphics())
                    {
                        for (var i = 1; i < _polygon.Count; i++)
                        {
                            g.DrawLine(Pens.Blue, new PointF(_polygon[i].X * _xScale+ rectangle.X, _polygon[i].Y * _yScale), new PointF(_polygon[i - 1].X * _xScale+ rectangle.X, _polygon[i - 1].Y * _yScale));
                        }
                        g.DrawLine(Pens.Blue, new PointF(_polygon[0].X * _xScale+ rectangle.X, _polygon[0].Y * _yScale), new PointF(_polygon[_polygon.Count - 1].X * _xScale+ rectangle.X, _polygon[_polygon.Count - 1].Y * _yScale));
                    }

                    _canArea = _polygon.ToArray();
                    var fi = new FileInfo(tbSingleFile.Text);
                    if (cbArea.Checked)
                    {
                        foreach (var cp in _clickedPoint)
                        {
                            var cir = _circles[cp];
                            var lvItem = new ListViewItem(cp.ToString());
                            lvItem.SubItems.Add(cir.Radius.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.X.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.Y.ToString("F6"));
                            
                            lvItem.SubItems.Add(fi.Name);
                            lvArea.Items.Add(lvItem);
                        }
                        var pi = new PointInArea();
                        for (var i = 0; i < _circles.Length; i++)
                        {
                            var cir = _circles[i];
                            var ts = pi.IsPointInside(_polygon.ToArray(), cir.Center);
                            if (!ts) continue;
                            if (_clickedPoint.Any(x => x == i)) continue;
                            var lvItem = new ListViewItem(i.ToString());
                            lvItem.SubItems.Add(cir.Radius.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.X.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.Y.ToString("F6"));
                            _clickedPoint.Add(i);
                            //var fi = new FileInfo(tbSingleFile.Text);
                            lvItem.SubItems.Add(fi.Name);
                            lvArea.Items.Add(lvItem);
                        }
                        PrepareLinks(_clickedPoint.Select(c => new PointListItem(c, _circles[c].Center)).ToList());
                        lbAvgRad.Text =
                            $@"Средний радиус: {
                                    _clickedPoint.Select(c => _circles[c].Radius).ToList().Average(x => x)
                                :F5}";
                        _polygon.Clear();
                        _clickedPoint.Clear();
                    } else if (cbChains.Checked)
                    {
                        for (var i = 1; i < _clickedPoint.Count; i++)
                        {
                            var itm = _clickedPoint[i];
                            var chItem = new ListViewItem(i.ToString());
                            chItem.SubItems.Add(fi.Name);

                            lvChains.Items.Add(chItem);
                        }
                    }
                }
            }
        }

        private void PrepareLinks(IEnumerable<PointListItem> lst)
        {
            var listPoints = lst as IList<PointListItem> ?? lst.ToList();
            var summ = listPoints.Sum(it => _circles[it.Id()].Radius);
            var rc = float.Parse(tbRadCount.Text);
            var avg = (summ / listPoints.Count) * rc;
            var lnkCount = 0;
            var lnkAvg = 0.0d;
            while (listPoints.Count > 1)
            {
                var item = listPoints.FirstOrDefault();
                listPoints.Remove(item);
                if (item == null) return;
                var tl = listPoints.OrderBy(x => x.GetDistance(item.GetPoint())).Take(6)
                    .Where(x => x.GetDistance(item.GetPoint()) < avg);
                foreach (var lnk in tl)
                {
                    lbLinks.Items.Add($"{item.Id()}<->{lnk.Id()}:{lnk.GetDistance(item.GetPoint()):F5}");
                    lnkCount++;
                    lnkAvg += lnk.GetDistance(item.GetPoint());
                }
            }
            lnkAvg = lnkAvg / lnkCount;
            lbLinks.Items.Add($"Всего:{lnkCount}; Средн:{lnkAvg}");
        }

        private void TrackBarGrade_Scroll(object sender, EventArgs e)
        {
            lbGrade.Text = @"Градация " + trackBarGrade.Value.ToString();
            FillGradeList();
        }

        private void FillGradeList()
        {
            var aList = new Dictionary<int, double>();
            for (var i = 0; i < listView1.Items.Count; i++)
            {
                aList.Add(i, double.Parse(listView1.Items[i].SubItems[7].Text.Replace('.', ',')));
            }
            lvGrade.Items.Clear();
            lvGrade.Columns.Clear();
            lvGrade.Columns.Add(@"#");
            chart1.Series[0].Points.Clear();
            var lvItem = new ListViewItem(@"#");

            const double coef = 180 / 128.0;
            const double hcoef = coef / 2.0;

            for (var i = 90.0; i <= 180; i += trackBarGrade.Value / coef)
            {
                var i1 = i;
                var t = aList.Count(x => x.Value >= i1 - trackBarGrade.Value / hcoef && x.Value < i1 + trackBarGrade.Value / hcoef);
                lvGrade.Columns.Add(i.ToString(CultureInfo.InvariantCulture), 40);
                lvItem.SubItems.Add(t.ToString());
                chart1.Series[0].Points.AddXY(i1, t);
            }
            lvGrade.Items.Add(lvItem);
            panel2.Width = flpStat.Width - panel1.Width - 30;
            panel3.Width = flpStat.Width - 20;
        }

        private void BtLoad_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog { Filter = @"Файлы txt|*.txt" };
            if (fd.ShowDialog() != DialogResult.OK) return;
            var filePath = fd.FileName;
            StreamReader sr;
            using (sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    var lineFs = sr.ReadLine();
                    if (lineFs == null) continue;
                    var sl = lineFs.Split(':');

                    var lvItem = new ListViewItem(sl[0]);
                    lvItem.SubItems.Add(sl[1]);
                    lvItem.SubItems.Add(sl[2]);
                    lvItem.SubItems.Add(sl[3]);
                    lvItem.SubItems.Add(sl[4]);
                    lvItem.SubItems.Add(sl[5]);
                    lvItem.SubItems.Add(sl[6]);
                    lvItem.SubItems.Add(sl[7]);
                    listView1.Items.Add(lvItem);
                }
            }
        }

        private void CbArea_CheckedChanged(object sender, EventArgs e)
        {
            cbAngles.Checked = false;
        }

        private void CbAngles_CheckedChanged(object sender, EventArgs e)
        {
            cbArea.Checked = false;
        }

        private void BtSaveArea_Click(object sender, EventArgs e)
        {
            var fi = new FileInfo(tbSingleFile.Text);
            var filePath = fi.DirectoryName + @"\res_area_" + DateTime.Now.ToShortDateString() + "_.txt";
            StreamWriter sw;
            using (sw = new StreamWriter(filePath))
            {
                foreach (ListViewItem item in lvArea.Items)
                {
                    sw.WriteLine(SubItemToString(item).Replace(',', '.'));
                }
                sw.WriteLine(lbAvgRad.Text.Replace(',', '.'));
            }
            sw.Close();

            filePath = fi.DirectoryName + @"\res_links_" + DateTime.Now.ToShortDateString() + "_.txt";
            using (sw = new StreamWriter(filePath))
            {
                foreach (string item in lbLinks.Items)
                {
                    sw.WriteLine(item.Replace(',', '.'));
                }
            }
            sw.Close();
        }

        private void BtStatSave_Click(object sender, EventArgs e)
        {
            var fi = new FileInfo(tbSingleFile.Text);
            var filePath = fi.DirectoryName + @"\res_stat_" + DateTime.Now.ToShortDateString() + "_.txt";
            StreamWriter sw;
            var headerStr = lvGrade.Columns.Cast<ColumnHeader>().Aggregate("", (current, hd) => current + (hd.Text + ":"));
            using (sw = new StreamWriter(filePath))
            {
                sw.WriteLine(headerStr);
                foreach (ListViewItem item in lvGrade.Items)
                {
                    sw.WriteLine(SubItemToString(item).Replace(',', '.'));
                }
            }
            sw.Close();
        }

        private void BtAreaClear_Click(object sender, EventArgs e)
        {
            lbLinks.Items.Clear();
            lvArea.Items.Clear();
        }

        private void Label11_Click(object sender, EventArgs e)
        {
            tbDropCount.Value = 15;
        }

        private void Label10_Click(object sender, EventArgs e)
        {
            tbMinimalDist.Value = 20;
        }

        private void Label9_Click(object sender, EventArgs e)
        {
            tbGradient.Value = 130;
        }

        private void Label8_Click(object sender, EventArgs e)
        {
            tbCurvature.Value = 40;
        }

        private void Label7_Click(object sender, EventArgs e)
        {
            tbMinRadius.Value = 25;
        }

        private void Label6_Click(object sender, EventArgs e)
        {
            tbMaxRadius.Value = 65;
        }

        private void pbOnePict_Paint(object sender, PaintEventArgs e)
        {
            if (_canArea.Length < 3) return;
            var pInfo = pbOnePict.GetType().GetProperty("ImageRectangle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pInfo == null) return;
            var rectangle = (Rectangle)pInfo.GetValue(pbOnePict, null);

            for (var i = 1; i < _canArea.Length; i++)
            {
                e.Graphics.DrawLine(Pens.Blue, new PointF(_canArea[i].X * _xScale + rectangle.X, _canArea[i].Y * _yScale), new PointF(_canArea[i - 1].X * _xScale + rectangle.X, _canArea[i - 1].Y * _yScale));
            }
            e.Graphics.DrawLine(Pens.Blue, new PointF(_canArea[0].X * _xScale + rectangle.X, _canArea[0].Y * _yScale), new PointF(_canArea[_canArea.Length - 1].X * _xScale + rectangle.X, _canArea[_canArea.Length - 1].Y * _yScale));
        }
    }
}
