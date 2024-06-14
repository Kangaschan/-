using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            UdpServer tcpServer = new UdpServer("127.0.0.1", 5000);
            await tcpServer.StartAsync();
            do
            {
                         
            }
            while (!tcpServer.stop);
            Console.WriteLine("End of game");
        }
    }

}
