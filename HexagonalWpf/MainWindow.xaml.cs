using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HexagonalWpf
{

    public partial class MainWindow : Window
    {

        private string f_CurrentFileName;
        private double f_OrigWidth;
        private double f_OrigHeight;
        private double f_Ratio;
        private RawCluster f_RawCluster;
        private RelativePosition f_RPosition;
        private HexagonPack f_HexPack;

        public MainWindow()
        {
            InitializeComponent();
            Closing += App_Exit;
            tbBinarizationThreshold.Text = Properties.Settings.Default.BinarizationThreshold.ToString();
            tbGaussianParam.Text = Properties.Settings.Default.GaussianParam.ToString();
            tbMaxAspectRatio.Text = Properties.Settings.Default.MaxAspectRatio.ToString();
            tbMinPerimetherLen.Text = Properties.Settings.Default.MinPerimetherLen.ToString();
            tbCameraZoom.Text = Properties.Settings.Default.CameraZoom.ToString();
        }



        private void WindowBinding_OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff" +
                "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|",
                RestoreDirectory = true
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            Debug.WriteLine(dlg.FileName);
            f_CurrentFileName = dlg.FileName;
            var oi = new BitmapImage(new Uri(dlg.FileName));
            f_OrigWidth = oi.PixelWidth;
            f_OrigHeight = oi.PixelHeight;
            OriginalImage.Source = oi;
            RangeCanvas();
        }

        private void App_Exit(object sender, CancelEventArgs cancelEventArg)
        {
            Properties.Settings.Default.Save();
        }

        private void RangeCanvas()
        {
            ViewContainer.UpdateLayout();
            OriginalImage.UpdateLayout();
            EventCanvas.Children.Clear();
            EventCanvas.Width = ViewContainer.ActualWidth;
            EventCanvas.Height = ViewContainer.ActualHeight;
            ObjectCanvas.Width = ViewContainer.ActualWidth;
            ObjectCanvas.Height = ViewContainer.ActualHeight;

            var rect = new Rectangle
            {
                Width = ViewContainer.ActualWidth,
                Height = ViewContainer.ActualHeight,
                Stroke = new SolidColorBrush(Colors.Transparent),
                Fill = new SolidColorBrush(Colors.Transparent)
            };
            var objRect = new Rectangle
            {
                Width = ViewContainer.ActualWidth,
                Height = ViewContainer.ActualHeight,
                Stroke = new SolidColorBrush(Colors.Transparent),
                Fill = new SolidColorBrush(Colors.Transparent)
            };
            EventCanvas.Children.Add(rect);
            ObjectCanvas.Children.Add(objRect);
            f_Ratio = ((EventCanvas.Width / f_OrigWidth) + (EventCanvas.Height / f_OrigHeight)) / 2;
        }

        private void ViewContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RangeCanvas();
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Task.Run(() => PrepareFileAsync());
        }
        private void CommandBinding_PrepareFolder(object sender, ExecutedRoutedEventArgs e)
        {

            f_HexPack = new HexagonPack(f_CurrentFileName, f_RPosition);
            _ = Task.Run(() => f_HexPack.PrepareFolderAsync());
        }

        private void CommandBinding_DrawPath(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.WriteLine("DrawPath");
            var NewPolyline = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeThickness = 2
            };
            foreach (var h in f_HexPack.HexList)
            {
                NewPolyline.Points.Add(new Point(h.Center.Element.Center.X * f_Ratio, h.Center.Element.Center.Y * f_Ratio));
            }
            ObjectCanvas.Children.Add(NewPolyline);

            Task.Run(() => f_HexPack.SaveExcelFile());

        }


        private async Task PrepareFileAsync()
        {
            f_RawCluster = new RawCluster(
                f_CurrentFileName,
                Properties.Settings.Default.GaussianParam,
                Properties.Settings.Default.BinarizationThreshold,
                Properties.Settings.Default.MaxAspectRatio,
                Properties.Settings.Default.MinPerimetherLen);
            await f_RawCluster.MakeCluster();
            Dispatcher.Invoke(() => DrawUIObject(f_RawCluster.GetElements));
        }

        private void DrawUIObject(IEnumerable<ClusterElement> elements)
        {
            ObjectCanvas.Children.Clear();
            try
            {
                foreach (var element in elements)
                {
                    DrawMarker(element, Colors.Yellow);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DrawMarker(ClusterElement element, Color color)
        {
            var wh = ((element.Element.Size.Width + element.Element.Size.Height) / 2) * f_Ratio;
            var uiElem = new Ellipse
            {
                Width = wh,
                Height = wh,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush
                {
                    Color = color
                }
            };

            Canvas.SetLeft(uiElem, (element.Element.Center.X - element.Element.Size.Width / 2) * f_Ratio);
            Canvas.SetTop(uiElem, (element.Element.Center.Y - element.Element.Size.Height / 2) * f_Ratio);

            ObjectCanvas.Children.Add(uiElem);
        }

        private void DrawHexagon(Hexagon h)
        {
            DrawMarker(h.Center, Colors.LimeGreen);
            foreach (var el in h.HList)
            {
                DrawMarker(el, Colors.Red);
            }
        }

        private void BinarizationThreshold_LostFocus(object sender, RoutedEventArgs e)
        {
            int.TryParse(tbBinarizationThreshold.Text, out int res);
            Properties.Settings.Default.BinarizationThreshold = res;
        }

        private void GaussianParam_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbGaussianParam.Text, out int res))
                Properties.Settings.Default.GaussianParam = res;
        }

        private void MaxAspectRatio_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbMaxAspectRatio.Text, out int res))
                Properties.Settings.Default.MaxAspectRatio = res;
        }

        private void MinPerimetherLen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbMinPerimetherLen.Text, out int res))
                Properties.Settings.Default.MinPerimetherLen = res;
        }

        private void CameraZoom_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbCameraZoom.Text, out int res))
                Properties.Settings.Default.CameraZoom = res;
        }

        private void ViewContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(ViewContainer);
            var pt = new Emgu.CV.Structure.RotatedRect(
                    new System.Drawing.PointF((float)(position.X / f_Ratio), (float)(position.Y / f_Ratio)),
                    new System.Drawing.SizeF(),
                    0
                );
            if (f_RawCluster != null)
            {
                var markedElement = f_RawCluster.GetNearer(pt);
                //DrawMarker(markedElement, Colors.Orange);
                f_RawCluster.CreateHexagon(markedElement);
                DrawHexagon(f_RawCluster.Hexagon);
                f_RawCluster.Hexagon.AverageLink();
                f_RPosition = f_RawCluster.GetRelativePosition(pt.Center);
            }

        }
    }
}
