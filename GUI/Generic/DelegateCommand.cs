using System;
using System.Windows.Input;

namespace SalesOrganizer.Generic
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> m_execute;
        private readonly Func<object, bool> m_canExecute;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => m_canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => m_execute(parameter);

        public void CanExecuteHasChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
