using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cassiopeia.Base
{
    public class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            CodeToExecute();
        }
        public Action CodeToExecute {get;set;}

        public CommandBase(Action action)
        {
            CodeToExecute = action;
        }
    }
}
