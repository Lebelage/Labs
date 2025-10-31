using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Services.Interface
{
    internal interface IDllWorkerService
    {
        void CallSum(string dllPath, string funName, int x, int y);
    }
}
