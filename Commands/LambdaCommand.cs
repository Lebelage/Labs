using Lab1.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Commands
{
    class LambdaCommand : Command
    {
        private readonly Action<object> _Execute;
        private readonly Func<object, bool> _CanExecute;

        public LambdaCommand(Action execute, Func<bool> canExecute) 
            : this(p => execute(), canExecute is null ? null : p => canExecute()) 
        {
        }

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _Execute = execute ?? throw new ArgumentNullException();
            _CanExecute = canExecute ?? throw new ArgumentNullException();
        }

        protected override bool CanExecute(object? p) => _CanExecute.Invoke(p);
        protected override void Execute(object? p) => _Execute(p);
    }
}
