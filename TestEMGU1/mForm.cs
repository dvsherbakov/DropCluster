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
using System.Reflection;

namespace TestEMGU1
{
    public partial class FmMain : Form
    {
        private CircleF[] m_Circles;
        private int m_MouseClickNo;
        private readonly List<PointListItem> m_ListCircles = new List<PointListItem>();
        private readonly List<PointF> m_Polygon = new List<PointF>();
        private float m_YScale;
        private float m_XScale;
        private readonly List<int> m_ClickedPoint = new List<int>();
        private PointF[] m_CanArea = new PointF[0];
        private readonly List<PointListItem> m_Branched = new List<PointListItem>();

        public FmMain()
        {
            InitializeComponent();
            m_MouseClickNo = 0;
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
            TransparencyKey = Color.FromArgb(255, 128, 128);

            if (IntPtr.Size != 8)
            {
                throw new Exception(@"Change VS options to ensure 64bit IIS Express");
            }
            InitAngleList();
            FillGradeList();
        }

        //private static List<BallElement> CompareToPrev(List<BallElement>prev, List<BallElement> lst)
        //{
        //    return prev.Select(pElem => lst.OrderBy(x => x.Range(pElem.getX(), pElem.getY())).FirstOrDefault()).ToList();
        //}

        private static IEnumerable<BallElement> SortCoord(IList<BallElement> prv, IList<BallElement> cur)
        {
            var lRes = new List<BallElement>();
            if (cur.Count > 0)
            {
                const int cPrev = 0;
                while (prv.Count > 0)
                {
                    var minDist = float.MaxValue;
                    var marker = 0;
                    for (var i = 0; i < cur.Count; i++)
                    {
                        var dst = prv[cPrev].Range(cur[i].GetX(), cur[i].GetY());
                        if (!(dst < minDist)) continue;
                        minDist = dst;
                        marker = i;
                    }
                    lRes.Add(cur[marker]);
                    cur.RemoveAt(marker);
                    prv.RemoveAt(cPrev);
                }
                return lRes.ToArray();
            }
            else return cur;
        }

        private void PrepareDir()
        {
            var fInfo = new FileInfo(tbSingleFile.Text);

            if (fInfo.Directory == null) return;
            var dirName = fInfo.Directory.FullName;
            var d = new DirectoryInfo(dirName); //Assuming Test is your Folder

            var files = d.GetFiles("c_*.png");
            foreach (var f in files) f.Delete();

            files = d.GetFiles("*.png");
            progressBar1.Value = 0;
            progressBar1.Maximum = files.Length;
            var strList = new List<string>();
            var dropCount = tbDropCount.Value;
            var prevList = new List<BallElement>();

            foreach (var file in files.OrderBy(x => x.Name))
            {
                var lst = PreparePicture(file.FullName, dropCount);
                IncValue();
                var ballElements = prevList.Count > 0 ? SortCoord(prevList, lst.ToList()).ToArray() : lst.ToArray();
                if (!ballElements.Any()) continue;
                //var cx = ballElements.Average(x => x.getX());
                //var cy = ballElements.Average(x => x.getY());
                var tstr = ballElements.Aggregate("", (current, t) => current + t.GetX() + ":" + t.GetY() + ":" + t.Radius() + ":");
                strList.Add(file.Name + ":" + tstr );
                prevList = ballElements.ToList();
                /*
                                ballElements = null;
                                lst = null;
                */
                
            }

            using (var sw = new StreamWriter(dirName + @"\stat.lst"))
            { 
                foreach (var t in strList.OrderBy(x => x))
                    sw.WriteLine(t.Replace(',', '.'));
            }
        }

        private void IncValue()
        {
            progressBar1.Value++;
        }

        private IEnumerable<BallElement> PreparePicture(string filename, int dropCount)
        {
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();

            var bmp = new Bitmap(filename);
            //var g = Graphics.FromImage(bmp);

            var img = new Image<Bgr, byte>(bmp);
            var uimage = new UMat();

            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);
            
            var minDist = tbMinimalDist.Value;
            var param1 = tbGradient.Value;
            var param2 = tbCurvature.Value;
            var minRadius = tbMinRadius.Value;
            var maxRadius = tbMaxRadius.Value;

