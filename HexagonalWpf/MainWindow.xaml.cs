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
using Path = System.IO.Path;

namespace HexagonalWpf
{

    public partial class MainWindow : Window
    {

        private string _currentFileName;
        private double _fOrigWidth;
        private double _fOrigHeight;
        private double _fRatio;
        private RawCluster _rawCluster;
        //private RelativePosition _relativePosition;
        private HexagonPack _hexPack;
       
        private readonly ClusterPack _clusterPack;

        public MainWindow()
        {
            InitializeComponent();
            Closing += App_Exit;
            tbBinarizationThreshold.Text = Properties.Settings.Default.BinarizationThreshold.ToString();
            tbGaussianParam.Text = Properties.Settings.Default.GaussianParam.ToString();
            tbMaxAspectRatio.Text = Properties.Settings.Default.MaxAspectRatio.ToString();
            tbMinPerimetherLen.Text = Properties.Settings.Default.MinPerimetherLen.ToString();
            tbCameraZoom.Text = Properties.Settings.Default.CameraZoom.ToString();
            _clusterPack = new ClusterPack("");
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
            
            _currentFileName = dlg.FileName;
            var oi = new BitmapImage(new Uri(dlg.FileName));
            _fOrigWidth = oi.PixelWidth;
            _fOrigHeight = oi.PixelHeight;
            OriginalImage.Source = oi;
            RangeCanvas();
        }

        private static void App_Exit(object sender, CancelEventArgs cancelEventArg)
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
            _ = EventCanvas.Children.Add(rect);
            _ = ObjectCanvas.Children.Add(objRect);
            _fRatio = ((EventCanvas.Width / _fOrigWidth) + (EventCanvas.Height / _fOrigHeight)) / 2;
        }

