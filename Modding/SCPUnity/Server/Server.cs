using System;
using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace Server
{
    class Server : UdpServer
    {
        public IPAddress address;
        public int port;
        public static Server Instance;

        public Server(IPAddress aAddress, int aPort) : base(aAddress, aPort)
        {
            port = aPort;
            address = aAddress;
            Instance = this;
        }

        protected override void OnStarted()
        {
            base.OnStarted();

            ReceiveAsync();
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            base.OnReceived(endpoint, buffer, offset, size);

            // echo data to all other clients
            // parse the buffer to handle expected values then send 
        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            base.OnSent(endpoint, sent);

            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            base.OnError(error);


        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            UdpServer server = new Server(IPAddress.Loopback, 10293);

            Console.WriteLine($"Starting server : {server.Address}:{server.Port}");

            if (server.Start())
            {
                Console.WriteLine($"Started server!");


            }
        }
    }
}

