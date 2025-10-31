using Lab1.Core.ScriptInterpreter.Interfaces;
using Lab1.Core.ScriptInterpreter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Lab1.Core.ScriptInterpreter.Services
{
    public class ScriptParser : IScriptParser
    {
        private readonly string[] _commands = { "let", "loop", "endloop", "save", "saveall", "pause", "experiment", "lines", "log" };
        private readonly string[] _variables = { "cuvette", "resistor" };

        public (List<ScriptCommand>, List<ScriptCommand>, List<ScriptCommand>, int) Parse(string script)
        {
            var lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(l => l.Trim())
                              .Where(l => !string.IsNullOrWhiteSpace(l))
                              .ToList();

            var cmds0 = new List<ScriptCommand>();
            var cmds1 = new List<ScriptCommand>();
            var cmds2 = new List<ScriptCommand>();
            bool loop1 = false, loop2 = false;
            int cuvette = -1, resistor = -1;
            int processedLines = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                var rawLine = lines[i];
                var line = RemoveComment(rawLine);
                if (string.IsNullOrWhiteSpace(line)) { processedLines++; continue; }

                var parts = Tokenize(line);
                if (parts.Count == 0) { processedLines++; continue; }

                var cmdStr = parts[0].ToLower();
                var cmdIndex = Array.IndexOf(_commands, cmdStr);
                if (cmdIndex == -1)
                    throw new Exception($"Строка {i + 1}: Неизвестная команда '{parts[0]}'\n> {rawLine}");

                var cmd = (CommandType)cmdIndex;

                try
                {
                    if (!loop1 && !loop2)
                        ParseLevel0(cmd, parts, cmds0, ref cuvette, ref resistor, ref loop1, i + 1);
                    else if (loop1 && !loop2)
                        ParseLevel1(cmd, parts, cmds1, ref loop2, ref loop1, i + 1);
                    else if (loop1 && loop2)
                        ParseLevel2(cmd, parts, cmds2, ref loop2, i + 1);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Строка {i + 1}: {ex.Message}\n> {rawLine}");
                }

                processedLines++;
            }

            if (loop1) throw new Exception("Не закрыт внешний цикл (endloop)");
            if (loop2) throw new Exception("Не закрыт вложенный цикл (endloop)");

            return (cmds0, cmds1, cmds2, processedLines);
        }

        private string RemoveComment(string line)
        {
            var idx = line.IndexOf("//");
            return idx >= 0 ? line.Substring(0, idx).Trim() : line.Trim();
        }

        private List<string> Tokenize(string line)
        {
            var matches = Regex.Matches(line, @"(=|\.\.)|[a-zA-Z_]\w*|\d+|\S+")
                              .Cast<Match>()
                              .Select(m => m.Value.Trim())
                              .Where(t => !string.IsNullOrEmpty(t))
                              .ToList();
            return matches;
        }

        private void ParseLevel0(CommandType cmd, List<string> parts, List<ScriptCommand> cmds0,
            ref int cuvette, ref int resistor, ref bool loop1, int lineNum)
        {
            switch (cmd)
            {
                case CommandType.Let:
                    var let = parts.Where(t => t != "=").ToList();
                    if (let.Count != 3) throw new Exception("let: ожидается 'let var = value'");
                    if (!int.TryParse(let[2], out int val)) throw new Exception("let: значение должно быть числом");
                    var varName = let[1].ToLower();
                    if (varName == _variables[0])
                    {
                        if (val < 0 || val > 3) throw new Exception("cuvette: 0..3");
                        cuvette = val;
                        cmds0.Add(new ScriptCommand(CommandType.Let, 0, val));
                    }
                    else if (varName == _variables[1])
                    {
                        if (val < 1 || val > 255) throw new Exception("resistor: 1..255");
                        resistor = val;
                        cmds0.Add(new ScriptCommand(CommandType.Let, 1, val));
                    }
                    else throw new Exception($"Неизвестная переменная: {let[1]}");
                    break;

                case CommandType.Loop:
                    var clean = parts.Where(p => p != "=" && p != "..").ToList();
                    if (clean.Count != 4) throw new Exception("loop: ожидается 'loop var = low .. high'");
                    if (!int.TryParse(clean[2], out int low) || !int.TryParse(clean[3], out int high))
                        throw new Exception("Границы loop должны быть числами");
                    var varIdx = Array.IndexOf(_variables, clean[1].ToLower());
                    if (varIdx == -1) throw new Exception($"Неизвестная переменная: {clean[1]}");
                    if (varIdx == 0 && cuvette >= 0) throw new Exception("loop по cuvette запрещён, если уже установлен");
                    loop1 = true;
                    cmds0.Add(new ScriptCommand(CommandType.Loop, varIdx, low, high));
                    break;

                case CommandType.EndLoop:
                    if (!loop1) throw new Exception("endloop без loop");
                    cmds0.Add(new ScriptCommand(CommandType.EndLoop));
                    loop1 = false;
                    break;

                case CommandType.Save:
                case CommandType.SaveAll:
                case CommandType.Pause:
                case CommandType.Lines:
                    cmds0.Add(new ScriptCommand(cmd));
                    break;

                case CommandType.Log:
                    var msg = string.Join(" ", parts.GetRange(1, parts.Count - 1));
                    cmds0.Add(new ScriptCommand(CommandType.Log, message: msg));
                    break;

                case CommandType.Experiment:
                    if (cuvette < 0 || resistor < 0) throw new Exception("experiment: cuvette и resistor должны быть установлены");
                    cmds0.Add(new ScriptCommand(CommandType.Experiment));
                    break;

                default:
                    throw new Exception($"Недопустимо на уровне 0: {cmd}");
            }
        }

        private void ParseLevel1(CommandType cmd, List<string> parts, List<ScriptCommand> cmds1,
            ref bool loop2, ref bool loop1, int lineNum)
        {
            switch (cmd)
            {
                case CommandType.Loop:
                    var clean = parts.Where(p => p != "=" && p != "..").ToList();
                    if (clean.Count != 4) throw new Exception("loop: ожидается 'loop resistor = low .. high'");
                    if (clean[1].ToLower() != _variables[1]) throw new Exception("Вложенный loop только по resistor");
                    if (!int.TryParse(clean[2], out int low) || !int.TryParse(clean[3], out int high))
                        throw new Exception("Границы loop должны быть числами");
                    loop2 = true;
                    cmds1.Add(new ScriptCommand(CommandType.Loop, 1, low, high));
                    break;

                case CommandType.EndLoop:
                    if (loop2)
                        cmds1.Add(new ScriptCommand(CommandType.EndLoop));
                    else
                    {
                        cmds1.Add(new ScriptCommand(CommandType.EndLoop));
                        loop1 = false;
                    }
                    break;

                case CommandType.Experiment:
                case CommandType.Pause:
                case CommandType.Lines:
                    cmds1.Add(new ScriptCommand(cmd));
                    break;

                case CommandType.Log:
                    var msg = string.Join(" ", parts.GetRange(1, parts.Count - 1));
                    cmds1.Add(new ScriptCommand(CommandType.Log, message: msg));
                    break;

                case CommandType.Save:
                case CommandType.SaveAll:
                    throw new Exception($"Команда {cmd} разрешена только на уровне 0");

                default:
                    throw new Exception($"Недопустимо на уровне 1: {cmd}");
            }
        }

        private void ParseLevel2(CommandType cmd, List<string> parts, List<ScriptCommand> cmds2, ref bool loop2, int lineNum)
        {
            switch (cmd)
            {
                case CommandType.EndLoop:
                    cmds2.Add(new ScriptCommand(CommandType.EndLoop));
                    loop2 = false;
                    break;

                case CommandType.Experiment:
                case CommandType.Pause:
                    cmds2.Add(new ScriptCommand(cmd));
                    break;

                case CommandType.Log:
                    var msg = string.Join(" ", parts.GetRange(1, parts.Count - 1));
                    cmds2.Add(new ScriptCommand(CommandType.Log, message: msg));
                    break;

                default:
                    throw new Exception($"Недопустимо на уровне 2: {cmd}");
            }
        }
    }
}

