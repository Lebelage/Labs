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
(* Получаем исходный сигнал *)
signal = signalData[[All, 2]];
n = Length[signal];
dtValue = " + dt.ToString(CultureInfo.InvariantCulture) + @";

(* 1. ПРЯМОЕ ФУРЬЕ-ПРЕОБРАЗОВАНИЕ *)
fourierTransform = Fourier[signal, FourierParameters -> {1, -1}];

(* Вычисляем частоты для полного спектра *)
freqStep = 1/(n * dtValue);
freqsFull = Table[If[i <= n/2, (i-1)*freqStep, (i-1-n)*freqStep], {i, n}];

(* 2. ФИЛЬТР НИЗКИХ ЧАСТОТ *)
cutoffFreq = 2.0; (* Частота среза - настройте по необходимости *)

(* Создаем фильтр низких частот *)
lowPassFilter = Table[If[Abs[freqsFull[[i]]] <= cutoffFreq, 1, 0], {i, n}];

(* Применяем фильтр к Фурье-спектру *)
filteredFourier = fourierTransform * lowPassFilter;

(* 3. ПОДГОТОВКА ДАННЫХ ДЛЯ ВЫВОДА *)

(* Исходный спектр (только положительные частоты) *)
fourierTransformPositive = Take[fourierTransform, Ceiling[n/2]];
freqsPositive = Table[(i-1)*freqStep, {i, Ceiling[n/2]}];
fourierData = Transpose[{freqsPositive, Abs[fourierTransformPositive]}];

(* Отфильтрованный спектр (только положительные частоты) *)
filteredFourierPositive = Take[filteredFourier, Ceiling[n/2]];
filteredFourierData = Transpose[{freqsPositive, Abs[filteredFourierPositive]}];

(* Возвращаем данные спектров *)
result = {
    ""originalSpectrum"" -> fourierData,
    ""filteredSpectrum"" -> filteredFourierData
};
result";

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
