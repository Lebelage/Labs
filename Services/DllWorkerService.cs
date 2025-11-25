using Lab1.Core.Libs;
using Lab1.Core.PInvoke;
using Lab1.Services.Interface;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Lab1.Core.Libs.DllInvoke;


namespace Lab1.Services
{
    internal class DllWorkerService : IDllWorkerService, IDisposable
    {
        private const uint CLSCTX_ALL = 0x17;
        private IntPtr hModule = IntPtr.Zero;
        private delegate int FunSummaDelegate(int x, int y);
        private FunSummaDelegate fun_summa;

        public bool CallSum(int x, int y, out int result)
        {
            result = default;

            if (hModule == IntPtr.Zero)
                return false;

            if (fun_summa is null)
                return false;

            result = fun_summa(x, y);

            return true;
        }
        public bool DllLoad(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                hModule = DllInvoke.LoadLibrary(filePath);
                if (hModule == IntPtr.Zero)
                {
                    Console.WriteLine($"Не удалось загрузить DLL: {filePath}");
                    return false;
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public bool FunctionSelect(string funcName)
        {
            if (hModule == IntPtr.Zero)
            {
                return false;
            }

            IntPtr procAddress = DllInvoke.GetProcAddress(hModule, funcName);

            if (procAddress == IntPtr.Zero)
            {
                Console.WriteLine($"Функция '{funcName}' не найдена!");
                return false;
            }

            fun_summa = Marshal.GetDelegateForFunctionPointer<FunSummaDelegate>(procAddress);

            return true;
        }

        public void InitOcs(string tlbPath = "C:\\Users\\neotro\\source\\repos\\OscDll\\OscB322.tlb", string coclassName = "OscDeviceB322", string interfaceName = "IOsc")
        {
            DllInvoke.CoUninitialize();
            DllInvoke.CoUninitialize();
            DllInvoke.CoUninitialize();

            var res = DllInvoke.CoInitialize(IntPtr.Zero);

            IntPtr pTypeLibUnk = IntPtr.Zero;

            int hr = DllInvoke.LoadTypeLibEx(tlbPath, REGKIND.REGKIND_NONE, out pTypeLibUnk);

            if (hr != 0 || pTypeLibUnk == IntPtr.Zero)
                throw new COMException($"LoadTypeLibEx failed: 0x{hr:X8}", hr);

            ITypeLib typeLib = (ITypeLib)Marshal.GetObjectForIUnknown(pTypeLibUnk);
            Marshal.Release(pTypeLibUnk);

            try
            {
                Guid clsid = Guid.Empty;
                Guid iid = Guid.Empty;

                int count = typeLib.GetTypeInfoCount();
                for (int i = 0; i < count; i++)
                {
                    typeLib.GetDocumentation(i, out string name, out _, out _, out _);
                    typeLib.GetTypeInfo(i, out ITypeInfo typeInfo);

                    if (typeInfo == null) continue;

                    IntPtr pAttr;
                    typeInfo.GetTypeAttr(out pAttr);
                    if (pAttr == IntPtr.Zero)
                    {
                        Marshal.ReleaseComObject(typeInfo);
                        continue;
                    }

                    TYPEATTR attr = Marshal.PtrToStructure<TYPEATTR>(pAttr);

                    if (name == coclassName) clsid = attr.guid;
                    else if (name == interfaceName) iid = attr.guid;

                    typeInfo.ReleaseTypeAttr(pAttr);
                    Marshal.ReleaseComObject(typeInfo);

                    if (clsid != Guid.Empty && iid != Guid.Empty)
                        break;
                }

                if (clsid == Guid.Empty || iid == Guid.Empty)
                    throw new Exception($"Не найдено: {coclassName} или {interfaceName}");

                IntPtr pDevice;

                hr = CoCreateInstance(ref clsid, IntPtr.Zero, CLSCTX_ALL, ref iid, out pDevice);
                if (hr != 0)
                    throw new COMException($"CoCreateInstance failed: 0x{hr:X8}", hr);

                IOsc casted = (IOsc)Marshal.GetTypedObjectForIUnknown(pDevice, typeof(IOsc));

                return;
            }
            finally
            {
                Marshal.ReleaseComObject(typeLib);
            }
        }
        public void Dispose()
        {
            DllInvoke.FreeLibrary(hModule);
            hModule = IntPtr.Zero;
        }
    }
}
