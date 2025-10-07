using Lab1.Infrastructure.enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.Device.Interfaces
{
    internal interface IConnection
    {
        public event EventHandler<bool>? ConnectionChanged;
        public event EventHandler<bool>? DataReceived;
        public event EventHandler<IEnumerable<byte>>? ResponseReceived;
        public void CreateConnectionSource(string portName, int baudRate = 115200, Parity parity = Parity.None, StopBits stopBits = StopBits.One, byte dataBits = 5);
        public Task<bool> OpenConnectionAsync();
        public Task<bool> CloseConnectionAsync();
        public Task<bool> SetCuvetteAndResistorAsync(byte cuvette, byte resistorIndex);
    }
}
