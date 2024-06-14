using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class UdpClient
    {
        private readonly IPEndPoint _serverEndPoint;
        private readonly Socket _clientSocket;
        private readonly EndPoint serverEndPoint;
        public UdpClient(string serverIpAddress, int serverPort)
        {
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public async Task<string> SendAndReceiveAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes("client" + message);

            await _clientSocket.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, _serverEndPoint);
            Console.WriteLine("Sent: " + message);

            byte[] buffer = new byte[1024];
            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

            var result = await _clientSocket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, serverEndPoint);
            string response = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);
            return response;
        }
        public async Task SendAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes("client" + message);

            await _clientSocket.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, _serverEndPoint);
            Console.WriteLine("Sent: " + message);
        }
        public async Task<string> ReceiveAsync()
        {
            byte[] buffer = new byte[1024];
            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

            var result = await _clientSocket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, serverEndPoint);
            string response = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);
            if (response.StartsWith("client"))
                return "";
                
            return response;
        }

    }
}
