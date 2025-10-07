using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.Device
{
    class RequestEncoder
    {
        public string SetCuvetteNumberAndResistorIndex(byte cuvette, byte index) => BuildWriteRequest(new byte[] { cuvette, index });

        //private string BuildReadRequest() { return null; }
        private string BuildWriteRequest(IEnumerable<byte> parameters) 
        {            
            return Encoding.ASCII.GetString(parameters.ToArray());;
        }
    }
}
