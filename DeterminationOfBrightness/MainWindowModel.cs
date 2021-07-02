using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;


namespace DeterminationOfBrightness
{
    internal class MainWindowModel : ViewModel
    {
        public ICommand QuitCommand { get; }

        public string CaptionFile => Properties.Resources.CaptionFIle;
        public string CaptionOpen => Properties.Resources.CaptionOpen;
        public string CaptionExit => Properties.Resources.CaptionExit;
        public string CaptionParams => Properties.Resources.CaptionParams;
        public string CaptionRecognize => Properties.Resources.CaptionRecognize;
        public MainWindowModel()
        {
            QuitCommand = new LambdaCommand(p => Application.Current.Shutdown());
        }
    }
}
