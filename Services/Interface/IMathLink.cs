using Lab1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Services.Interface
{
    internal interface IMathLink
    {
        event EventHandler<bool>? ConnectionChanged;
        event EventHandler<List<SignalModel>>? SignalDataHandled;
        event EventHandler<List<FourierModel>>? FourierDataHandled;
        Task<bool> SelectKernelAsync();

        Task SolveEquationAsync(string equation, double t0, double tmax, double dt);

        Task SpectrumAsync();
    }
}
