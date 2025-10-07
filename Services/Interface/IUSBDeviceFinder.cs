using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Services.Interface
{
    interface IUSBDeviceFinder
    {
        public List<string> GetMouses();
        public List<string> GetKeyboards();
        public List<string> GetHIDDevices();
        public bool FindDevices();
    }
}
