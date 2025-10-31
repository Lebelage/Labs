using Lab1.Core.Libs;
using Lab1.Services.Interface;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;


namespace Lab1.Services
{
    internal class DllWorkerService : IDllWorkerService, IDisposable
    {
        private IntPtr hComponent = IntPtr.Zero;
        private delegate int FunSummaDelegate(int x, int y);
        private FunSummaDelegate fun_summa;

        public void CallSum(string dllPath, string funName, int x, int y)
        {           
        }   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
