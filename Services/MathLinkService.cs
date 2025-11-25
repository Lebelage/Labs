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
        private string _CurrentSignal;
        public Task<bool> SelectKernelAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    string path = "C:\\Program Files\\Wolfram Research\\Wolfram\\14.3\\MathKernel.exe";

                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        var args = $"-linkmode launch -linkname \"{path}\"";
                        _Link = MathLinkFactory.CreateKernelLink(args);
                        _Link.WaitAndDiscardAnswer();
                        ConnectionChanged?.Invoke(this, true);
                    }
                    return true;

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


        public Task GetReconstructedSignalAsync(double t0, double tmax, double dt, double lowFreq, double highFreq)
        {
            return Task.Run(() =>
            {
                if (_Link == null)
                    throw new InvalidOperationException("Wolfram Link не инициализирован.");

                try
                {
                string restoreCode = $@"
                t0 = {t0.ToString(CultureInfo.InvariantCulture)};
                tmax = {tmax.ToString(CultureInfo.InvariantCulture)};
                dt = {dt.ToString(CultureInfo.InvariantCulture)};
                lowFreq = {lowFreq.ToString(CultureInfo.InvariantCulture)};
                highFreq = {highFreq.ToString(CultureInfo.InvariantCulture)};

                t = Range[t0, tmax, dt];
                signal = {_CurrentSignal};
                n = Length[signal];
                dtValue = dt;
                fs = 1/dtValue;

                fourier = Fourier[signal, FourierParameters -> {{1, -1}}];

                freqs = Range[0, fs - fs/n, fs/n];

                filter = Table[
                    If[{lowFreq} <= Abs[freqs[[i]]] <= {highFreq}, 1, 0],
                    {{i, n}}
                ];
                filteredFourier = fourier * filter;

                restored = Re[InverseFourier[filteredFourier,
                                            FourierParameters -> {{1, -1}}]];
                restored = Chop[restored, 10^-12];

                result = Transpose[{{t, restored}}];
                N[result]          
            ";

                    _Link.Evaluate(restoreCode);
                    _Link.WaitForAnswer();
                    _Link.NewPacket();

                    _Link.Evaluate("result");
                    _Link.WaitForAnswer();
                    var result = (double[,])_Link.GetArray(typeof(double), 2);
                    _Link.NewPacket();

                    var points = Enumerable.Range(0, result.GetLength(0))
                        .Select(i => new SignalModel
                        {
                            _X = result[i, 0],
                            _T = result[i, 1]
                        }).ToList();

                    Application.Current.Dispatcher.Invoke(() => { SignalDataHandled?.Invoke(this, points); });
                }
                catch (Exception ex)
                {
                    _Link?.NewPacket();
                    throw;
                }
            });
        }
        public Task GetFourierWithFilterAsync(string signal, double t0, double tmax, double dt, double lowFreq, double highFreq)
        {

            return Task.Run(() =>
            {
                if (_Link == null) throw new InvalidOperationException("Wolfram Link не инициализирован.");

                _CurrentSignal = signal;

                try
                {

                    string signalCode = $@"
                        t = Range[{t0.ToString(CultureInfo.InvariantCulture)},
                                        {tmax.ToString(CultureInfo.InvariantCulture)},
                                        {dt.ToString(CultureInfo.InvariantCulture)}];
                        signal = {signal};
                        n = Length[signal];
                        dtValue = {dt.ToString(CultureInfo.InvariantCulture)};
                        fs = 1/dtValue;

                        (* ДПФ *)
                        fourier = Fourier[signal, FourierParameters -> {{1, -1}}];

                        (* Частотная ось *)
                        freqs = Range[0, fs - fs/n, fs/n];

                        (* ФНЧ *)
                        filter = Table[If[{lowFreq} <= Abs[freqs[[i]]] <= {highFreq}, 1, 0], {{i, n}}];
                        filteredFourier = fourier * filter;

                        (* Положительная часть *)
                        posCount = Ceiling[n/2];
                        freqsPos = Table[(i-1)/(n*dtValue), {{i, posCount}}];
                        amplitudes = Abs[Take[filteredFourier, posCount]];

                        (* ПРАВИЛЬНЫЙ TRANSPOSE *)
                        result = Transpose[{{freqsPos, amplitudes}}];
                        result
                    ";

                    _Link.Evaluate(signalCode);
                    _Link.WaitForAnswer();
                    _Link.NewPacket();

                    _Link.Evaluate("result");
                    _Link.WaitForAnswer();
                    var result = (double[,])_Link.GetArray(typeof(double), 2);
                    _Link.NewPacket();

                    var points = Enumerable.Range(0, result.GetLength(0))
                        .Select(i => new FourierModel
                        {
                            _Frequency = result[i, 0],
                            _Amplitude = result[i, 1]
                        }).ToList();

                    Application.Current.Dispatcher.Invoke(() => { FourierDataHandled?.Invoke(this, points); });
                }
                catch (Exception ex)
                {
                    _Link?.NewPacket();

                }
            });

        }
        public void Dispose()
        {
            _Link?.Close();
        }
    }
}
