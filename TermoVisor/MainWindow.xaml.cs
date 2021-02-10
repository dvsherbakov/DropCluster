using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private bool f_DragPressed;

        private float[] f_CurrentLine;

        private readonly float f_PointPerPixel = 33.5f;

        public MainWindow()
        {
            CsvData = new PrepareCsv();
            InitializeComponent();
            f_ToolMode = 0;
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
            f_ToolMode = 1;
            Maximum.Content = $"Максимальная: {CsvData.MaxTemp:F1}";
            Minimum.Content = $"Минимальная: {CsvData.MinTemp:F1}";
        }


        private void AddLine(Point p1, Point p2)
        {
            var line = new Line
            {
                Stroke = new SolidColorBrush(Colors.SandyBrown),
                StrokeThickness = 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y
            };
            CanvasSubstrate.Children.Add(line);
        }

        private void OnCanvasMouseUp(object sender, MouseEventArgs e)
        {

        }

        private void OnTopCanvasMouseUp(object sender, MouseEventArgs e)
        {
            if (f_ToolMode == 5)
            {
                f_DragPressed = false;
            }
        }

        private void OnTopCanvasMouseDown(object sender, MouseEventArgs e)
        {

            if (f_ToolMode == 2)
            {
                f_PointTwo = new Point(f_PosX, f_PosY);
                f_ToolMode = 5;
                f_CurrentLine = CsvData.GetTempLine((int)f_PointOne.X, (int)f_PointOne.Y, (int)f_PointTwo.X, (int)f_PointTwo.Y);
                DrawTempChart(f_CurrentLine);
            }

            if (f_ToolMode == 5)
            {
                f_DragPressed = !f_DragPressed;//true;
                var position = Mouse.GetPosition(ImgContainer);
                f_DragPoint = position;
            }
            if (f_ToolMode == 1)
            {
                f_PointOne = new Point(f_PosX, f_PosY);
                f_ToolMode = 2;
            }
        }


        private void OnTopCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(ImgContainer);
            f_PosX = (int)position.X;
            f_PosY = (int)position.Y;
            var temp = CsvData.GetTemp(f_PosX, f_PosY);
            Temperature.Text = $"Текущая: {temp:F1}";

            if (f_ToolMode == 5 && f_DragPressed)
            {
                CanvasSubstrate.Children.Clear();
                var dx = f_DragPoint.X - position.X;
                var dy = f_DragPoint.Y - position.Y;
                if (!(GetDistance(position, f_PointTwo) < 15))
                {
                    f_PointOne.X -= dx;
                    f_PointOne.Y -= dy;
                }
                if (!(GetDistance(position, f_PointOne) < 15))
                {
                    f_PointTwo.X -= dx;
                    f_PointTwo.Y -= dy;
                }

                AddLine(f_PointOne, f_PointTwo);

                f_CurrentLine = CsvData.GetTempLine((int)f_PointOne.X, (int)f_PointOne.Y, (int)f_PointTwo.X, (int)f_PointTwo.Y);
                DrawTempChart(f_CurrentLine);

                f_DragPoint = position;
            }

            if (f_ToolMode == 2)
            {
                CanvasSubstrate.Children.Clear();
                AddLine(f_PointOne, new Point(f_PosX, f_PosY));
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

        private void DrawTempChart(float[] data)
        {
            if (data.Length <= 0) return;
            ChartCanvas.Children.Clear();

            var (max, min) = MaxValue(data);
            var k = 80 / max;

            for (var i = 0; i < data.Length; i++)
            {
                var cl = CsvData.GetColorFromValue(data[i]);
                System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(cl.A, cl.R, cl.G, cl.B);
                var line = new Line
                {
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = 2,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeDashCap = PenLineCap.Round,
                    X1 = i + 5,
                    Y1 = 80,
                    X2 = i + 5,
                    Y2 = 80 - k * data[i]
                };
                ChartCanvas.Children.Add(line);
            }

            ChartMax.Text = $"Максимальная: {max:F1}";
            ChartMin.Text = $"Минимальная: {min:F1}";
            ChartLen.Text = $"Количество: {data.Length}";
        }

        private (float max, float min) MaxValue(float[] data)
        {
            if (data.Length <= 0) return (0f, 0f);
            var maxValue = data[0];
            var minValue = data[0];
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] > maxValue) maxValue = data[i];
                if (data[i] < minValue) minValue = data[i];
            }
            return (max: maxValue, min: minValue);
        }

        private void SetClipBoard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(PrepareClipBoard());
        }

        private string PrepareClipBoard()
        {
            return string.Join("\r\n", f_CurrentLine);
        }

        private void SetMax(object sender, RoutedEventArgs e)
        {
            Polygon p = new Polygon
            {
                Stroke = Brushes.Green,
                Fill = Brushes.Green,
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Points = new PointCollection()
                {
                    new Point(CsvData.MaxPoint.X, CsvData.MaxPoint.Y),
                    new Point(CsvData.MaxPoint.X - 2, CsvData.MaxPoint.Y + 5),
                    new Point(CsvData.MaxPoint.X + 2, CsvData.MaxPoint.Y + 5)
                }
            };
            MarkersCanvas.Children.Add(p);
        }

        private void SetMin(object sender, RoutedEventArgs e)
        {
            Polygon p = new Polygon
            {
                Stroke = Brushes.Green,
                Fill = Brushes.Green,
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Points = new PointCollection()
                {
                    new Point(CsvData.MinPoint.X, CsvData.MinPoint.Y),
                    new Point(CsvData.MinPoint.X - 2, CsvData.MinPoint.Y + 5),
                    new Point(CsvData.MinPoint.X + 2, CsvData.MinPoint.Y + 5)
                }
            };
            MarkersCanvas.Children.Add(p);
        }

        private static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
