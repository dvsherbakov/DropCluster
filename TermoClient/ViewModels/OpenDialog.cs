using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace TermoClient.ViewModels
{
    public class OpenDialog : Freezable
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
            typeof(string), typeof(OpenDialog), new PropertyMetadata(default(string)));
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter),
            typeof(string), typeof(OpenDialog), new PropertyMetadata(Properties.Resources.DefaultFilter));
        public string Filter { get => (string)GetValue(FilterProperty); set => SetValue(FilterProperty, value); }

        public static readonly DependencyProperty SelectedFileProperty = DependencyProperty.Register(nameof(SelectedFile),
            typeof(string), typeof(OpenDialog), new PropertyMetadata(default(string)));
        public string SelectedFile { get => (string)GetValue(SelectedFileProperty); set => SetValue(SelectedFileProperty, value); }

        public ICommand OpenCommand { get; }

        public OpenDialog()
        {
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted);
        }

        private void OnOpenCommandExecuted(object p)
        {
            var dlg = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter,
                RestoreDirectory = true,
                InitialDirectory = Environment.CurrentDirectory
            };
            if (dlg.ShowDialog() != true) return;
            SelectedFile = dlg.FileName;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new OpenDialog();
        }
    }
}
