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
            

            switch (type)
            {
                case EventType.Join:

                    Client client = new Client();
                    char[] chars = message.ToCharArray();


                    for (uint i = 1; i < size; i++)
                    {
                        if (chars[i] == '\0')
                            break;

                        if (chars[i] == '\n' || chars[i] == '\r')
                            continue;

                        client.username += chars[i];
                    }

                    int id = GenerateId();
                    while (idList.Contains(id))
                    {
                        id = GenerateId();
                    }
                    idList.Add(id);

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
                    string lobbyInfoToSend =
                        $@"{Directory.GetCurrentDirectory()}\bin\lobbyinfo.txt";
                    if (new DirectoryInfo(Directory.GetCurrentDirectory()).Name != "SCMP")
                        lobbyInfoToSend = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\bin\lobbyinfo.txt";

                    Utilities.WaitForFile(lobbyInfoToSend);
                    string lobbyInfo = Utilities.ReadFileBytes(lobbyInfoToSend);

                    // SPLIT THE STRING WITH A COMMA DELIMITER
                    string[] playerNames = lobbyInfo.Split(',');
                    char[] badChars = { '\r', '\n', ','};
                    lobbyInfo = $"{playerNames.Length}\0";
                    foreach (string name in playerNames)
                    {
                        string temp = name;
                        foreach (char c in badChars)
                        {
                            while (temp.Contains(c))
                            {
                                temp = temp.Remove(temp.IndexOf(c), 1);
                            }
                        }

                        if(temp != "")
                            lobbyInfo += $"{temp},";
                    }

                    // don't send trailing comma
                    lobbyInfo = lobbyInfo.Remove(lobbyInfo.Length - 1, 1);

                    Console.WriteLine(lobbyInfo);
                    Send(endpoint, EventType.Acknowledge);
                    Send(endpoint, EventType.Acknowledge, lobbyInfo);
                    break;
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

    private long Send(EndPoint endpoint, EventType eventType, string value = "")
    {
        return Send(endpoint, $"{(int)eventType}{value}");
    }

    private long Send(EndPoint endpoint, EventType eventType, byte[] value)
    {
        return Send(endpoint, $"{(int)eventType}{value}");
    }
}
