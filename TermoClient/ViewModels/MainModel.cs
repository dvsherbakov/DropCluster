using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TermoClient.ViewModels
{
    internal class MainModel : ViewModel
    {
        private WindowState f_CurWindowState;
        public WindowState CurWindowState
        {
            get => f_CurWindowState;
            set => Set(ref f_CurWindowState, value);
        }

        private int f_WindowHeight;
        public int WindowHeight
        {
            get => f_WindowHeight;
            set => Set(ref f_WindowHeight, value);
        }

        private int f_WindowWidth;
        public int WindowWidth
        {
            get => f_WindowWidth;
            set => Set(ref f_WindowWidth, value);
        }

        private string f_FileName;
        public string FileName
        {
            get => f_FileName;
            set
            {
                if (Set(ref f_FileName, value))
                {
                    Debug.WriteLine($"test {f_FileName}");
                    f_ImageData.OpenFile(value);
                    ImgSource = f_ImageData.GetImage;
                }
            }
        }

        private BitmapImage f_ImgSource;
        public BitmapImage ImgSource
        {
            get => f_ImgSource;
            set => Set(ref f_ImgSource, value);
        }

        private double f_PanelX;
        public double PanelX
        {
            get => f_PanelX;
            set
            {
                Set(ref f_PanelX, value);
                Debug.WriteLine(value);
            }
        }

        private double f_PanelY;
        public double PanelY
        {
            get => f_PanelY;
            set => Set(ref f_PanelY, value);
        }

        private readonly PrepareCsv f_ImageData;

        #region Commands
        public ICommand QuitCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand MaximizedCommand { get; }
        public ICommand StandardSizeCommand { get; }
        #endregion

        public MainModel()
        {
            WindowHeight = 680;
            WindowWidth = 840;

            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            StandardSizeCommand = new LambdaCommand(OnStandardSizeCommand);

            f_ImageData = new PrepareCsv();
        }

        private void OnQuitApp(object p)
        {
            Application.Current.Shutdown();
        }

        private void OnMinimizedCommandExecute(object p)
        {
            CurWindowState = WindowState.Minimized;
        }

        private void OnMaximizedCommandExecute(object p)
        {
            CurWindowState = CurWindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnStandardSizeCommand(object sender)
        {
            WindowHeight = 680;
            WindowWidth = 840;
        }

        public void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Stop this");
        }


    }
}
