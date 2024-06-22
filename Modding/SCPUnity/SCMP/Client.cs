using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UdpClient = NetCoreServer.UdpClient;
using UnityEngine;
using SCMP.Patches;

namespace SCMP
{
    internal class Client : UdpClient
    {
        private bool _stop;

        public Client(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            Disconnect();
            while (IsConnected)
            {
                Thread.Yield();
            }
        }

        protected override void OnConnected()
        {
            Debug.Log($"Echo UDP client connected a new session with Id {Id}");

            // Start receive datagrams
            ReceiveAsync();
        }

        protected override void OnDisconnected()
        {
            Debug.Log($"Echo UDP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                Connect();
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            Debug.Log("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));

            // Continue receive datagrams
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Debug.Log($"Echo UDP client caught an error with code {error}");
        }

        class Program
        {
            static void Main(string[] args)
            {
                bool disconnect = false;

                // UDP server address
                string address = MainMenuPatch.IpInputField.text;
                //if (args.Length > 0)
                //    address = args[0];

                // UDP server port
                int port = int.Parse(MainMenuPatch.PortInputField.text);
                //if (args.Length > 1)
                //    port = int.Parse(args[1]);

                Debug.Log($"UDP server address: {address}");
                Debug.Log($"UDP server port: {port}");

                // Create a new UDP chat client
                var client = new Client(address, port);

                // Connect the client
                Console.Write("Client connecting...");
                client.Connect();
                Debug.Log("Done!");

                if (disconnect == true)
                {
                    // Disconnect the client
                    Console.Write("Client disconnecting...");
                    client.DisconnectAndStop();
                    Debug.Log("Done!");
                }
            }
        }
    }
}
