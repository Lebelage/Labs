using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Lab1.Core.Libs
{
    public static class DllInvoke
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId,
                                      uint dwLanguageId, StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

        [DllImport("ole32.dll")]
        public static extern int CoCreateInstance(
        ref Guid rclsid, IntPtr pUnkOuter, uint dwClsContext, ref Guid riid, out IntPtr ppv);

        [DllImport("ole32.dll")]
        public static extern int CoInitialize(IntPtr pvReserved);

        [DllImport("ole32.dll")]
        public static extern void CoUninitialize();

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int LoadTypeLibEx(
        string szFile,
        REGKIND regKind,
        out IntPtr ppTLib);  

        public enum REGKIND { REGKIND_DEFAULT, REGKIND_REGISTER, REGKIND_NONE }

    }
}
