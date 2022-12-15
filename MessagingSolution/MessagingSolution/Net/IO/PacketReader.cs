using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketReader : BinaryReader
    {
        private readonly NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMsg()
        {
            byte[] msgBuffer;
            var lenght = ReadInt32();
            msgBuffer = new byte[lenght];
            _ns.Read(msgBuffer, 0, lenght);

            var msg = Encoding.ASCII.GetString(msgBuffer);

            return msg;
        }
    }
}
