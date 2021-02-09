using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

        private Point f_DragPoint;
        private Point f_DragMovePoint;
        private bool f_DragPressed;

        public MainWindow()
        {
            CsvData = new PrepareCsv();
            InitializeComponent();
            f_ToolMode = 1;
            f_DragPressed = false;
        }

        private void OpenCsvFile(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "csv documents (.csv)|*.csv",
                RestoreDirectory = true
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            CsvData.OpenFile(dlg.FileName);
            SrcImage.Source = CsvData.GetImage;
        }

        private void OnImageMouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(ImgContainer);
            f_PosX = (int)position.X;
            f_PosY = (int)position.Y;
            var temp = CsvData.GetTemp(f_PosX, f_PosY);
            Temperature.Text = $"Текущая: {temp:F1}";
            if (f_ToolMode == 2)
            {
                CanvasSubstrate.Children.Clear();
                var line = new Line
                {
                    Stroke = new SolidColorBrush(Colors.SandyBrown),
                    StrokeThickness = 5,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeDashCap = PenLineCap.Round,
                    X1 = f_PointOne.X,
                    Y1 = f_PointOne.Y,
                    X2 = f_PosX,
                    Y2 = f_PosY
                };
                CanvasSubstrate.Children.Add(line);
            }
        }

        private void OnImageMouseDown(object sender, MouseEventArgs e)
        {
            if (f_ToolMode == 1)
            {
                f_PointOne = new Point(f_PosX, f_PosY);
                f_ToolMode = 2;
            }
        }

        private void OnCanvasMouseDown(object sender, MouseEventArgs e)
        {
            if (f_ToolMode == 2)
            {
                f_PointTwo = new Point(f_PosX, f_PosY);
                f_ToolMode = 3;
                var tmp = CsvData.GetTempLine((int)f_PointOne.X, (int)f_PointOne.Y, (int)f_PointTwo.X, (int)f_PointTwo.Y);
            }

            if (f_ToolMode == 5)
            {
                f_DragPressed = !f_DragPressed;//true;
                var position = Mouse.GetPosition(ImgContainer);
                f_DragPoint = position;
            }
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (f_ToolMode == 5 && f_DragPressed)
            {
                CanvasSubstrate.Children.Clear();
                var position = Mouse.GetPosition(ImgContainer);
                var dx = f_DragPoint.X - position.X;
                var dy = f_DragPoint.Y - position.Y;
                f_PointOne.X -= dx;
                f_PointOne.Y -= dy;
                f_PointTwo.X -= dx;
                f_PointTwo.Y -= dy;
                var line = new Line
                {
                    Stroke = new SolidColorBrush(Colors.SandyBrown),
                    StrokeThickness = 5,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeDashCap = PenLineCap.Round,
                    X1 = f_PointOne.X,
                    Y1 = f_PointOne.Y,
                    X2 = f_PointTwo.X,
                    Y2 = f_PointTwo.Y
                };
                CanvasSubstrate.Children.Add(line);
                f_DragPoint = position;
            }
        }

        private void OnCanvasMouseUp(object sender, MouseEventArgs e)
        {
            if (f_ToolMode == 5)
            {
                f_DragPressed = false;
            }
        }

        private void SetPointTool(object sender, RoutedEventArgs e)
        {
            f_ToolMode = 1;
        }

        private void SetMoveTool(object sender, RoutedEventArgs e)
        {
            f_ToolMode = 5;
        }
    }
}
