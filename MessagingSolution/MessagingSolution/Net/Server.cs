using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatClient.Net.IO;

namespace ChatClient.Net
{
    class Server
    {
        readonly TcpClient _client;
        public PacketReader PackReader;

        public event Action ConnectedEvent;
        public event Action MsgRecievedEvent;
        public event Action DisconnectedEvent;

        private string ip_address;

        public string Ip_address
        {
            get { return ip_address; }
            set { ip_address = value; }
        }



        public Server()
        {
            _client = new TcpClient();
            Ip_address = GetLocalIPAddress();

        }
        public void ConnectToServer(string Username)
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect(GetLocalIPAddress(), 8000);
                    PackReader = new PacketReader(_client.GetStream());

                    if (!string.IsNullOrEmpty(Username))
                    {
                        var connectPacket = new PacketBuilder();
                        connectPacket.WriteOpCode(0);
                        connectPacket.WriteMessage(Username);
                        _client.Client.Send(connectPacket.GetPacketBytes());
                    }
                    ReadPackets();
                }
                catch (Exception)
                {
                    Console.WriteLine("Server was not found");
                    return;
                }
                
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opCode = PackReader.ReadByte();

                    switch (opCode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke(); 
                            break;

                        case 5:
                            MsgRecievedEvent?.Invoke();
                            break;

                        case 10:
                            DisconnectedEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine($"OpCode [{opCode}] is not defined");
                            break;
                    }
                    
                }
            }
            );
        }

        public void SendMsgToServer(string message)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WriteOpCode(5);
            msgPacket.WriteMessage(message);
            _client.Client.Send(msgPacket.GetPacketBytes());
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
