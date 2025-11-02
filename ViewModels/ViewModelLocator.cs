using Lab1.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;


namespace Lab1.ViewModels
{
    class ViewModelLocator : ViewModel
    {
        public MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();
        public SignalPlotViewModel SignalPlotVM => App.Services.GetRequiredService<SignalPlotViewModel>();
        public FourierPlotViewModel FourierPlotVM => App.Services.GetRequiredService<FourierPlotViewModel>();
    }
}