        private void ViewContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ObjectCanvas.Children.Clear();
            RangeCanvas();
            if (_rawCluster != null && _rawCluster.GetCluser.Count > 0)
            {
                Dispatcher.Invoke(() => DrawUiObject(_rawCluster.GetElements));
            }
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _ = Task.Run(PrepareFileAsync);
        }
        private void CommandBinding_PrepareFolder(object sender, ExecutedRoutedEventArgs e)
        {

            // _hexPack = new HexagonPack(_currentFileName, _relativePosition);
            //  _ = Task.Run(() => _hexPack.PrepareFolderAsync());

            _ = PrepareFolderAsync(Path.GetDirectoryName(_currentFileName));
        }

        private void CommandBinding_PrepareFolderArea(object sender, ExecutedRoutedEventArgs e)
        {
            PrepareFolderAreaAsync(Path.GetDirectoryName(_currentFileName));
        }

        private void CommandBinding_DrawPath(object sender, ExecutedRoutedEventArgs e)
        {
            
            var newPolyLine = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeThickness = 2
            };
            foreach (var h in _hexPack.HexList)
            {
                newPolyLine.Points.Add(new Point(h.Center.Element.Center.X * _fRatio, h.Center.Element.Center.Y * _fRatio));
            }
            _ = ObjectCanvas.Children.Add(newPolyLine);

            _ = Task.Run(() => _hexPack.SaveExcelFile());

        }


        private async Task PrepareFileAsync()
        {
            _rawCluster = new RawCluster(
                _currentFileName,
                Properties.Settings.Default.GaussianParam,
                Properties.Settings.Default.BinarizationThreshold,
                Properties.Settings.Default.MaxAspectRatio,
                Properties.Settings.Default.MinPerimetherLen);
            await _rawCluster.MakeCluster();
            if (_clusterPack.Id == "") _clusterPack.Id = Path.GetDirectoryName(_currentFileName);
            _clusterPack.Add(_rawCluster.GetCluser);
            Dispatcher.Invoke(() => DrawUiObject(_rawCluster.GetElements));
        }

        private async Task PrepareFolderAsync(string folderName)
        {

            // var folderName = Path.GetDirectoryName(folderName);
            var fileExt = Path.GetExtension(_currentFileName);
            _clusterPack.Clear();
            var fl = new FileList(folderName, fileExt);

            //var files = Directory.GetFiles(folderName ?? string.Empty, $"*{fileExt}");
            foreach (var file in fl.GetList)
            {
                await Task.Run(() =>
                {
                    _rawCluster = new RawCluster(
                       file,
                        Properties.Settings.Default.GaussianParam,
                        Properties.Settings.Default.BinarizationThreshold,
                        Properties.Settings.Default.MaxAspectRatio,
                        Properties.Settings.Default.MinPerimetherLen);
                    _ = _rawCluster.MakeCluster();
                    if (_clusterPack.Id == "") _clusterPack.Id = Path.GetDirectoryName(file);
                    _clusterPack.Add(_rawCluster.GetCluser);
                    Dispatcher.Invoke(() => Counter.Text = _clusterPack.Count.ToString());
                });
            }
        }

        private void PrepareFolderAreaAsync(string folderName)
        {
            var fileExt = Path.GetExtension(_currentFileName);
            var fl = new FileList(folderName, fileExt);
        }

        private void DrawUiObject(IEnumerable<ClusterElement> elements)
        {
            ObjectCanvas.Children.Clear();
            try
            {
                foreach (var element in elements)
                {
                    DrawMarker(element, Colors.Red);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DrawMarker(ClusterElement element, Color color)
        {
            var wh = ((element.Element.Size.Width + element.Element.Size.Height) / 2) * _fRatio;
            var uiElem = new Ellipse
            {
                Width = wh,
                Height = wh,
                StrokeThickness = 4,
                Stroke = new SolidColorBrush
                {
                    Color = color
                }
            };

            Canvas.SetLeft(uiElem, (element.Element.Center.X - element.Element.Size.Width / 2) * _fRatio);
            Canvas.SetTop(uiElem, (element.Element.Center.Y - element.Element.Size.Height / 2) * _fRatio);

            ObjectCanvas.Children.Add(uiElem);

            var textBlock = new TextBlock { Text = element.Id.ToString(), Foreground = new SolidColorBrush(Colors.Blue), FontSize = 30 };
            Canvas.SetLeft(textBlock, element.Element.Center.X * _fRatio);
            Canvas.SetTop(textBlock, element.Element.Center.Y * _fRatio);
            ObjectCanvas.Children.Add(textBlock);
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
            if (int.TryParse(tbMinPerimetherLen.Text, out var res))
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
                    new System.Drawing.PointF((float)(position.X / _fRatio), (float)(position.Y / _fRatio)),
                    new System.Drawing.SizeF(),
                    0
                );
            // if (_rawCluster == null) return;
            // var markedElement = _rawCluster.GetNearer(pt);
            //DrawMarker(markedElement, Colors.Orange);
            // _rawCluster.CreateHexagon(markedElement);
            //DrawHexagon(_rawCluster.Hexagon);
            // _rawCluster.Hexagon.AverageLink();

        }

        private void CommandBinding_SearchLinks(object sender, ExecutedRoutedEventArgs e)
        {
            _clusterPack.Renumbering();
            //throw new NotImplementedException();
            //Stopwatch this
        }

        private void CommandBinding_OnExecutedNextSrc(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.Invoke(() => DrawUiObject(_clusterPack.NextById(_clusterPack.CurrentId).GetList));
            Dispatcher.Invoke(() => Counter.Text = _clusterPack.GetCurrentNumberId());
        }

        private void CommandBinding_OnExecutedPrevSrc(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.Invoke(() => DrawUiObject(_clusterPack.PrevById(_clusterPack.CurrentId).GetList));
            Dispatcher.Invoke(() => Counter.Text = _clusterPack.GetCurrentNumberId());
        }

        private void CommandBinding_OnExecutedSaveResult(object sender, ExecutedRoutedEventArgs e)
        {
            _clusterPack.SaveResult();
        }

        private void FirstSrc(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.Invoke(() => DrawUiObject(_clusterPack.First().GetList));
            Dispatcher.Invoke(() => Counter.Text = _clusterPack.GetCurrentNumberId());
        }

        private void CommandBinding_OnExecutedSaveAvg(object sender, ExecutedRoutedEventArgs e)
        {
            _clusterPack.SaveAvgDiameters();
        }

        private void CommandBinding_OnExecutedSaveShearInfo(object sender, ExecutedRoutedEventArgs e)
        {
            _ = _clusterPack.SaveShearInfo();
        }

        private void CommandBindingOnExecutedSaveBrightestSpot(object sender, ExecutedRoutedEventArgs e)
        {
            _clusterPack.SaveBrightestSpot();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
