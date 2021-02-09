using System.Windows;
using System.Windows.Input;

namespace TermoVisor
{
   
    public partial class MainWindow : Window
    {
        private PrepareCsv CsvData { get; }
        private int f_PosX;
        private int f_PosY;
        private int f_ToolMode;
        private Point f_PointOne;
        private Point f_PointTwo;

        public MainWindow()
        {
            CsvData = new PrepareCsv();
            InitializeComponent();
            f_ToolMode = 1;
        }

        private void OpenCsvFile(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv", Filter = "csv documents (.csv)|*.csv", RestoreDirectory = true
            };
            
            var result = dlg.ShowDialog();
            if (result != true) return;
            CsvData.OpenFile(dlg.FileName);
            SrcImage.Source = CsvData.GetImage;
        }

        private void OnImageMouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(ImgContainer);
            f_PosX = (int) position.X;
            f_PosY = (int) position.Y;
            var temp = CsvData.GetTemp(f_PosX, f_PosY);
            Temperature.Text = temp.ToString("F1");
        }

        private void OnImageMouseDown(object sender, MouseEventArgs e)
        {
            switch (f_ToolMode)
            {
                case 1:
                    f_PointOne = new Point(f_PosX, f_PosY);
                    f_ToolMode = 2;
                    break;
                case 2:
                    f_PointTwo = new Point(f_PosX, f_PosY);
                    f_ToolMode = 3;
                    break;
                default:
                    f_ToolMode = 1;
                    break;
            }
        }

        private void SetPointTool(object sender, RoutedEventArgs e)
        {
           f_ToolMode = 1;
        }
    }
}
