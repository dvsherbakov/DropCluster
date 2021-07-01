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

        public MainWindowModel()
        {
            QuitCommand = new LambdaCommand(p => Application.Current.Shutdown());
        }
    }
}
