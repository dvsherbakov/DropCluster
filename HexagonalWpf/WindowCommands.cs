using System.Windows.Input;

namespace HexagonalWpf
{
    public class WindowCommands
    {
        static WindowCommands()
        {
            PrepareCurrent = new RoutedCommand("PrepareCurrent", typeof(MainWindow));
            PrepareFolder = new RoutedCommand("PrepareFolder", typeof(MainWindow));
            DrawPath = new RoutedCommand("DrawPath", typeof(MainWindow));
            SearchLinks = new RoutedCommand("SearchLinks", typeof(MainWindow));
            NextSrc = new RoutedCommand("NextSrc", typeof(MainWindow));
            PrevSrc = new RoutedCommand("PrevSrc", typeof(MainWindow));
        }
        public static RoutedCommand PrepareCurrent { get; set; }
        public static RoutedCommand PrepareFolder { get; set; }
        public static RoutedCommand DrawPath { get; set; }
        public static  RoutedCommand SearchLinks { get; set; }
        public static  RoutedCommand NextSrc { get; set; }
        public static  RoutedCommand PrevSrc { get; set; }
    }
}
