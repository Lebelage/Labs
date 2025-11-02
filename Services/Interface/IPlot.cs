using Lab1.Model;
using ScottPlot;
using System.Collections.Generic;


namespace Lab1.Services.Interface
{
    internal interface IPlot
    {
        public void Initialize(WpfPlot plot, string yAxisName, string xAxisName);

        public void Draw(List<SignalModel> signal);

        public void Draw(List<FourierModel> signal);
    }
}
