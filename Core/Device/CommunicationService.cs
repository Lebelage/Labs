using Lab1.Core.Device.Interfaces;
using Lab1.Infrastructure.enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1.Core.Device
{
    internal class CommunicationService : IConnection
    {
        public event EventHandler<bool>? ConnectionChanged;
        public event EventHandler<bool>? DataReceived;
        public event EventHandler<IEnumerable<byte>>? ResponseReceived;


        byte[] Received = new byte[1024];
        private bool IsOpenned = false;
        private SerialPort ConnectionSource;


        public void CreateConnectionSource(string portName, int baudRate = 115200, Parity parity = Parity.None, StopBits stopBits = StopBits.One, byte dataBits = 5)
        {
            ConnectionSource = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            ConnectionSource.ReadTimeout = 1000;
            ConnectionSource.WriteTimeout = 200;
        }

        public async Task<bool> CloseConnectionAsync()
        {
            try
            {
                await Task.Run(() =>
                {

                    ConnectionSource?.Close();
                    ConnectionSource?.Dispose();
                    IsOpenned = false;

                    ConnectionChanged?.Invoke(this, false);

                });
            }
            catch
            {
                IsOpenned = true;
                return false;
            }
            return true;
        }

        public async Task<bool> OpenConnectionAsync()
        {
            if (ConnectionSource.IsOpen)
                return false;

            IsOpenned = ConnectionSource.IsOpen;

            try
            {
                await Task.Run(() =>
                {
                    Dispose();
                    ConnectionSource?.Open();
                    ConnectionChanged?.Invoke(this, true);
                    ConnectionSource.DataReceived += OnDataReceived;

                });
            }
            catch
            {
                IsOpenned = false;
                ConnectionChanged?.Invoke(this, false);
                return IsOpenned;
            }

            return true;
        }

        public async Task<bool> SetCuvetteAndResistorAsync(byte cuvette, byte resistorIndex)
        {
            try
            {
                RequestEncoder encoder = new RequestEncoder();
                var request = encoder.SetCuvetteNumberAndResistorIndex(cuvette, resistorIndex);
                await Task.Run(() => SendRequestAsync(request, SendTypes.Set));
                return true;
            }
            catch
            {
                return false;
            }

        }

        private async Task SendRequestAsync(string request, SendTypes sendType)
        {
            if (!ConnectionSource.IsOpen)
            {
                throw new InvalidOperationException("Connection is not open.");
            }

            try
            {
                byte[] data = Encoding.ASCII.GetBytes(request);

                ConnectionSource.Write(data, 0, data.Length);

                if (sendType == SendTypes.Get)
                {
                    byte[] received = new byte[1024];
                    int bytesRead = await Task.Run(() => ConnectionSource.Read(received, 0, received.Length));

                    if (bytesRead > 0)
                    {
                        byte[] response = new byte[bytesRead];
                        Array.Copy(received, 0, response, 0, bytesRead);
                        ResponseReceived?.Invoke(this, response);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("Failed to send request or receive response.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during the request.", ex);
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        private void Dispose()
        {
            ConnectionSource?.Dispose();
        }
    }
}
