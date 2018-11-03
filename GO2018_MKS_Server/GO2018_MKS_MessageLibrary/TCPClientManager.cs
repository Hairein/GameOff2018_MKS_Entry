using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class TCPClientManager
    {
        private string tcpServerAddress = "localhost";
        private int tcpServerPort = 13000;
        public TcpClient client;

        public void ConnectToTcpServer()
        {
            client = new TcpClient(tcpServerAddress, tcpServerPort);
        }

        public void DisconnectFromTcpServer()
        {
            if (client.Connected)
            {
                client.Close();
            }
        }

        public void SendMessage(string message)
        {
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public bool ReceiveMessage(out string message)
        {
            message = string.Empty;

            int availableClientData = client.Available;
            if (availableClientData == 0)
            {
                return false;
            }

            NetworkStream stream = client.GetStream();

            byte[] readBuffer = new byte[availableClientData];
            int bytesRead = stream.Read(readBuffer, 0, availableClientData);

            message = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
            return true;
        }
    }
}
