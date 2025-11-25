using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.PInvoke
{


    [ComImport]
    [Guid("5D1CDD50-82DB-4BAC-AD36-CCF7B602B27F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface _IOscEvents
    {
        [PreserveSig] int OnDataReady(uint nChannelsMask);

    }

    [ClassInterface(ClassInterfaceType.None)]
    public class OscSink : _IOscEvents
    {
        private readonly Action<uint> _callback;

        public OscSink(Action<uint> callback)
        {
            _callback = callback;
        }

        public int OnDataReady(uint nChannelsMask)
        {
            _callback(nChannelsMask);
            return 0; // S_OK
        }
    }

    [ComImport]
    [Guid("27603805-73E0-476B-B207-668B69191F96")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOscChannel
    {
        [PreserveSig] int get_Enabled(out int pVal);
        [PreserveSig] int get_DataSizeBounds(int bMax, out uint pVal);
        [PreserveSig] int get_RISEnabled(out int pVal);
        [PreserveSig] int get_RisDataSizeBounds(int bMax, out uint pVal);
        [PreserveSig] int get_TimeBaseBounds(int bMax, out uint pVal);
        [PreserveSig] int get_TimeBaseToFreq(uint nTimeBase, out double pfFreq);
        [PreserveSig] int get_RISTimeBaseBounds(int bMax, out uint pVal);
        [PreserveSig] int get_RISTimeBaseToFreq(uint nTimeBase, out double pfFreq);
        [PreserveSig] int get_GainsCount(out uint pVal);
        [PreserveSig] int get_GainToVolts(uint nGain, out double pfVolts);
        [PreserveSig] int get_InputTypesCount(out uint pVal);

        [PreserveSig]
        int get_InputTypeToBSTR(uint nType,
            [MarshalAs(UnmanagedType.BStr)] out string pBSTR);

        [PreserveSig]
        int get_PretriggerBounds(int bRIS, int bMax, out int pVal);

        [PreserveSig] int get_ZeroLevelsCount(out uint pVal);
        [PreserveSig] int get_ZeroLevelToVolts(uint nZeroLevel, out double pfVolts);
        [PreserveSig] int get_CatchGlitchesEnabled(out int pVal);
        [PreserveSig] int get_TriggerBounds(int bMax, out uint pVal);
        [PreserveSig] int get_TriggerLevelToVolts(int bExternal, uint nVal, out double pfVolts);

        [PreserveSig] int get_Active(out int pVal);
        [PreserveSig] int set_Active(int pVal);

        [PreserveSig] int get_DataSize(out uint pVal);
        [PreserveSig] int set_DataSize(uint pVal);

        [PreserveSig] int get_RISDataSize(out uint pVal);
        [PreserveSig] int set_RISDataSize(uint pVal);

        [PreserveSig] int get_RIS(out int pVal);
        [PreserveSig] int set_RIS(int pVal);

        [PreserveSig] int get_TimeBase(out uint pVal);
        [PreserveSig] int set_TimeBase(uint pVal);

        [PreserveSig] int get_RISTimeBase(out uint pVal);
        [PreserveSig] int set_RISTimeBase(uint pVal);

        [PreserveSig] int get_Gain(out uint pVal);
        [PreserveSig] int set_Gain(uint pVal);

        [PreserveSig] int get_InputType(out uint pVal);
        [PreserveSig] int set_InputType(uint pVal);

        [PreserveSig] int get_PreTrigger(out int pVal);
        [PreserveSig] int set_PreTrigger(int pVal);

        [PreserveSig] int get_ZeroLevel(out uint pVal);
        [PreserveSig] int set_ZeroLevel(uint pVal);

        [PreserveSig] int get_CatchGlitches(out int pVal);
        [PreserveSig] int set_CatchGlitches(int pVal);

        [PreserveSig] int get_TriggerSlopeNegative(out int pVal);
        [PreserveSig] int set_TriggerSlopeNegative(int pVal);

        [PreserveSig] int get_TriggerCouplingDC(out int pVal);
        [PreserveSig] int set_TriggerCouplingDC(int pVal);

        [PreserveSig] int get_TriggerExternal(out int pVal);
        [PreserveSig] int set_TriggerExternal(int pVal);

        [PreserveSig] int get_TriggerLevel(out uint pVal);
        [PreserveSig] int set_TriggerLevel(uint pVal);

        [PreserveSig] int get_TriggerNormal(out int pVal);
        [PreserveSig] int set_TriggerNormal(int pVal);

        [PreserveSig] int get_BitsCount(out uint pVal);

        [PreserveSig]
        int get_ID([MarshalAs(UnmanagedType.BStr)] out string pID);

        [PreserveSig]
        int get_IconRes(uint iProcessID, out uint hModule, out uint dwRes);

        // double[] output
        [PreserveSig]
        int GetSignalData(uint nCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] out double[] pData);

        // uint[] output
        [PreserveSig]
        int GetDigitalData(uint nCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] out uint[] pData);

        [PreserveSig] int get_PostHistoryCycles(out uint pVal);
        [PreserveSig] int set_PostHistoryCycles(uint pVal);

        [PreserveSig] int get_RISFixedRAM(out int pVal);
        [PreserveSig] int set_RISFixedRAM(int pVal);

        [PreserveSig] int get_RISHistogrammCycles(out uint pVal);
        [PreserveSig] int set_RISHistogrammCycles(uint pVal);

        [PreserveSig] int get_RISHistogramm(out int pVal);
        [PreserveSig] int set_RISHistogramm(int pVal);

        [PreserveSig] int VoltsToCode(double Volts, out double pVal);
        [PreserveSig] int CodeToVolts(double Code, out double pVal);

        [PreserveSig] int get_AverageType(out uint pVal);
        [PreserveSig] int set_AverageType(uint pVal);

        [PreserveSig] int get_AverageNum(out uint pVal);
        [PreserveSig] int set_AverageNum(uint pVal);

        [PreserveSig] int get_Averages(out int pVal);
        [PreserveSig] int set_Averages(int pVal);

        [PreserveSig] int get_TriggerInputTypesCount(out uint pVal);

        [PreserveSig]
        int get_TriggerInputTypeToBSTR(uint nType,
            [MarshalAs(UnmanagedType.BStr)] out string pBSTR);

        [PreserveSig] int get_TriggerInputType(out uint pVal);
        [PreserveSig] int set_TriggerInputType(uint pVal);

        [PreserveSig] int get_CyclicRun(out int pVal);
        [PreserveSig] int set_CyclicRun(int pVal);

        [PreserveSig] int get_VoltsToZeroLevel(double fVolts, out uint pZeroLev);

        [PreserveSig] int get_ProbeCoeff(out double pVal);
        [PreserveSig] int set_ProbeCoeff(double pVal);

        [PreserveSig] int get_TriggerProbeCoeff(out double pVal);
        [PreserveSig] int set_TriggerProbeCoeff(double pVal);

        [PreserveSig] int get_TrgFilter(out ushort pVal);
        [PreserveSig] int set_TrgFilter(ushort pVal);

        [PreserveSig] int get_TrgFilterCount(out ushort pVal);

        [PreserveSig] int SigSearch();

        // VARIANT → object
        [PreserveSig] int GetDigitalDataEx(uint nCount, out object pData);

        [PreserveSig] int set_SamplingFreq(double pVal);
        [PreserveSig] int get_SamplingFreq(out double pVal);

        [PreserveSig] int GetSignalDataEx(uint nCount, out object pData);

        [PreserveSig] int get_SmoothSize(out uint pnSmoothPoints);
        [PreserveSig] int set_SmoothSize(uint pnSmoothPoints);

        [PreserveSig] int set_SmoothEnabled(int pbSmoothEn);
        [PreserveSig] int get_SmoothEnabled(out int pbSmoothEn);

        [PreserveSig] int set_SmoothPassCnt(uint pnPassCnt);
        [PreserveSig] int get_SmoothPassCnt(out uint pnPassCnt);

        [PreserveSig] int get_TriggerChannel(out uint pnChInd);
        [PreserveSig] int set_TriggerChannel(uint pnChInd);

        [PreserveSig] int get_TriggerACDC(out int pbOpenInput);
        [PreserveSig] int set_TriggerACDC(int pbOpenInput);

        [PreserveSig]
        int GetSignalDataF4(uint nCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] out float[] pData);

        [PreserveSig] int GetSignalDataF4Ex(uint nCount, out object pData);

        [PreserveSig] int get_VoltsToTriggerLevel(int bExternal, double fVal, out uint pnCode);

        [PreserveSig]
        int get_TrgFilterToBSTR(ushort nIndex,
            [MarshalAs(UnmanagedType.BStr)] out string pBSTR);

        [PreserveSig] int get_Ohm50Enable(out int pVal);
        [PreserveSig] int set_Ohm50Enable(int pVal);

        [PreserveSig] int get_BWEnable(out int pVal);
        [PreserveSig] int set_BWEnable(int pVal);

        [PreserveSig] int get_VgaCoef(out ushort pVal);
        [PreserveSig] int set_VgaCoef(ushort pVal);

        [PreserveSig] int get_AdcCoef(out ushort pVal);
        [PreserveSig] int set_AdcCoef(ushort pVal);

        //[PreserveSig] int get_VgaConfig(out IVgaConfig pVal);
    }

    [ComImport]
    [Guid("60140064-971F-4BF3-9B21-09448A0A0DC4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOsc
    {
        // property ChannelsCount
        [PreserveSig]
        int get_ChannelsCount(out uint pVal);

        // property Channel
        [PreserveSig]
        int get_Channel(uint nChannel, out IOscChannel pVal);

        // property DeviceID
        [PreserveSig]
        int get_DeviceID([MarshalAs(UnmanagedType.BStr)] out string pbstrID);

        // property HardwareID
        [PreserveSig]
        int get_HardwareID(out uint pVal);

        // property Run (getter)
        [PreserveSig]
        int get_Run(out int pVal);

        // property Run (setter)
        [PreserveSig]
        int set_Run(int pVal);

        // property CalibrationsCount
        [PreserveSig]
        int get_CalibrationsCount(out uint pVal);

        // property Calibration
        //[PreserveSig]
        //int get_Calibration(uint nIndex, out IOscCalibration pVal);

        // property Location
        [PreserveSig]
        int get_Location([MarshalAs(UnmanagedType.BStr)] out string pVal);

        // property Port
        [PreserveSig]
        int get_Port(out uint pVal);

        // property Version
        [PreserveSig]
        int get_Version([MarshalAs(UnmanagedType.BStr)] out string pVal);

        // property Interface
        [PreserveSig]
        int get_Interface([MarshalAs(UnmanagedType.BStr)] out string pVal);

        // property Specific
        //[PreserveSig]
        //int get_Specific(out ISpecific pVal);

        // method SigSearch
        [PreserveSig]
        int SigSearch(ushort nChanMask);

        // property ReadyEventHandle (getter)
        [PreserveSig]
        int get_ReadyEventHandle(out uint pEventHandle);

        // property ReadyEventHandle (setter)
        [PreserveSig]
        int set_ReadyEventHandle(uint pEventHandle);

        // property VgaConfig
        //[PreserveSig]
        //int get_VgaConfig(out IVgaConfig pVal);
    }
}
