﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;

class Server : UdpServer
{
    public enum EventType
    {
        Join = 0,
        Leave = 1,
        UpdateRotation = 2,
        UpdateLocation = 3,
        StartGame = 4,
        EndGame = 5
    }

    public struct Client
    {
        public string username;
        public int id;
        public EndPoint endpoint;
    }

    public static Server Instance;
    public IPAddress address;
    public int port;
    public List<Client> ClientList;

    private List<int> idList;

    public Server(IPAddress aAddress, int aPort) : base(aAddress, aPort)
    {
        port = aPort;
        address = aAddress;
        Instance = this;
        ClientList = new List<Client>();
        idList = new List<int>();
    }

    protected override void OnStarted()
    {
        Console.WriteLine($"Started server");

        ReceiveAsync();
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        string message = Encoding.Default.GetString(buffer);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(buffer);

        EventType type = (EventType)BitConverter.ToInt32(buffer, 0);

        switch (type)
        {
            case EventType.Join:
                Client client = new Client();
                char[] chars = message.ToCharArray();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{type}Event");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("] ");
                Console.ResetColor();

                for (uint i = 1; i < size; i++)
                {
                    if (chars[i] == '\0')
                        break;

                    client.username += chars[i];
                }

                int id = GenerateId();
                while (idList.Contains(id))
                {
                    id = GenerateId();
                    idList.Add(id);
                }

                client.id = id;
                client.endpoint = endpoint;
                ClientList.Add(client);
                Console.WriteLine($"{client.username} joined with id {client.id}");
                break;
            case EventType.Leave:
                break;
            case EventType.UpdateRotation:
                break;
            case EventType.UpdateLocation:
                break;
            case EventType.StartGame:
                break;
            case EventType.EndGame:
                break;
            default: break;
        }

        if (endpoint != Endpoint)
        {
            Console.WriteLine($"Incoming: {Encoding.UTF8.GetString(buffer, (int)offset, (int)size)}");

            foreach (Client client in ClientList)
            {
                if (endpoint != client.endpoint)
                {
                   // SendAsync(client.endpoint, buffer, offset, size);
                    Console.WriteLine(SendAsync(client.endpoint, buffer, offset, size));
                }
            }
        }



        ////base.OnReceived(endpoint, buffer, offset, size);
        //SocketAddress socketAddress = endpoint.Serialize();
        //EventType eventType = (EventType)BitConverter.ToInt32(buffer, 0);

        //Console.WriteLine(eventType.ToString());
        //switch (eventType)
        //{
        //    case EventType.Join:
        //        Client client = new Client();

        //        for (uint i = 1; i < size; i++)
        //        {
        //            if (buffer[i] != '\0')
        //            {
        //                client.username += buffer[i];
        //            }
        //        }

        //        int id = GenerateId();
        //        while (idList.Contains(id))
        //        {
        //            id = GenerateId();
        //        }

        //        client.id = id;
        //        client.endpoint = endpoint;
        //        ClientList.Add(client);
        //        Console.Write($" {client.username} joined with id {client.id}");
        //        break;
        //    case EventType.Leave:
        //        break;
        //    case EventType.UpdateRotation:
        //        break;
        //    case EventType.UpdateLocation:
        //        break;
        //    case EventType.StartGame:
        //        break;
        //    case EventType.EndGame:
        //        break;
        //    default: break;
        //}
        ReceiveAsync();
    }

    protected override void OnSent(EndPoint endpoint, long sent)
    {
        //base.OnSent(endpoint, sent);

        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Server caught an error with code {error}");
    }

    private int GenerateId()
    {
        return new Random().Next(0, int.MaxValue / 25);
    }
}
