using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Visual_Assembler.ViewModels.Commands
{

    public class RelayCommand<T> : ICommand {

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private Action<T> methodToExecute;
        private Func<T, bool> canExecuteEvaluator;

        public RelayCommand(Action<T> methodToExecute, Func<T, bool> canExecuteEvaluator) {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }
        public RelayCommand(Action<T> methodToExecute)
            : this(methodToExecute, null) {
        }
        public bool CanExecute(object parameter) {
            if (this.canExecuteEvaluator == null) {
                return true;
            } else {
                bool result = this.canExecuteEvaluator.Invoke((T)parameter);
                return result;
            }
        }
        public void Execute(object parameter) {
            this.methodToExecute.Invoke((T)parameter);
        }

    }

    public class RelayCommand : ICommand {

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private Action methodToExecute;
        private Func<bool> canExecuteEvaluator;

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator) {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }
        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null) {
        }
        public bool CanExecute(object parameter) {
            if (this.canExecuteEvaluator == null) {
                return true;
            } else {
                bool result = this.canExecuteEvaluator.Invoke();
                return result;
            }
        }
        public void Execute(object parameter) {
            this.methodToExecute.Invoke();
        }

    }

}
