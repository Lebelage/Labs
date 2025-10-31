using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Core.ScriptInterpreter.Models
{
    public class ExperimentResults
    {
        public int Cuvette { get; set; }
        public int Resistor { get; set; }
        public int DataSetSize { get; set; }
        public int HarmonicsNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public List<double> Data { get; set; } = new List<double>();

    }
}
