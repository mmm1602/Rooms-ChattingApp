using System;
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


        public Server()
        {
            _client = new TcpClient();
        }
        public void ConnectToServer(string Username)
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect("192.168.1.105", 56853);
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
                            Console.WriteLine("Of Course");
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
    }
}
