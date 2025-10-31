using Lab1.Core.ScriptInterpreter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.ScriptInterpreter.Interfaces
{
    public interface ICommandExecutor
    {
        void Execute(List<ScriptCommand> cmds0, List<ScriptCommand> cmds1, List<ScriptCommand> cmds2, int processedLines);
    }
}
