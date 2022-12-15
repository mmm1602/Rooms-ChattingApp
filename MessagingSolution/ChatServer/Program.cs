using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ChatServer.Net.IO;

namespace ChatServer
{
    class Program
    {
        static List<Client> _Users;
        static TcpListener _Listener;
        static void Main(string[] args)
        {
            _Users = new List<Client>();
            _Listener = new TcpListener(IPAddress.Parse("192.168.0.104"), 8000);
            _Listener.Start();

            while (true)
            {
                var Client = new Client(_Listener.AcceptTcpClient());
                _Users.Add(Client);

                /*Inform every client*/
                LogConnection();
            }
        }

        static void LogConnection()
        {
            foreach (var user in _Users)
            {
                foreach (var USR in _Users)
                {
                    var packetToSend = new PacketBuilder();

                    packetToSend.WriteOpCode(1);
                    packetToSend.WriteMsg(USR.Username);
                    packetToSend.WriteMsg(USR.UID.ToString());
                    user.ClientSocket.Client.Send(packetToSend.GetPacketBytes());
                }
            }
        }

        public static void SendMessage(string message, string UID)
        {
            var TargetClient = _Users.Find(x => x.UID.ToString() == UID);
            var msgPacket = new PacketBuilder();
            msgPacket.WriteOpCode(5);
            msgPacket.WriteMsg(message);
            TargetClient.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }

        public static void LogDisconnect(string uid)
        {
            var disconnectedUser = _Users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _Users.Remove(disconnectedUser);

            if(_Users.Count > 1)
            {
                foreach (var user in _Users)
                {
                    var logPacket = new PacketBuilder();
                    logPacket.WriteOpCode(10);
                    logPacket.WriteMsg(uid);
                    user.ClientSocket.Client.Send(logPacket.GetPacketBytes());
                    SendMessage($"[{DateTime.Now}] {disconnectedUser.Username} disconnected");

                }
            }
        }
    }
}
