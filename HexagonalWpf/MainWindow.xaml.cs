using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexagonalWpf
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowBinding_OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff"+
                "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|",
                RestoreDirectory = true
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            Debug.WriteLine(dlg.FileName);
            OriginalImage.Source = new BitmapImage(new Uri(dlg.FileName));
            RangeCanvas();
        }

        private void RangeCanvas()
        {
            ViewContainer.UpdateLayout();
            EventCanvas.Children.Clear();
            EventCanvas.Width = ViewContainer.ActualWidth;
            EventCanvas.Height = ViewContainer.ActualHeight;
            var rect = new Rectangle
            {
                Width = ViewContainer.ActualWidth,
                Height = ViewContainer.ActualHeight,
                Stroke = new SolidColorBrush(Colors.SandyBrown),
                Fill = new SolidColorBrush(Colors.Transparent)
            };
            EventCanvas.Children.Add(rect);
        }

        private void ViewContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RangeCanvas();
        }
    }
}
