using Lab1.Commands;
using Lab1.Core.Device.Interfaces;
using Lab1.Core.ScriptInterpreter.Interfaces;
using Lab1.Core.ScriptInterpreter.Services;
using Lab1.Core.ScriptInterpreter.Utils;
using Lab1.Infrastructure.enums;
using Lab1.Model;
using Lab1.Services;
using Lab1.Services.Interface;
using Lab1.ViewModels.Base;
using Lab1.Views.controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScottPlot.Plottable;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Lab1.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region Fields
        private readonly string __UNDEFINED = "Undefined";

        private ICommandExecutor _commandExecutor;
        private IScriptParser _scriptParser;

        #region DriverName : string
        private string _DriverName = "FTDIBUS";
        public string DriverName { get => _DriverName; set => Set(ref _DriverName, value); }
        #endregion

        #region DriverPath : string
        private string _DriverPath;
        public string DriverPath { get => _DriverPath; set => Set(ref _DriverPath, value); }
        #endregion

        #region MousesList : string
        private string _MousesList;
        public string MousesList { get => _MousesList; set => Set(ref _MousesList, value); }
        #endregion

        #region KeyboardsList : string
        private string _KeyboardsList;
        public string KeyboardsList { get => _KeyboardsList; set => Set(ref _KeyboardsList, value); }
        #endregion

        #region HIDsList : string
        private string _HIDsList;
        public string HIDsList { get => _HIDsList; set => Set(ref _HIDsList, value); }
        #endregion

        #region RegistryAndDevicesControlVisibility : Visibility
        private Visibility _RegistryAndDevicesControlVisibility;
        public Visibility RegistryAndDevicesControlVisibility { get => _RegistryAndDevicesControlVisibility; set => Set(ref _RegistryAndDevicesControlVisibility, value); }
        #endregion

        #region CommunicationControlVisibility : Visibility
        private Visibility _CommunicationControlVisibility;
        public Visibility CommunicationControlVisibility { get => _CommunicationControlVisibility; set => Set(ref _CommunicationControlVisibility, value); }
        #endregion

        #region ScriptControlVisibility : Visibility
        private Visibility _ScriptControlVisibility;
        public Visibility ScriptControlVisibility { get => _ScriptControlVisibility; set => Set(ref _ScriptControlVisibility, value); }
        #endregion

        #region DLLWorkerControlVisibility : Visibility
        private Visibility _DLLWorkerControlVisibility;
        public Visibility DLLWorkerControlVisibility { get => _DLLWorkerControlVisibility; set => Set(ref _DLLWorkerControlVisibility, value); }
        #endregion

        #region MathLinkControlVisibility : Visibility
        private Visibility _MathLinkControlVisibility;
        public Visibility MathLinkControlVisibility { get => _MathLinkControlVisibility; set => Set(ref _MathLinkControlVisibility, value); }
        #endregion

        #region COMPorts : ObservableCollection<string> - Available computer COM ports list
        /// <summary>Available computer COM ports list</summary>
        private ObservableCollection<string> _COMPorts;
        public ObservableCollection<string> COMPorts { get => _COMPorts; set => Set(ref _COMPorts, value); }
        #endregion

        #region BaudRates : ObservableCollection<int> - Default baud rates list
        /// <summary>Default baud rates list</summary>
        public ObservableCollection<uint> BaudRates { get; } = new() { 110u, 300u, 600u, 1200u, 2400u, 4800u, 9600u, 14400u, 19200u, 38400u, 57600u, 115200u, 128000u, 256000u, 500000u };
        #endregion

        #region Paraties : ObservableCollection<Parity> - Default parities list
        /// <summary>Default parities list</summary>
        public ObservableCollection<Parity> Parities { get; } = new() { Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space };
        #endregion

        #region StopBitses : ObservableCollection<StopBits> - Default stop bits list
        /// <summary>Default stop bits list</summary>
        public ObservableCollection<StopBits> StopBites { get; } = new() {StopBits.None, StopBits.One, StopBits.Two, StopBits.OnePointFive };
        #endregion

        #region DataBits : ObservableCollection<byte> - Default data bits list
        /// <summary>Default data bits list</summary>
        public ObservableCollection<byte> DataBits { get; } = new() { 5, 6, 7, 8 };
        #endregion

        #region SelectedCOMPort : string - Selected COM port name
        /// <summary>Selected COM port name</summary>
        private string? _SelectedCOMPort;
        /// <summary>Selected COM port name</summary>
        public string? SelectedCOMPort
        {
            get { return _SelectedCOMPort; }
            set
            {
                Set(ref _SelectedCOMPort, value);
            }
        }
        #endregion

        #region SelectedBaudRate : uint - Selected baud rate
        /// <summary>Selected baud rate</summary>
        private uint _SelectedBaudRate = 115200;
        /// <summary>Selected baud rate</summary>
        public uint SelectedBaudRate
        {
            get { return _SelectedBaudRate; }
            set
            {
                Set(ref _SelectedBaudRate, value);
            }
        }
        #endregion

        #region SelectedParity : Parity - Selected parity
        /// <summary>Selected parity</summary>
        private Parity _SelectedParity = Parity.None;
        /// <summary>Selected parity</summary>
        public Parity SelectedParity
        {
            get { return _SelectedParity; }
            set
            {
                Set(ref _SelectedParity, value); 
            }
        }
        #endregion

        #region DataBits : byte? - Selected data bits
        /// <summary>Selected data bits</summary>
        private byte? _SelectedDataBits = 8;
        /// <summary>Selected data bits</summary>
        public byte? SelectedDataBits
        {
            get { return _SelectedDataBits; }
            set
            {
                Set(ref _SelectedDataBits, value);
            }
        }
        #endregion

        #region SelectedStopBits : StopBits - Selected stop bits
        /// <summary>Selected stop bits</summary>
        private StopBits _SelectedStopBits = StopBits.None;
        /// <summary>Selected stop bits</summary>
        public StopBits SelectedStopBits
        {
            get { return _SelectedStopBits; }
            set
            {
                Set(ref _SelectedStopBits, value); 
            }
        }
        #endregion

        #region ConnectionButtonName : string
        private string _ConnectionButtonName = "Connect";
        public string ConnectionButtonName { get => _ConnectionButtonName; set => Set(ref _ConnectionButtonName, value); }
        #endregion

        #region IsConnectionButtonEnabled : bool
        private bool _IsConnectionButtonEnabled = true;
        public bool IsConnectionButtonEnabled { get => _IsConnectionButtonEnabled; set => Set(ref _IsConnectionButtonEnabled, value); }
        #endregion

        #region CuvetteNum : int
        private int _CuvetteNum = 1;
        public int CuvetteNum { get => _CuvetteNum; set => Set(ref _CuvetteNum, value); }
        #endregion

        #region ResistorIndex : int
        private int _ResistorIndex = 0;
        public int ResistorIndex { get => _ResistorIndex; set => Set(ref _ResistorIndex, value); }
        #endregion

        #region DllFilePath : string
        private string _DllFilePath;
        public string DllFilePath { get => _DllFilePath; set => Set(ref _DllFilePath, value); }
        #endregion

        #region FuncName : string
        private string _FuncName;
        public string FuncName { get => _FuncName; set => Set(ref _FuncName, value); }
        #endregion

        #region DllSumXParam : inst
        private int _DllSumXParam;
        public int DllSumXParam { get => _DllSumXParam; set => Set(ref _DllSumXParam, value); }
        #endregion

        #region DllSumYParam : int
        private int _DllSumYParam;
        public int DllSumYParam { get => _DllSumYParam; set => Set(ref _DllSumYParam, value); }
        #endregion

        #region DllWorkerServiceResult : string
        private string _DllWorkerServiceResult;
        public string DllWorkerServiceResult { get => _DllWorkerServiceResult; set => Set(ref _DllWorkerServiceResult, value); }
        #endregion

        #region ScriptText : string
        private string _ScriptText;
        public string ScriptText { get => _ScriptText; set => Set(ref _ScriptText, value); }
        #endregion

        #region LogText : string
        private string _LogText;
        public string LogText { get => _LogText; set => Set(ref _LogText, value); }
        #endregion

        #region SignalPoints : ObservableCollection<SignalModel>
        private ObservableCollection<SignalModel> _SignalPoints = new();
        public ObservableCollection<SignalModel> SignalPoints { get => _SignalPoints; set => Set(ref _SignalPoints, value); }
        #endregion

        #region Equation : string
        private string _Equation = "Sin[2Pi*t]";
        public string Equation { get => _Equation; set => Set(ref _Equation, value); }
        #endregion

        #region T0 : double
        private double _T0 = 0;
        public double T0 { get => _T0; set => Set(ref _T0, value); }
        #endregion

        #region Tmax : double
        private double _Tmax = 20;
        public double Tmax { get => _Tmax; set => Set(ref _Tmax, value); }
        #endregion

        #region Dt : double
        private double _Dt = 0.1d;
        public double Dt { get => _Dt; set => Set(ref _Dt, value); }
        #endregion

        #region SignalPlot : PlotService
        private PlotService _SignalPlot;
        public PlotService SignalPlot { get => _SignalPlot; set => Set(ref _SignalPlot, value); }
        #endregion

        #region FourierPlot : PlotService
        private PlotService _FourierPlot;
        public PlotService FourierPlot { get => _FourierPlot; set => Set(ref _FourierPlot, value); }
        #endregion

        #region IsSolveEnabled : bool
        private bool _IsSolveEnabled = false;
        public bool IsSolveEnabled { get => _IsSolveEnabled; set => Set(ref _IsSolveEnabled, value); }
        #endregion

        public bool IsExecuting { get; private set; }
        private bool IsConnected = false;
        #endregion

        #region services
        private IFindRegistryKey findRegistryKey;
        private IUSBDeviceFinder usbDeviceFinder;
        private IConnection communication;
        #endregion

        #region Commands

        #region (Command) : FindRegPathCommand
        public ICommand FindRegPathCommand { get; }
        private bool CanFindRegPathCommandExecute(object p) => true;
        private void OnFindRegPathCommandExecuted(object p) 
        {
            if (DriverName is not null) 
            {
                DriverPath = findRegistryKey.Find(DriverName);
                return;
            }

            DriverPath = __UNDEFINED;

        }
        #endregion

        #region (Command) : FindDevicesCommand
        public ICommand FindDevicesCommand { get; }
        private bool CanFindDevicesCommandExecute(object p) => true;
        private void OnFindDevicesCommandExecuted(object p)
        {
            if(!usbDeviceFinder.FindDevices())
                return;

            var hids = usbDeviceFinder.GetHIDDevices();
            foreach(var hid in hids) 
            {
                HIDsList += hid + "\n";
            }

            var mouses = usbDeviceFinder.GetMouses();
            foreach (var mouse in mouses)
            {
                MousesList += mouse + "\n";
            }

            var keyboards = usbDeviceFinder.GetKeyboards();
            foreach (var keyboard in keyboards)
            {
                KeyboardsList += keyboard + "\n";
            }

        }
        #endregion

        #region (Command) : OpenRegistryAndDeviceControlCommand
        public ICommand OpenRegistryAndDeviceControlCommand { get; }
        private bool CanOpenRegistryAndDeviceControlCommandExecute(object p) => true;
        private void OnOpenRegistryAndDeviceControlCommandExecuted(object p) => ChangeControl(ControlsType.RegistryAndDevices);
        #endregion

        #region (Command) : OpenScriptControlCommand
        public ICommand OpenScriptControlCommand { get; }
        private bool CanOpenScriptControlCommandExecute(object p) => true;
        private void OnOpenScriptControlCommandExecuted(object p) => ChangeControl(ControlsType.Script);
        #endregion

        #region (Command) : OpenCommunicationControlCommand
        public ICommand OpenCommunicationControlCommand { get; }
        private bool CanOpenCommunicationControlCommandExecute(object p) => true;
        private void OnOpenCommunicationControlCommandExecuted(object p) => ChangeControl(ControlsType.Communication);
        #endregion

        #region (Command) : OpenDllWorkerControlCommand
        public ICommand OpenDllWorkerControlCommand { get; }
        private bool CanOpenDllWorkerControlCommandExecute(object p) => true;
        private void OnOpenDllWorkerControlCommandExecuted(object p) => ChangeControl(ControlsType.DLLWorker);
        #endregion

        #region (Command) : OpenMathLinkCommand
        public ICommand OpenMathLinkCommand { get; }
        private bool CanOpenMathLinkCommandExecute(object p) => true;
        private void OnOpenMathLinkCommandExecuted(object p) => ChangeControl(ControlsType.MathLink);
        #endregion

        #region (Command) : ConnectionCommand
        public ICommand ConnectionCommand { get; }
        private bool CanConnectionCommandExecute(object p) => true;
        private void OnConnectionCommandExecuted(object p) 
        {
            communication.CreateConnectionSource(SelectedCOMPort);

            if (!IsConnected) 
            {
                communication.OpenConnectionAsync();
            }
            else 
            {
                communication.CloseConnectionAsync();
            }

            IsConnectionButtonEnabled = false;
        }
        #endregion

        #region (Command) : SendSetRequestCommand
        public ICommand SendSetRequestCommand { get; }
        private bool CanSendSetRequestCommandExecute(object p) => true;
        private void OnSendSetRequestCommandExecuted(object p)
        {
            if (IsConnected) 
            {
                communication.SetCuvetteAndResistorAsync((byte)CuvetteNum, (byte)ResistorIndex);
            }
        }
        #endregion

        #region (Command) : LoadSumDLLCommand
        public ICommand LoadSumDLLCommand { get; }
        private bool CanLoadSumDLLCommandExecute(object p) => true;
        private void OnLoadSumDLLCommandExecuted(object p)
        {
            //App.Services.GetRequiredService<IDllWorkerService>().InitOcs();
            if (!App.Services.GetRequiredService<IDllWorkerService>().DllLoad(DllFilePath)) 
            {
                DllWorkerServiceResult = "Dll не была загружена";
                return;
            }

            DllWorkerServiceResult = "Dll была успешно загружена";
        }
        #endregion

        #region (Command) : SelectDllFuncCommand
        public ICommand SelectDllFuncCommand { get; }
        private bool CanSelectDllFuncCommandExecute(object p) => true;
        private void OnSelectDllFuncCommandExecuted(object p)
        {
            if (!App.Services.GetRequiredService<IDllWorkerService>().FunctionSelect(FuncName))
            {
                DllWorkerServiceResult = $"Функция {FuncName} не существует";
                return;
            }

            DllWorkerServiceResult = $"Функция {FuncName} была выбрана";
        }
        #endregion

        #region (Command) : CallFuncCommand
        public ICommand CallFuncCommand { get; }
        private bool CanCallFuncCommandExecute(object p) => true;
        private void OnCallFuncCommandExecuted(object p)
        {
            if (!App.Services.GetRequiredService<IDllWorkerService>().CallSum(DllSumXParam, DllSumYParam, out int res))
            {
                DllWorkerServiceResult = $"Функция {FuncName} не была вызвана";
                return;
            }
            DllWorkerServiceResult = $"Функция {FuncName} вернула {res}";
        }
        #endregion

        #region (Command) : StartScriptCommand
        public ICommand StartScriptCommand { get; }      
        private bool CanStartScriptCommandExecute(object p) => true;
        private void OnStartScriptCommandExecuted(object p)
        {
            Task.Run(() => StartScript());
        }
        #endregion

        #region (Command) : SelectKernelCommand
        public ICommand SelectKernelCommand { get; }
        private bool CanSelectKernelCommandExecute(object p) => true;
        private void OnSelectKernelCommandExecuted(object p)
        {
            //App.Services.GetRequiredService<IDllWorkerService>().InitOcs();
            SelectKernel();
        }
        #endregion

        #region (Command) : SolveEquationCommand
        public ICommand SolveEquationCommand { get; }
        private bool CanSolveEquationCommandExecute(object p) => true;
        private void OnSolveEquationCommandExecuted(object p)
        {
            SolveEquationAsync();
        }
        #endregion

        #endregion

        #region Methods
        private void ChangeControl(ControlsType sender) 
        {
            CommunicationControlVisibility = Visibility.Collapsed;
            RegistryAndDevicesControlVisibility = Visibility.Collapsed;
            ScriptControlVisibility = Visibility.Collapsed;
            DLLWorkerControlVisibility = Visibility.Collapsed;
            MathLinkControlVisibility = Visibility.Collapsed;

            if (sender == ControlsType.RegistryAndDevices) 
            {
                RegistryAndDevicesControlVisibility = Visibility.Visible;
            }
            else if(sender == ControlsType.Communication) 
            {
                CommunicationControlVisibility = Visibility.Visible;
            }
            else if(sender == ControlsType.Script) 
            {
                ScriptControlVisibility= Visibility.Visible;
            }
            else if (sender == ControlsType.DLLWorker)
            {
                DLLWorkerControlVisibility = Visibility.Visible;
            }
            else if(sender == ControlsType.MathLink) 
            {
                MathLinkControlVisibility = Visibility.Visible;
            }
        }

        private void StartScript()
        {
            LogText = "Сценарий интерпретирован успешно\n\n";
            try
            {
                var (cmds0, cmds1, cmds2, lines) = _scriptParser.Parse(ScriptText);
                _commandExecutor.Execute(cmds0, cmds1, cmds2, lines);
            }
            catch (Exception ex)
            {
                LogText += $"ОШИБКА: {ex.Message}\n";
            }
        }

        private async void SelectKernel() 
        {
            await App.Services.GetRequiredService<IMathLink>().SelectKernelAsync();
        }

        private async void SolveEquationAsync()
        {
            //await App.Services.GetRequiredService<IMathLink>().SolveEquationAsync(Equation,T0,Tmax,Dt);
            await App.Services.GetRequiredService<IMathLink>().GetFourierWithFilterAsync(Equation, T0,Tmax,Dt,1,5);
            await App.Services.GetRequiredService<IMathLink>().GetReconstructedSignalAsync(T0,Tmax,Dt,1,5);
        }

        private void UpdateCOMportsList() 
        {
            try
            {
                string com_pattern = @"COM\d+";
                COMPorts = new ObservableCollection<string>([.. SerialPort.GetPortNames().Where(s => Regex.IsMatch(s, com_pattern))]);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Handlers
        private void OnConnectionChanged(object? sender, bool e)
        {
            IsConnected = e;
            ConnectionButtonName = e ? "Disconnect" : "Connect";
            IsConnectionButtonEnabled = true;
        }

        private void OnSignalDataHandled(object? sender, List<SignalModel> e)
        {
            if (e is null)
                return;

            SignalPoints = new ObservableCollection<SignalModel>(e);
        }

        private void OnMathLinkConnectionChanged(object? sender, bool e)
        {
            IsSolveEnabled = e;
        }
        #endregion
        public MainWindowViewModel() 
        {

            findRegistryKey = App.Services.GetRequiredService<IFindRegistryKey>();
            usbDeviceFinder = App.Services.GetRequiredService<IUSBDeviceFinder>();
            communication = App.Services.GetRequiredService<IConnection>();
  
            _commandExecutor = new CommandExecutor();
            _scriptParser = new ScriptParser();

            Logger.LogMessage += (s, msg) => LogText += msg + "\n";

            RegistryAndDevicesControlVisibility = Visibility.Visible;
            CommunicationControlVisibility = Visibility.Collapsed;
            ScriptControlVisibility = Visibility.Collapsed;
            DLLWorkerControlVisibility = Visibility.Collapsed;
            MathLinkControlVisibility = Visibility.Collapsed;

            FindRegPathCommand = new LambdaCommand(OnFindRegPathCommandExecuted, CanFindRegPathCommandExecute);
            FindDevicesCommand = new LambdaCommand(OnFindDevicesCommandExecuted, CanFindDevicesCommandExecute);
            ConnectionCommand = new LambdaCommand(OnConnectionCommandExecuted, CanConnectionCommandExecute);
            SendSetRequestCommand = new LambdaCommand(OnSendSetRequestCommandExecuted, CanSendSetRequestCommandExecute);
            StartScriptCommand = new LambdaCommand(OnStartScriptCommandExecuted, CanStartScriptCommandExecute);
            CallFuncCommand = new LambdaCommand(OnCallFuncCommandExecuted, CanCallFuncCommandExecute);
            LoadSumDLLCommand = new LambdaCommand(OnLoadSumDLLCommandExecuted, CanLoadSumDLLCommandExecute);
            SelectDllFuncCommand = new LambdaCommand(OnSelectDllFuncCommandExecuted, CanSelectDllFuncCommandExecute);
            OpenDllWorkerControlCommand = new LambdaCommand(OnOpenDllWorkerControlCommandExecuted, CanOpenDllWorkerControlCommandExecute);
            OpenMathLinkCommand = new LambdaCommand(OnOpenMathLinkCommandExecuted, CanOpenMathLinkCommandExecute);
            SelectKernelCommand = new LambdaCommand(OnSelectKernelCommandExecuted, CanSelectKernelCommandExecute);
            SolveEquationCommand = new LambdaCommand(OnSolveEquationCommandExecuted, CanSolveEquationCommandExecute);

            OpenCommunicationControlCommand = new LambdaCommand(OnOpenCommunicationControlCommandExecuted, CanOpenCommunicationControlCommandExecute);
            OpenRegistryAndDeviceControlCommand = new LambdaCommand(OnOpenRegistryAndDeviceControlCommandExecuted, CanOpenRegistryAndDeviceControlCommandExecute);
            OpenScriptControlCommand = new LambdaCommand(OnOpenScriptControlCommandExecuted, CanOpenScriptControlCommandExecute);

            communication.ConnectionChanged += OnConnectionChanged;

            App.Services.GetRequiredService<IMathLink>().SignalDataHandled += OnSignalDataHandled;
            App.Services.GetRequiredService<IMathLink>().ConnectionChanged += OnMathLinkConnectionChanged;

            UpdateCOMportsList();

        }

        
    }
}