            m_Circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1, minDist, param1,
                param2, minRadius, maxRadius);
            bmp.Dispose();
            img.Dispose();
            uimage.Dispose();
           // var circleImage = img.Copy(); //CopyBlank();

            //var f = new FileInfo(filename);
            //var fn = f.DirectoryName + @"\c_" + f.Name;
            //circleImage.Save(fn);

            return m_Circles.OrderByDescending(x => x.Radius).Take(dropCount).Select(circle => new BallElement(circle.Area, circle.Center.X, circle.Center.Y, circle.Radius)).ToList();
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
            using (OpenFileDialog opf = new OpenFileDialog())
            {
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    tbSingleFile.Text = opf.FileName;
                }
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
            if (!File.Exists(filename)) return;
            var bmp = new Bitmap(filename);
            m_ListCircles.Clear();

            var img = new Image<Bgr, byte>(bmp);
            var uimage = new UMat();

            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            var minDist = tbMinimalDist.Value;
            var param1 = tbGradient.Value;
            var param2 = tbCurvature.Value;
            var minRadius = tbMinRadius.Value;
            var maxRadius = tbMaxRadius.Value;

            m_Circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1, minDist, param1,
                param2, minRadius, maxRadius);

            var ballElements = m_Circles.OrderByDescending(x => x.Radius).Select(circle => new BallElement(circle.Area, circle.Center.X, circle.Center.Y, circle.Radius)).ToList();
            var tstr = ballElements.Aggregate("", (current, t) => current + t.GetX() + ":" + t.GetY() + ":" + t.Radius() + ":");
            tbCadr.Text = tstr;
            var circleImage = img.Copy(); //CopyBlank();
            pbOnePict.SizeMode = PictureBoxSizeMode.Zoom;
            pbOnePict.Image = circleImage.Bitmap;
            var pInfo = pbOnePict.GetType().GetProperty("ImageRectangle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pInfo != null)
            {
                var rectangle = (Rectangle)pInfo.GetValue(pbOnePict, null);
                m_YScale = rectangle.Height / (float)circleImage.Bitmap.Size.Height;
                m_XScale = rectangle.Width / (float)circleImage.Bitmap.Size.Width;
            }
            
            for (var i = 0; i < m_Circles.Length; i++)
            {
                var circle = m_Circles[i];
                circleImage.Draw(circle, new Bgr(Color.Brown), 2);

                circleImage.Draw(i.ToString(), new Point((int)circle.Center.X, (int)circle.Center.Y), FontFace.HersheyComplex, 2.0, new Bgr(Color.Brown));
                var fPt = new PointF(circle.Center.X * m_XScale, circle.Center.Y * m_YScale);
                m_ListCircles.Add(new PointListItem(i, fPt, circle.Radius));
            }
            m_Branched.Clear();
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
                var angle = Angle_point(m_Circles[point1].Center, m_Circles[point2].Center, m_Circles[point3].Center);
                angle = angle * 180 / Math.PI;
                if (angle > 180)
                {
                    angle = 360 - angle;
                }
                var fi = new FileInfo(tbSingleFile.Text);
                var lvItem = new ListViewItem(fi.Name);
                lvItem.SubItems.Add(point1.ToString());
                lvItem.SubItems.Add(m_Circles[point1].Radius.ToString("F3"));
                lvItem.SubItems.Add(point2.ToString());
                lvItem.SubItems.Add(m_Circles[point2].Radius.ToString("F3"));
                lvItem.SubItems.Add(point3.ToString());
                lvItem.SubItems.Add(m_Circles[point3].Radius.ToString("F3"));
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
                    var et = m_ListCircles.OrderBy(x => x.GetDistance(cp)).FirstOrDefault();
                    if (et == null) return;
                    switch (m_MouseClickNo)
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
                            m_MouseClickNo = 0;
                            break;
                    }
                    m_MouseClickNo++;
                    if (m_MouseClickNo > 2) m_MouseClickNo = 0;
                }
                else
                {
                    Button2_Click(sender, e);
                }
            } else if (cbBranched.Checked)
            {
                if (t.Button == MouseButtons.Left)
                {
                    var cp = new PointF(t.X - rectangle.X, t.Y);
                    var et = m_ListCircles.OrderBy(x => x.GetDistance(cp)).FirstOrDefault();
                    m_Branched.Add(et);
                }
                else
                {
                    Debug.WriteLine($"Drops count {m_Branched.Count}");
                    var brOverage = m_Branched.Average(x=>x.GetRadius());
                    var resOvg = 0.0;
                    for (var i = 1; i < m_Branched.Count; i++)
                    {
                        var res = i * (m_Branched.Count - i);
                        resOvg += res;
                        Debug.WriteLine($"{i}: {res}");
                    }

                    resOvg /= m_Branched.Count;
                    var rg = (brOverage * brOverage / m_Branched.Count()) * resOvg;
                    Debug.WriteLine($"Rg(1)^={rg}");

                    foreach (var itm in m_Branched)
                    {
                        Debug.WriteLine($"Point #{itm.Id()}, X:{itm.GetPoint().X}, Y:{itm.GetPoint().Y}, D: {itm.GetRadius()}");
                    }
                    var vList = new List<Vector>();
                    foreach (var itmI in m_Branched)
                    {
                        foreach (var itmJ in m_Branched)
                        {
                            if (itmI.Id() == itmJ.Id()) continue;
                            var vector = new Vector(itmI.GetPoint(), itmJ.GetPoint(), itmI.Id(), itmJ.Id());
                            if (!vList.Where(x => x.Identify(itmI.Id(), itmJ.Id())).Any())
                            {
                                vList.Add(vector);
                                Debug.WriteLine($"{itmI.Id()} - {itmJ.Id()}, X:{vector.X}, Y:{vector.Y}");
                            }
                        }
                    }
                    Debug.WriteLine($"Vectors count {vList.Count}");

                    var vSubtactSumm = 0.0;
                    foreach (var vectorI in vList)
                    {
                        foreach (var vectorJ in vList)
                        {
                            
                            //Debug.WriteLine($"Vector I X:{vectorI.X}, Y: {vectorI.Y}; Vector J X:{vectorJ.X}, Y: {vectorJ.Y}");
                            var sub = vectorI.Subtract(vectorJ);
                            //Debug.WriteLine($"Subtract vector X:{sub.X}, Y: {sub.Y}");
                            var mul = sub.ScalarMul(sub);
                           // Debug.WriteLine($"Square: {mul}");
                            vSubtactSumm += mul;
                            
                        }
                    }

                    vSubtactSumm /= (vList.Count * vList.Count);
                    Debug.WriteLine($"Rg(2)^={vSubtactSumm}");
                    var strList = new List<string>();
                    foreach (var cp in m_Branched)
                    {
                        strList.Add($"{cp.Id()}: {cp.GetPoint().X}: {cp.GetPoint().Y}: {cp.GetRadius()}");
                    }
                    StreamWriter sw;
                    using (sw = new StreamWriter("export-branched.csv"))
                    {
                        foreach (var item in strList)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    sw.Close();
                }
            } 
            else
            {
                if (t.Button == MouseButtons.Left)
                {
                    var cp = new PointF(t.X - rectangle.X, t.Y);
                    var et = m_ListCircles.OrderBy(x => x.GetDistance(cp)).FirstOrDefault();
                    if (et == null) return;
                    m_Polygon.Add(m_Circles[et.Id()].Center);
                    m_ClickedPoint.Add(et.Id());
                }
                else
                {
                    using (var g = pbOnePict.CreateGraphics())
                    {
                        for (var i = 1; i < m_Polygon.Count; i++)
                        {
                            g.DrawLine(Pens.Blue, new PointF(m_Polygon[i].X * m_XScale+ rectangle.X, m_Polygon[i].Y * m_YScale), new PointF(m_Polygon[i - 1].X * m_XScale+ rectangle.X, m_Polygon[i - 1].Y * m_YScale));
                        }
                        g.DrawLine(Pens.Blue, new PointF(m_Polygon[0].X * m_XScale+ rectangle.X, m_Polygon[0].Y * m_YScale), new PointF(m_Polygon[m_Polygon.Count - 1].X * m_XScale+ rectangle.X, m_Polygon[m_Polygon.Count - 1].Y * m_YScale));
                    }

                    m_CanArea = m_Polygon.ToArray();
                    var fi = new FileInfo(tbSingleFile.Text);
                    if (cbArea.Checked)
                    {
                        foreach (var cp in m_ClickedPoint)
                        {
                            var cir = m_Circles[cp];
                            var lvItem = new ListViewItem(cp.ToString());
                            lvItem.SubItems.Add(cir.Radius.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.X.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.Y.ToString("F6"));
                            
                            lvItem.SubItems.Add(fi.Name);
                            lvArea.Items.Add(lvItem);
                        }
                        
                        for (var i = 0; i < m_Circles.Length; i++)
                        {
                            var cir = m_Circles[i];
                            var ts = PointInArea.IsPointInside(m_Polygon.ToArray(), cir.Center);
                            if (!ts) continue;
                            if (m_ClickedPoint.Any(x => x == i)) continue;
                            var lvItem = new ListViewItem(i.ToString());
                            lvItem.SubItems.Add(cir.Radius.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.X.ToString("F6"));
                            lvItem.SubItems.Add(cir.Center.Y.ToString("F6"));
                            m_ClickedPoint.Add(i);
                            //var fi = new FileInfo(tbSingleFile.Text);
                            lvItem.SubItems.Add(fi.Name);
                            lvArea.Items.Add(lvItem);
                        }
                        PrepareLinks(m_ClickedPoint.Select(c => new PointListItem(c, m_Circles[c].Center, m_Circles[c].Radius)).ToList());
                        lbAvgRad.Text =
                            $@"Средний радиус: {
                                    m_ClickedPoint.Select(c => m_Circles[c].Radius).ToList().Average(x => x)
                                :F5}";
                        m_Polygon.Clear();
                        m_ClickedPoint.Clear();
                    } else if (cbChains.Checked)
                    {
                        if (m_ClickedPoint.Count <= 2) return;
                        var chItem = new ListViewItem(lvChains.Items.Count.ToString());
                        chItem.SubItems.Add(fi.Name);
                        var pList = m_ClickedPoint[0].ToString();
                        var avgR = 0.0d;
                        for (var i = 1; i < m_ClickedPoint.Count; i++)
                        {
                            var itm = m_ClickedPoint[i];
                            var pItm = m_ClickedPoint[i - 1];
                            var cti = new PointListItem(i, m_Circles[itm].Center, m_Circles[itm].Radius);
                            avgR += cti.GetDistance(m_Circles[pItm].Center);
                            pList += ", " + itm;
                        }
                        chItem.SubItems.Add(pList);
                        chItem.SubItems.Add(m_ClickedPoint.Count.ToString());
                        chItem.SubItems.Add((avgR / m_ClickedPoint.Count-1).ToString("F5"));
                        var lR = (new PointListItem(0, m_Circles[m_ClickedPoint[0]].Center, m_Circles[m_ClickedPoint[0]].Radius)).GetDistance(m_Circles[m_ClickedPoint[m_ClickedPoint.Count-1]].Center);
                        chItem.SubItems.Add(lR.ToString("F5"));
                        chItem.SubItems.Add((m_Polygon.Count-m_Polygon.Distinct().Count()).ToString());
                        lvChains.Items.Add(chItem);
                        m_Polygon.Clear();
                        m_ClickedPoint.Clear();
                    }
                }
            }
        }

        private void PrepareLinks(IEnumerable<PointListItem> lst)
        {
            var lList = new List<double>();
            var listPoints = lst as IList<PointListItem> ?? lst.ToList();
            var sum = listPoints.Sum(it => m_Circles[it.Id()].Radius);
            var lAvg = sum / listPoints.Count;
            var lCount = listPoints.Count;
            var rc = float.Parse(tbRadCount.Text.Replace("\"", string.Empty));
            var avg = (sum / listPoints.Count) * rc;
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
                    lList.Add(lnk.GetDistance(item.GetPoint())/lAvg);
                    lnkCount++;
                    lnkAvg += lnk.GetDistance(item.GetPoint());
                }
            }
            lnkAvg /= lnkCount;
            lbLinks.Items.Add($"Всего:{lnkCount}; Средн:{lnkAvg}");
            BuildHistogramm(lList, lCount, lAvg);
        }

        private string BuildHistogramm(IReadOnlyCollection<double> lst, int count, double avg)
        {
            var outStr = $"{count}:{avg}";
            // var interval = lst.Max() - lst.Min();
            // var dy = interval / 10 * 2;
            var cnt = lst.Count;
            chart2.Series[0].Points.Clear();
            
            for (var i = 1.0; i <= 10 * 2; i ++)
            { 
                var t = lst.Count(x => x <=i);   
                chart2.Series[0].Points.AddXY(i, (float)t/cnt);
                outStr += $":{i}:{(float)t / cnt}";
                if (!(i >= 1) || !(i <= 10)) continue;
                {
                    for (var di = 0.2; di<=0.8; di +=0.2)
                    {
                        var dt = lst.Count(x => x <= (i + di));
                        outStr += $":{i + di}:{(float)dt / cnt}";
                    }
                }
            }

            return outStr;
        }

        private void TrackBarGrade_Scroll(object sender, EventArgs e)
        {
            lbGrade.Text = @"Градация " + trackBarGrade.Value;
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
            using (OpenFileDialog fd = new OpenFileDialog { Filter = @"Файлы txt|*.txt" })
            {
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

        private void PbOnePict_Paint(object sender, PaintEventArgs e)
        {
            if (m_CanArea.Length < 3) return;
            var pInfo = pbOnePict.GetType().GetProperty("ImageRectangle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pInfo == null) return;
            var rectangle = (Rectangle)pInfo.GetValue(pbOnePict, null);

            for (var i = 1; i < m_CanArea.Length; i++)
            {
                e.Graphics.DrawLine(Pens.Blue, new PointF(m_CanArea[i].X * m_XScale + rectangle.X, m_CanArea[i].Y * m_YScale), new PointF(m_CanArea[i - 1].X * m_XScale + rectangle.X, m_CanArea[i - 1].Y * m_YScale));
            }
            e.Graphics.DrawLine(Pens.Blue, new PointF(m_CanArea[0].X * m_XScale + rectangle.X, m_CanArea[0].Y * m_YScale), new PointF(m_CanArea[m_CanArea.Length - 1].X * m_XScale + rectangle.X, m_CanArea[m_CanArea.Length - 1].Y * m_YScale));
        }

        private void BtnChainDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in lvChains.SelectedItems)
            {
                lvChains.Items.Remove(eachItem);
            }
        }

        private void BtnChainsSave_Click(object sender, EventArgs e)
        {
            var fi = new FileInfo(tbSingleFile.Text);
            var filePath = fi.DirectoryName + @"\chains_" + DateTime.Now.ToShortDateString() + "_.txt";
            StreamWriter sw;
            var headerStr = lvChains.Columns.Cast<ColumnHeader>().Aggregate("", (current, hd) => current + (hd.Text + ":"));
            using (sw = new StreamWriter(filePath))
            {
                sw.WriteLine(headerStr);
                foreach (ListViewItem item in lvChains.Items)
                {
                    sw.WriteLine(SubItemToString(item).Replace(',', '.'));
                }
            }
            sw.Close();
        }

        private void TmExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TmRename_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;
                var files = Directory.GetFiles(fbd.SelectedPath);

                foreach (var file in files)
                {
                    var onlyPath = Path.GetDirectoryName(file);
                    var onlyName = Path.GetFileNameWithoutExtension(file);
                    var extension = Path.GetExtension(file);
                    if (onlyName.Length == 1) onlyName = "00" + onlyName;
                    if (onlyName.Length == 2) onlyName = "0" + onlyName;
                    var fullName = Path.Combine(onlyPath, onlyName + extension);
                    File.Move(file, fullName);

                }
            }
        }
    }
}
