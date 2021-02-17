using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
        public static RoutedCommand PrepareCurrent { get; set; }
        public static RoutedCommand PrepareFolder { get; set; }
        public static RoutedCommand DrawPath { get; set; }
    }
}
