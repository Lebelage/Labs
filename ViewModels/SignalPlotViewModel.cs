using Lab1.Model;
using Lab1.Services;
using Lab1.Services.Interface;
using Lab1.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.ViewModels
{
    class SignalPlotViewModel : ViewModel
    {
        private WpfPlot _Plot;
        public WpfPlot Plot { get => _Plot; set => Set(ref _Plot, value); }

        private IPlot _Signal;


        private void OnSignalDataHandled(object? sender, List<SignalModel> e)
        {
            if (e is null)
                return;

            _Signal.Draw(e);
        }

        public SignalPlotViewModel()
        {
            _Plot = new WpfPlot();
            _Signal = new PlotService();
            _Signal.Initialize(_Plot, "x(t)", "t");

            App.Services.GetRequiredService<IMathLink>().SignalDataHandled += OnSignalDataHandled;
        }

    }
}
