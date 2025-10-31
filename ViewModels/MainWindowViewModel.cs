using Lab1.Commands;
using Lab1.Core.Device.Interfaces;
using Lab1.Core.ScriptInterpreter.Interfaces;
using Lab1.Core.ScriptInterpreter.Services;
using Lab1.Core.ScriptInterpreter.Utils;
using Lab1.Infrastructure.enums;
using Lab1.Services;
using Lab1.Services.Interface;
using Lab1.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.ApplicationModel.CommunicationBlocking;

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

        #region COMPorts : ObservableCollection<string> - Available computer COM ports list
        /// <summary>Available computer COM ports list</summary>
        public ObservableCollection<string> COMPorts { get; } = new() { };
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

        #region ScriptText : string
        private string _ScriptText;
        public string ScriptText { get => _ScriptText; set => Set(ref _ScriptText, value); }
        #endregion

        #region LogText : string
        private string _LogText;
        public string LogText { get => _LogText; set => Set(ref _LogText, value); }
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
            //App.Services.GetRequiredService<IConnection>().SetCuvetteAndResistorAsync(1, 1);
            App.Services.GetRequiredService<IDllWorkerService>().CallSum("C:\\Users\\neotro\\source\\repos\\Labs\\sum.dll", "sum", 2, 9);
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

        #region (Command) : ConnectionCommand
        public ICommand ConnectionCommand { get; }
        private bool CanConnectionCommandExecute(object p) => true;
        private void OnConnectionCommandExecuted(object p) 
        {
            communication.CreateConnectionSource("COM1");

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

        #region (Command) : StartScriptCommand
        public ICommand StartScriptCommand { get; }
        

        private bool CanStartScriptCommandExecute(object p) => true;
        private void OnStartScriptCommandExecuted(object p)
        {
            Task.Run(() => StartScript());
        }
        #endregion

        #endregion


        #region Methods
        private void ChangeControl(ControlsType sender) 
        {
            CommunicationControlVisibility = Visibility.Collapsed;
            RegistryAndDevicesControlVisibility = Visibility.Collapsed;
            ScriptControlVisibility = Visibility.Collapsed;

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
        #endregion

        #region Handlers
        private void OnConnectionChanged(object? sender, bool e)
        {
            IsConnected = e;
            ConnectionButtonName = e ? "Disconnect" : "Connect";
            IsConnectionButtonEnabled = true;
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

            FindRegPathCommand = new LambdaCommand(OnFindRegPathCommandExecuted, CanFindRegPathCommandExecute);
            FindDevicesCommand = new LambdaCommand(OnFindDevicesCommandExecuted, CanFindDevicesCommandExecute);
            ConnectionCommand = new LambdaCommand(OnConnectionCommandExecuted, CanConnectionCommandExecute);
            SendSetRequestCommand = new LambdaCommand(OnSendSetRequestCommandExecuted, CanSendSetRequestCommandExecute);
            StartScriptCommand = new LambdaCommand(OnStartScriptCommandExecuted, CanStartScriptCommandExecute);

            OpenCommunicationControlCommand = new LambdaCommand(OnOpenCommunicationControlCommandExecuted, CanOpenCommunicationControlCommandExecute);
            OpenRegistryAndDeviceControlCommand = new LambdaCommand(OnOpenRegistryAndDeviceControlCommandExecuted, CanOpenRegistryAndDeviceControlCommandExecute);
            OpenScriptControlCommand = new LambdaCommand(OnOpenScriptControlCommandExecuted, CanOpenScriptControlCommandExecute);

            communication.ConnectionChanged += OnConnectionChanged;
        }

    }
}
