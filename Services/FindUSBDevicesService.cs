using Lab1.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;


namespace Lab1.Services
{
    [StructLayout(LayoutKind.Sequential)] public struct RAWINPUTDEVICELIST { public IntPtr hDevice; public uint dwType; }

    [StructLayout(LayoutKind.Explicit)]
    public struct RID_DEVICE_INFO
    {
        [FieldOffset(0)]
        public uint cbSize;
        [FieldOffset(4)]
        public uint dwType;
        [FieldOffset(8)]
        public RID_DEVICE_INFO_MOUSE mouse;
        [FieldOffset(8)]
        public RID_DEVICE_INFO_KEYBOARD keyboard;
        [FieldOffset(8)]
        public RID_DEVICE_INFO_HID hid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RID_DEVICE_INFO_MOUSE
    {
        public uint dwId;
        public uint dwNumberOfButtons;
        public uint dwSampleRate;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fHasHorizontalWheel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RID_DEVICE_INFO_KEYBOARD
    {
        public uint dwType;
        public uint dwSubType;
        public uint dwKeyboardMode;
        public uint dwNumberOfFunctionKeys;
        public uint dwNumberOfIndicators;
        public uint dwNumberOfKeysTotal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RID_DEVICE_INFO_HID
    {
        public uint dwVendorId;
        public uint dwProductId;
        public uint dwVersionNumber;
        public ushort usUsage;
        public ushort usUsagePage;
    }
    class FindUSBDevicesService : IUSBDeviceFinder
    {
        private struct DeviceInfo
        {
            public string? deviceName;
            public RID_DEVICE_INFO deviceInfo;
        }


        private const uint RIDI_DEVICENAME = 0x20000007;
        private const uint RIDI_DEVICEINFO = 0x2000000b;
        private const uint RIM_TYPEMOUSE = 0;
        private const uint RIM_TYPEKEYBOARD = 1;
        private const uint RIM_TYPEHID = 2;


        private List<DeviceInfo> Devices;
        public List<string> GetHIDDevices()
        {
            return Devices.Where(d => d.deviceInfo.dwType == RIM_TYPEHID).Select(d => d.deviceName).ToList();
        }

        public List<string> GetKeyboards()
        {
            return Devices.Where(d => d.deviceInfo.dwType == RIM_TYPEKEYBOARD).Select(d => d.deviceName).ToList();
        }

        public List<string> GetMouses()
        {
            return Devices.Where(d => d.deviceInfo.dwType == RIM_TYPEMOUSE).Select(d => d.deviceName).ToList();
        }

        public bool FindDevices()
        {
            uint nDevices = 0;
            WinAPIWrapper.GetRawInputDeviceList(IntPtr.Zero, ref nDevices, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

            if (nDevices < 1)
                return false;

            IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(nDevices * Marshal.SizeOf(typeof(RAWINPUTDEVICELIST))));

            if (pRawInputDeviceList == IntPtr.Zero)
                return false;

            try
            {
                int nResult = (int)WinAPIWrapper.GetRawInputDeviceList(pRawInputDeviceList, ref nDevices, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));
                if (nResult < 0)
                    return false;


                for (uint i = 0; i < nDevices; i++)
                {
                    RAWINPUTDEVICELIST device = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                        pRawInputDeviceList + (int)(i * Marshal.SizeOf(typeof(RAWINPUTDEVICELIST))),
                        typeof(RAWINPUTDEVICELIST));

                    uint nBufferSize = 0;
                    nResult = (int)WinAPIWrapper.GetRawInputDeviceInfo(device.hDevice, RIDI_DEVICENAME, IntPtr.Zero, ref nBufferSize);

                    if (nResult < 0)        
                        continue;

                    IntPtr wcDeviceName = Marshal.AllocHGlobal((int)nBufferSize * sizeof(char));
                    if (wcDeviceName == IntPtr.Zero)
                        continue;
                    try
                    {

                        nResult = (int)WinAPIWrapper.GetRawInputDeviceInfo(device.hDevice, RIDI_DEVICENAME, wcDeviceName, ref nBufferSize);
                        if (nResult < 0)
                            continue;
                        

                        string deviceName = Marshal.PtrToStringUni(wcDeviceName);

                        RID_DEVICE_INFO rdiDeviceInfo = new RID_DEVICE_INFO();
                        rdiDeviceInfo.cbSize = (uint)Marshal.SizeOf(typeof(RID_DEVICE_INFO));
                        nBufferSize = rdiDeviceInfo.cbSize;

                        IntPtr pDeviceInfo = Marshal.AllocHGlobal((int)nBufferSize);
                        try
                        {
                            nResult = (int)WinAPIWrapper.GetRawInputDeviceInfo(device.hDevice, RIDI_DEVICEINFO, pDeviceInfo, ref nBufferSize);

                            if (nResult < 0)
                                continue;

                            rdiDeviceInfo = (RID_DEVICE_INFO)Marshal.PtrToStructure(pDeviceInfo, typeof(RID_DEVICE_INFO));
                            Devices.Add(new DeviceInfo() { deviceName = deviceName, deviceInfo = rdiDeviceInfo });
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(pDeviceInfo);
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(wcDeviceName);
                    }
                }
            }
            finally
            {
                // Clean Up - Free Memory
                Marshal.FreeHGlobal(pRawInputDeviceList);
            }

            if (Devices.Count == 0)
                return false;

            return true;
        }    
        
        
        public FindUSBDevicesService()
        {
            Devices = new();
        }
    }
}
