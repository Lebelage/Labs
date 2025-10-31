using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.ScriptInterpreter.Utils
{
    public static class Logger
    {
        private static readonly bool DEBUG = true;
        private static StreamWriter _writer;

        static Logger()
        {
            if (DEBUG)
                _writer = new StreamWriter("debug_log.txt", append: true) { AutoFlush = true };
        }

        public static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var log = $"[{timestamp}] {message}";
            Console.WriteLine(log);
            _writer?.WriteLine(log);

            // Передаём в ViewModel через событие
            LogMessage?.Invoke(null, log);
        }

        public static event EventHandler<string> LogMessage;
    }
}
