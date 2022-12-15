using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ChatServer.Net.IO;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        readonly PacketReader _packReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _packReader = new PacketReader(ClientSocket.GetStream());

            var opCode = _packReader.ReadByte();

            Username = _packReader.ReadMsg();

            Console.WriteLine($"[{DateTime.Now}] A client has connected with the username: {Username}");

            Task.Run(() => Process());

        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opCode = _packReader.ReadByte();
                    switch (opCode)
                    {
                        case 5:
                            var msg = _packReader.ReadMsg();
                            Console.WriteLine($"[{DateTime.Now}] Client with username: {Username} has sent the message : {msg}");
                            Program.SendMessage($"[{DateTime.Now}] {Username}: {msg}", );
                            break;

                        default:
                            Console.WriteLine($"OpCode [{opCode}] is not defined");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{DateTime.Now}] [{UID}]: Disconnected!");
                    Program.LogDisconnect(UID.ToString());
                    ClientSocket.Close();
                }
            }
        }
    }
}
