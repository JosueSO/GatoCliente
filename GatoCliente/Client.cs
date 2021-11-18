using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GatoCliente
{
    public class Client
    {
        Socket sClient;
        IPEndPoint endPoint;

        public Client(string ip, int port)
        {
            IPHostEntry host = Dns.GetHostEntry(ip);
            IPAddress ipAdd = host.AddressList[0];
            endPoint = new IPEndPoint(ipAdd, port);

            sClient = new Socket(ipAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            sClient.Connect(endPoint);
        }

        public void Send(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            sClient.Send(buffer);
        }

        public string Receive()
        {
            byte[] buffer = new byte[1024];

            sClient.Receive(buffer);

            string message = Encoding.UTF8.GetString(buffer);

            int pos = message.IndexOf('\0');

            message = message.Substring(0, pos);

            return message;
        }
    }
}
