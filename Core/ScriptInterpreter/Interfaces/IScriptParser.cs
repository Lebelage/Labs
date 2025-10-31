using Lab1.Core.ScriptInterpreter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.ScriptInterpreter.Interfaces
{
    public interface IScriptParser
    {
        (List<ScriptCommand> level0, List<ScriptCommand> level1, List<ScriptCommand> level2, int processedLines)
        Parse(string script);
    }
}
