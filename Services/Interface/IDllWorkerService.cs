using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Services.Interface
{
    internal interface IDllWorkerService
    {
        bool DllLoad(string filePath);
        bool FunctionSelect(string funcName);
        bool CallSum(int x, int y, out int result);

        void InitOcs(string tlbPath = "C:\\Users\\neotro\\source\\repos\\OscDll\\OscB322.tlb", string coclassName = "OscDeviceB322", string interfaceName = "IOsc");
    }
}
