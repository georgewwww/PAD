using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Subscriber
{
    public class SubscriberSocket
    {
        private readonly Socket _socket;
        private readonly string _bankName;

        public SubscriberSocket(string bankName)
        {
            _bankName = bankName;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectedCallback, null);
            Console.WriteLine("Waiting for a connection");
        }

        private void ConnectedCallback(IAsyncResult result)
        {
            if (_socket.Connected)
            {
                Console.WriteLine("Subscriber connected to broker.");
                Subscribe();
                StartReveive();
            } else
            {
                Console.WriteLine("Error: Subscriber could not connect to broker.");
            }
        }

        private void Subscribe()
        {
            var data = Encoding.UTF8.GetBytes("subscribe#" + _bankName);
            Send(data);
        }

        private void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Could not send data: {e.Message}");
            }
        }

        private void StartReveive()
        {
            var connection = new ConnectionInfo { Socket = _socket };

            _socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                SocketFlags.None, ReceiveCallback, connection);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = result.AsyncState as ConnectionInfo;

            try
            {
                int buffSize = _socket.EndReceive(result, out SocketError response);

                if (response == SocketError.Success)
                {
                    byte[] payloadBytes = new byte[buffSize];
                    Array.Copy(connection.Data, payloadBytes, payloadBytes.Length);

                    PayloadHandler.Handle(payloadBytes);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: Can't receive data from broker. {e.Message}");
            }
            finally
            {
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                        SocketFlags.None, ReceiveCallback, connection);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    connection.Socket.Close();
                }
            }
        }
    }
}
