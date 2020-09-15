using Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Broker
{
    class BrokerSocket
    {
        private Socket _socket;
        private const int CONNECTIONS_LIMIT = 8;

        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string ip, int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _socket.Listen(CONNECTIONS_LIMIT);
            Accept();
        }

        private void Accept()
        {
            _socket.BeginAccept(AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult result)
        {
            ConnectionInfo connection = new ConnectionInfo();

            try
            {
                connection.Socket = _socket.EndAccept(result);
                connection.Address = connection.Socket.RemoteEndPoint.ToString();

                Console.WriteLine($"A connection was established by address {connection.Address}");

                connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                    SocketFlags.None, ReceiveCallback, connection);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't accept. {e.Message}");
            }
            finally
            {
                Accept();
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = result.AsyncState as ConnectionInfo;

            try
            {
                Socket senderSocket = connection.Socket;
                SocketError response;
                int buffSize = senderSocket.EndReceive(result, out response);

                if (response == SocketError.Success)
                {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connection.Data, payload, payload.Length);

                    PayloadHandler.Handle(payload, connection);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't receive data: {e.Message}");
            }
            finally
            {
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                        SocketFlags.None, ReceiveCallback, connection);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    var address = connection.Socket.RemoteEndPoint.ToString();

                    ConnectionsStorage.Remove(address);
                    connection.Socket.Close();
                }
            }
        }
    }
}
