using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;

class Server : UdpServer
{
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
        // if server received from itself, just continue receiving
        if (endpoint != Endpoint)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            int.TryParse(message[0].ToString(), out var typeInt);
            EventType type = (EventType)typeInt;

            // idk why it decides to send big endian for a single message then little endian
            //if (BitConverter.IsLittleEndian)
            //{
            //    Array.Reverse(buffer);
            //}

            //EventType type = (EventType)BitConverter.ToInt32(typeInt, 0);


            switch (type)
            {
                case EventType.Join:

                    Client client = new Client();
                    char[] chars = message.ToCharArray();


                    for (uint i = 1; i < size; i++)
                    {
                        if (chars[i] == '\n' || chars[i] == '\r')
                            continue;

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
                    EventColors.FormatEventTypeOutput(type);
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
                case EventType.Acknowledge:
                    Console.WriteLine("ACK received");
                    Send(endpoint, $"{(int)EventType.Acknowledge}");
                    Console.WriteLine($"ACK sent to endpoint {endpoint}");
                    break;
                case EventType.JoinLobby:
                case EventType.LeaveLobby:
                case EventType.KickPlayer:
                case EventType.MessageSent:
                case EventType.MessageReceived:
                default: break;
            }

            //Console.WriteLine($"Incoming: {Encoding.UTF8.GetString(buffer, (int)offset, (int)size)}");

            foreach (Client client in ClientList)
            {
                if (endpoint != client.endpoint)
                {
                    SendAsync(client.endpoint, buffer, offset, size);
                }
            }
        }

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
