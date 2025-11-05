using Lab1.Model;
using Lab1.Services.Interface;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Wolfram.NETLink;

namespace Lab1.Services
{
    internal class MathLinkService : Interface.IMathLink, IDisposable
    {
        public event EventHandler<bool>? ConnectionChanged;
        public event EventHandler<List<SignalModel>>? SignalDataHandled;
        public event EventHandler<List<FourierModel>>? FourierDataHandled;

        private IKernelLink _Link;
        public Task<bool> SelectKernelAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Title = "Выберите WolframKernel.exe",
                        Filter = "Wolfram Kernel|MathKernel.exe|Все файлы|*.*",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        string path = dialog.FileName;

                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            var args = $"-linkmode launch -linkname \"{path}\"";
                            _Link = MathLinkFactory.CreateKernelLink(args);
                            _Link.WaitAndDiscardAnswer();
                            ConnectionChanged?.Invoke(this, true);
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ConnectionChanged?.Invoke(this, false);
                    _Link?.Close();
                    return false;
                }

                return true;
            });

        }

        public Task SolveEquationAsync(string equation, double t0, double tmax, double dt)
        {
            return Task.Run(() =>
            {
                if (_Link is null)
                    throw new InvalidOperationException("Wolfram Link не инициализирован.");

                try
                {
                    string functionDefinition = equation;
                    _Link.Evaluate(functionDefinition);
                    _Link.WaitForAnswer();
                    _Link.NewPacket();

                    string signalCode = $@"
            timeData = Table[{{t, x[t]}}, {{t, {t0.ToString(CultureInfo.InvariantCulture)}, {tmax.ToString(CultureInfo.InvariantCulture)}, {dt.ToString(CultureInfo.InvariantCulture)}}}];
            signalData = timeData;
            If[!MatrixQ[signalData], Throw['Invalid signal data']];
            signalData";

                    _Link.Evaluate(signalCode);
                    _Link.WaitForAnswer();

                    if (_Link.GetType() == typeof(void))
                    {
                        throw new Exception("Ошибка при вычислении сигнала в Mathematica");
                    }
                    _Link.NewPacket();

                    _Link.Evaluate("Length[signalData]");
                    _Link.WaitForAnswer();
                    int signalPoints = _Link.GetInteger();

                    string fourierCode = @"
    signal = signalData[[All, 2]];
    n = Length[signal];
    (* Вычисляем Фурье-преобразование *)
    fourierTransform = Fourier[signal, FourierParameters -> {1, -1}];
    (* Смещаем нулевую частоту в центр *)
    fourierTransformShifted = RotateRight[fourierTransform, Floor[n/2]];
    (* Правильно вычисляем частоты *)
    freqStep = 1/(n*" + dt.ToString(CultureInfo.InvariantCulture) + @");
    freqs = Table[If[i <= Ceiling[n/2], (i-1)*freqStep, (i-1-n)*freqStep], {i, n}];
    (* Сортируем частоты для правильного отображения *)
    fourierData = Transpose[{freqs, Abs[fourierTransformShifted]}];
    (* Берем только положительные частоты для одного пика *)
    fourierDataPositive = Select[fourierData, First[#] >= 0 &];
    If[!MatrixQ[fourierDataPositive], Throw['Invalid Fourier data']];
    fourierDataPositive";

                    _Link.Evaluate(fourierCode);
                    _Link.WaitForAnswer();

                    if (_Link.GetType() == typeof(void))
                    {
                        throw new Exception("Ошибка при вычислении Фурье спектра в Mathematica");
                    }
                    _Link.NewPacket();

                    _Link.Evaluate("Length[fourierData]");
                    _Link.WaitForAnswer();
                    int fourierPoints = _Link.GetInteger();
                    _Link.NewPacket();

                    GetDataFromWolfram();

                }
                catch (Exception ex)
                {
                    _Link.NewPacket();
                }
            });
        }

        private void GetDataFromWolfram()
        {
            try
            {
                _Link.Evaluate("signalData");
                _Link.WaitForAnswer();
                var signalArray = (double[,])_Link.GetArray(typeof(double), 2);

                if (signalArray is null)
                    throw new ArgumentNullException();

                _Link.NewPacket();

                _Link.Evaluate("fourierData");
                _Link.WaitForAnswer();
                var fourierArray = (double[,])_Link.GetArray(typeof(double), 2);

                if (fourierArray is null)
                    throw new ArgumentNullException();

                _Link.NewPacket();

                Application.Current.Dispatcher.Invoke(
                    new Action(() =>
                    {
                        SignalDataHandled?.Invoke(this, HandleSignalData(signalArray));
                        FourierDataHandled?.Invoke(this, HandleFourierData(fourierArray));
                    }));
            }
            catch (Exception ex)
            {
                _Link?.NewPacket();
            }
        }

        private List<SignalModel> HandleSignalData(double[,] data)
        {
            return Enumerable
                .Range(0, data.GetLength(0))
                .Select(i => new SignalModel
                {
                    _X = data[i, 0],
                    _T = data[i, 1],
                }).ToList();
        }

        private List<FourierModel> HandleFourierData(double[,] data)
        {
            return Enumerable
                .Range(0, data.GetLength(0))
                .Select(i => new FourierModel
                {
                    _Frequency = data[i, 0],
                    _Amplitude = data[i, 1],
                }).ToList();
        }

        private void SendSetRequest(string request)
        {

        }

        private void SendGetRequest(string request, out object response)
        {
            response = null;
        }
        public Task SpectrumAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _Link?.Close();
        }
    }
}
