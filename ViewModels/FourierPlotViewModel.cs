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
    internal class FourierPlotViewModel : ViewModel
    {
        private WpfPlot _Plot;
        public WpfPlot Plot { get => _Plot; set => Set(ref _Plot, value); }

        private IPlot _Fourier;
        private void OnFourierDataHandled(object? sender, List<FourierModel> e)
        {
            if(e is null)
                return;

            _Fourier.Draw(e);
        }
        public FourierPlotViewModel()
        {
            _Plot = new WpfPlot();
            _Fourier = new PlotService();

            _Fourier.Initialize(_Plot, "|X(w)|", "w, HZ");

            App.Services.GetRequiredService<IMathLink>().FourierDataHandled += OnFourierDataHandled;
        }


    }
}
