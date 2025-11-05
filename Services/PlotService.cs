using Lab1.Model;
using Lab1.Services.Interface;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Services
{
    internal class PlotService : IPlot
    {
        private WpfPlot _Plot;
        public void Draw(List<SignalModel> signal)
        {
            _Plot.Plot.Clear();

            var x_val = signal.Select(s => s._X).ToArray();
            var y_val = signal.Select(s => s._T).ToArray();

            _Plot.Plot.AddScatter(x_val, y_val);
            _Plot.Refresh();
        }

        public void Draw(List<FourierModel> signal)
        {
            _Plot.Plot.Clear();

            var x_val = signal.Select(s => s._Frequency).ToArray();
            var y_val = signal.Select(s => s._Amplitude).ToArray();

            _Plot.Plot.AddScatter(x_val, y_val);
            _Plot.Refresh();
        }

        public void Initialize(WpfPlot plot, string yAxisName, string xAxisName)
        {
            _Plot = plot;

            _Plot.Plot.XLabel(xAxisName);
            _Plot.Plot.YLabel(yAxisName);
        }
    }
}
