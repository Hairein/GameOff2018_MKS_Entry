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

        StringBuilder incomingBuffer = new StringBuilder();

        public void ConnectToTcpServer()
        {
            client = new TcpClient(tcpServerAddress, tcpServerPort);
            client.LingerState = new LingerOption(false, 0);
            client.NoDelay = true;
        }

        public void DisconnectFromTcpServer()
        {
            if (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                stream.Close();

                client.Close();
            }
        }

        private bool VerifyServerConnection()
        {
            return client.Client.Connected;
        }

        public void SendMessage(string message)
        {
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        /*        
        public bool ReceiveMessage(out string message)
        {
            message = string.Empty;

            List<string> listOfMessages = new List<string>();

            int availableClientData = client.Available;
            if (availableClientData != 0)
            {
                NetworkStream stream = client.GetStream();

                byte[] readBuffer = new byte[availableClientData];
                int bytesRead = stream.Read(readBuffer, 0, availableClientData);

                incomingBuffer.Append(Encoding.UTF8.GetString(readBuffer, 0, bytesRead));
            }

            if(incomingBuffer.Length == 0)
            {
                return false;
            }

            string cutableMessages = incomingBuffer.ToString();
            int bracketCloseIndex = cutableMessages.IndexOf('}');
            if (bracketCloseIndex >= 0) 
            {
                message = cutableMessages.Substring(0, bracketCloseIndex + 1);

                string remainingPart = cutableMessages.Substring(bracketCloseIndex + 1, cutableMessages.Length - 1);
                incomingBuffer = new StringBuilder(remainingPart);

                return true;
            }

            return false;
        }
        */
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
