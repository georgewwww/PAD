using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Sender
{
    class SenderSocket
    {
        private Socket _socket;
        public bool isConnected;

        public SenderSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectedCallback, null);
            Thread.Sleep(2000);
        }

        public void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: While sending data to broker. {e.Message}");
            }
        }

        private void ConnectedCallback(IAsyncResult result)
        {
            if (_socket.Connected)
            {
                Console.WriteLine("Sender connected to Broker.");
            } else
            {
                Console.WriteLine("Error: Sender not connected to Broker.");
            }

            isConnected = _socket.Connected;
        }
    }
}
