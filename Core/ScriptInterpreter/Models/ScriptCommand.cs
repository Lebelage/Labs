
namespace Lab1.Core.ScriptInterpreter.Models
{
    public class ScriptCommand
    {
        public CommandType Cmd { get; }
        public int Op1 { get; }
        public int Op2 { get; }
        public int Op3 { get; }
        public string Message { get; }

        public ScriptCommand(CommandType cmd, int op1 = -1, int op2 = -1, int op3 = -1, string message = null)
        {
            Cmd = cmd;
            Op1 = op1;
            Op2 = op2;
            Op3 = op3;
            Message = message;
        }
    }

    public enum CommandType
    {
        Let, Loop, EndLoop, Save, SaveAll, Pause, Experiment, Lines, Log
    }
}
