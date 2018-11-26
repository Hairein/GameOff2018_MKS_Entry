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
        private string tcpServerAddress = "5.175.24.40";
        private int tcpServerPort = 13000;
        public TcpClient tcpClient = null;

        StringBuilder incomingBuffer = new StringBuilder();

        public void ConnectToTcpServer()
        {
        }

        public void ConnectToTcpServer(string serverAddress, int serverPort)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                DisconnectFromTcpServer();
            }

            tcpServerAddress = serverAddress;
            tcpServerPort = serverPort;

            tcpClient = new TcpClient(tcpServerAddress, tcpServerPort);
            tcpClient.LingerState = new LingerOption(false, 0);
            tcpClient.NoDelay = true;
        }

        public void DisconnectFromTcpServer()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                return;
            }

            NetworkStream stream = tcpClient.GetStream();
            stream.Close();

            tcpClient.Close();
        }

        public bool IsConnected()
        {
            if (tcpClient == null)
            {
                return false;
            }

            return tcpClient.Client.Connected;
        }

        public void SendJsonMessage(string message)
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                return;
            }

            NetworkStream stream = tcpClient.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void SendMessageObject(object messageObject)
        {
            if (tcpClient == null || !tcpClient.Connected)
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

            if (tcpClient == null || !tcpClient.Connected)
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
                Console.WriteLine("cutting up message: " + readMessage);

                int bracketCounter = 0;
                for (int charPos = 0; charPos < readMessage.Length; charPos++)
                {
                    char charAtPos = readMessage[charPos];

                    if (charAtPos == '{')
                    {
                        bracketCounter++;
                    }
                    else if (charAtPos == '}')
                    {
                        bracketCounter--;

                        if (bracketCounter == 0)
                        {
                            nextMessage = readMessage.Substring(0, charPos + 1);

                            string remainingMessage = string.Empty;
                            int remainingLength = readMessage.Length - nextMessage.Length;
                            if (remainingLength > 0)
                            {
                                remainingMessage = readMessage.Substring(charPos + 1, remainingLength);
                            }

                            incomingBuffer = new StringBuilder(remainingMessage);
                            break;
                        }
                    }
                }
            }

            return nextMessage;
        }
    }
}
