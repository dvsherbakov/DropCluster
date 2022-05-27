using System.Windows.Input;

namespace HexagonalWpf
{
    public class WindowCommands
    {
        static WindowCommands()
        {
            PrepareCurrent = new RoutedCommand("PrepareCurrent", typeof(MainWindow));
            PrepareFolder = new RoutedCommand("PrepareFolder", typeof(MainWindow));
            PrepareFolderArea = new RoutedCommand("PrepareFolderArea", typeof(MainWindow));
            DrawPath = new RoutedCommand("DrawPath", typeof(MainWindow));
            SearchLinks = new RoutedCommand("SearchLinks", typeof(MainWindow));
            NextSrc = new RoutedCommand("NextSrc", typeof(MainWindow));
            PrevSrc = new RoutedCommand("PrevSrc", typeof(MainWindow));
            FirstSrc = new RoutedCommand("FirstSrc", typeof(MainWindow));
            SaveResult = new RoutedCommand("SaveResult", typeof(MainWindow));
            SaveAvg= new RoutedCommand("SaveAvg", typeof(MainWindow));
            SaveShearInfo = new RoutedCommand("SaveShearInfo", typeof(MainWindow));
            SaveBrightestSpot = new RoutedCommand("SaveBrightestSpot", typeof(MainWindow));
        }
        public static RoutedCommand PrepareCurrent { get; set; }
        public static RoutedCommand PrepareFolder { get; set; }
        public static  RoutedCommand PrepareFolderArea { get; set; }
        public static RoutedCommand DrawPath { get; set; }
        public static  RoutedCommand SearchLinks { get; set; }
        public static  RoutedCommand NextSrc { get; set; }
        public static  RoutedCommand PrevSrc { get; set; }
        public static RoutedCommand FirstSrc { get; set; }
        public static  RoutedCommand SaveResult { get; set; }
        public static  RoutedCommand SaveAvg { get; set; }
        public static RoutedCommand SaveShearInfo { get; set; }
        public static RoutedCommand SaveBrightestSpot { get; set; }
    }
}
