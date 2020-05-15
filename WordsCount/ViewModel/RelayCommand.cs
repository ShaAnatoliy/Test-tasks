using System;
using System.Windows.Input;

namespace WordsCount
{
	internal class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Func<object, bool> canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public RelayCommand(Action<object> action, Func<object, bool> canExecute = null)
		{
			execute = action;
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return this.canExecute == null || this.canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			this.execute(parameter);
		}
	}
}