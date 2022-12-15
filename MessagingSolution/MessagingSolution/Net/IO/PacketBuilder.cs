using System;
using System.IO;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketBuilder
    {
        readonly MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opCode)
        {
            _ms.WriteByte(opCode);
        }

        public void WriteMessage(string msg)
        {
            var msgLenght = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLenght));
            _ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}
