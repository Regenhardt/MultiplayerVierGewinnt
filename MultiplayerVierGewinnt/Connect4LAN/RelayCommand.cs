using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Connect4LAN
{
    public class RelayCommand : ICommand
    {
        #region [ Fields ]

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        #endregion // Fields

        #region [ Constructors]
        /// <summary>
        /// Create new relay command, based on injection of action object.
        /// </summary>
        /// <param name="execute">Action object</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Create new relay command, based on injection of action (command contents) and function predicate for the can execute methode.
        /// </summary>
        /// <param name="execute">Action object.</param>
        /// <param name="canExecute">Function predicate for the can execute.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region [ ICommand Members ]
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Evemt handler for the can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Command execution with parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion // ICommand Members
    }
}