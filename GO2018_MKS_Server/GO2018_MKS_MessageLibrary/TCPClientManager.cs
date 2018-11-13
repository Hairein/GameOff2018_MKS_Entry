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
        public TcpClient tcpClient;

        StringBuilder incomingBuffer = new StringBuilder();

        public void ConnectToTcpServer()
        {
            tcpClient = new TcpClient(tcpServerAddress, tcpServerPort);
            tcpClient.LingerState = new LingerOption(false, 0);
            tcpClient.NoDelay = true;
        }

        public void ConnectToTcpServer(string serverAddress, int serverPort)
        {
            tcpServerAddress = serverAddress;
            tcpServerPort = serverPort;

            tcpClient = new TcpClient(tcpServerAddress, tcpServerPort);
            tcpClient.LingerState = new LingerOption(false, 0);
            tcpClient.NoDelay = true;
        }

        public void DisconnectFromTcpServer()
        {
            if (tcpClient.Connected)
            {
                NetworkStream stream = tcpClient.GetStream();
                stream.Close();

                tcpClient.Close();
            }
        }

        private bool CheckServerConnected()
        {
            return tcpClient.Client.Connected;
        }

        public void SendJsonMessage(string message)
        {
            if (!tcpClient.Connected)
            {
                return;
            }

            NetworkStream stream = tcpClient.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void SendMessageObject(object messageObject)
        {
            if (!tcpClient.Connected)
            {
                return;
            }

            string message = JsonConvert.SerializeObject(messageObject);

            NetworkStream stream = tcpClient.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public string ReceiveMessage()
        {
            string nextMessage = string.Empty;

            if (!tcpClient.Connected)
            {
                return nextMessage;
            }

            int availableClientData = tcpClient.Available;
            if (availableClientData > 0)
            {
                NetworkStream stream = tcpClient.GetStream();

                byte[] readBuffer = new byte[availableClientData];
                int bytesRead = stream.Read(readBuffer, 0, availableClientData);

                string partMessageText = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);

                incomingBuffer.Append(partMessageText);
            }

            string readMessage = incomingBuffer.ToString();
            if (!string.IsNullOrEmpty(readMessage))
            {
                int closingBracketIndex = readMessage.IndexOf('}');
                if (closingBracketIndex >= 0)
                {
                    nextMessage = readMessage.Substring(0, closingBracketIndex + 1);

                    int remainingLength = readMessage.Length - nextMessage.Length;
                    string remainingMessage = readMessage.Substring(closingBracketIndex + 1, remainingLength);

                    incomingBuffer = new StringBuilder(remainingMessage);
                }
            }

            return nextMessage;
        }
    }
}
