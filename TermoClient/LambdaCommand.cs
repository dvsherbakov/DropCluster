using System;
using System.Windows.Input;

namespace TermoClient
{
    internal class LambdaCommand : ICommand
    {
        private readonly Action<object> f_Execute;
        private readonly Func<object, bool> f_CanExecute;

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            f_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            f_CanExecute = canExecute;

        }

        public bool CanExecute(object parameter)
        {
            return f_CanExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            f_Execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
