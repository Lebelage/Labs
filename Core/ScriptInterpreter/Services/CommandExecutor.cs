using Lab1.Core.ScriptInterpreter.Interfaces;
using Lab1.Core.ScriptInterpreter.Models;
using Lab1.Core.ScriptInterpreter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Lab1.Core.ScriptInterpreter.Services
{
    public class CommandExecutor : ICommandExecutor
    {
        private const bool DEBUG = true;
        private StreamWriter _fout;
        private int _cuvette = -1;
        private int _resistor = -1;

        public void Execute(List<ScriptCommand> cmds0, List<ScriptCommand> cmds1, List<ScriptCommand> cmds2, int processedLines)
        {
            //if (DEBUG) _fout = new StreamWriter("debug_log.txt", false);

            Log("вошли в Emulate");
            Log("Сценарий интерпретирован успешно");

            foreach (var cmd in cmds0)
            {
                switch (cmd.Cmd)
                {
                    case CommandType.Let:
                        if (cmd.Op1 == 0) { _cuvette = cmd.Op2; Log($"номер кюветы -> {_cuvette}"); }
                        else { _resistor = cmd.Op2; Log($"индекс резистора -> {_resistor}"); }
                        break;

                    case CommandType.Loop:
                        if (cmd.Op1 == 0)
                            RunLoop_Cuvette(cmd.Op2, cmd.Op3, cmds1, cmds2);
                        else
                            throw new Exception("loop по resistor разрешён только внутри loop по cuvette");
                        break;

                    case CommandType.Save:
                        WriteExperiment();
                        break;

                    case CommandType.SaveAll:
                        WriteAll();
                        break;

                    case CommandType.Pause:
                        Log("Пауза до окончания диалога");
                        MessageBox.Show("Пауза", "Пауза", MessageBoxButton.OK);
                        break;

                    case CommandType.Experiment:
                        SingleExperiment(_resistor, _cuvette);
                        break;

                    case CommandType.Lines:
                        Log($"Обработано строк: {processedLines}");
                        break;

                    case CommandType.Log:
                        Log(cmd.Message);
                        break;
                }
            }

            Log("вышли из Emulate");
            _fout?.Close();
        }

        private void RunLoop_Cuvette(int low, int high, List<ScriptCommand> cmds1, List<ScriptCommand> cmds2)
        {
            Log($"стартуем цикл по кювете: {low}..{high}");
            for (int c = low; c <= high; c++)
            {
                _cuvette = c;
                Log($"номер кюветы -> {_cuvette}");

                foreach (var cmd in cmds1)
                {
                    switch (cmd.Cmd)
                    {
                        case CommandType.Loop when cmd.Op1 == 1:
                            RunLoop_Resistor(cmd.Op2, cmd.Op3, cmds2);
                            break;
                        case CommandType.Experiment:
                            SingleExperiment(_resistor, _cuvette);
                            break;
                        case CommandType.Pause:
                            Log("Пауза до окончания диалога");
                            MessageBox.Show("Пауза", "Пауза", MessageBoxButton.OK);
                            break;
                        case CommandType.Lines:
                            Log($"Обработано строк: {cmds1.Count}");
                            break;
                        case CommandType.Log:
                            Log(cmd.Message);
                            break;
                    }
                }
            }
        }

        private void RunLoop_Resistor(int low, int high, List<ScriptCommand> cmds2)
        {
            Log($"стартуем цикл по резистору: {low}..{high}");
            for (int r = low; r <= high; r++)
            {
                _resistor = r;
                Log($"индекс резистора -> {_resistor}");

                foreach (var cmd in cmds2)
                {
                    switch (cmd.Cmd)
                    {
                        case CommandType.Experiment:
                            SingleExperiment(r, _cuvette);
                            break;
                        case CommandType.Pause:
                            Log("Пауза до окончания диалога");
                            MessageBox.Show("Пауза", "Пауза", MessageBoxButton.OK);
                            break;
                        case CommandType.Log:
                            Log(cmd.Message);
                            break;
                    }
                }
            }
        }

        private void SingleExperiment(int r, int c)
        {
            Log($"зашли в SingleExperiment: R={r}, C={c}");
            System.Threading.Thread.Sleep(100);
            Log("вышли из SingleExperiment");
        }

        private void WriteExperiment() => Log("Записали одиночный эксперимент");
        private void WriteAll() => Log("Записали все эксперименты");

        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logger.Log(message);
            });
            _fout?.WriteLine(message);
        }
    }
}
