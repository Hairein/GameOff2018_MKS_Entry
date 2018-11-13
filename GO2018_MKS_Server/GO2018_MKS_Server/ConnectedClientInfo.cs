using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_Server
{
    public class ConnectedClientInfo : IDisposable
    {
        private TcpClient tcpClient;

        private List<GenericMessage> messageStack = new List<GenericMessage>();

        private StringBuilder incomingBuffer = new StringBuilder();

        private string platformId = string.Empty;
        private string playerHandle = string.Empty;

        public bool IsConnected
        {
            get
            {
                if (tcpClient != null)
                {
                    return tcpClient.Client.Connected;
                }

                return false;
            }
        }

        private bool isCreatingSession = false;
        private CreateSessionMessage createSessionMessage = null;

        public ConnectedClientInfo(TcpClient newTcpClient)
        {
            tcpClient = newTcpClient;
        }

        public void Dispose()
        {
            if (tcpClient.Client.Connected)
            {
                NetworkStream stream = tcpClient.GetStream();
                stream.Close();

                tcpClient.Close();
            }
        }

        public TcpClient GetTcpClient()
        {
            return tcpClient;
        }

        public void AddMessage(GenericMessage newMessage)
        {
            messageStack.Add(newMessage);
        }

        public GenericMessage[] GetPendingMessages()
        {
            GenericMessage[] pendingMessages = messageStack.ToArray();
            messageStack.Clear();

            return pendingMessages;
        }

        public void SendPendingMessages()
        {
            if (!tcpClient.Client.Connected)
            {
                return;
            }

            foreach (GenericMessage message in messageStack)
            {
                string messageText = JsonConvert.SerializeObject(message);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(messageText);

                NetworkStream stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
            }

            messageStack.Clear();
        }

        public string GetNextIncomingMessage()
        {
           string nextMessage = string.Empty;

           if(tcpClient == null || !tcpClient.Client.Connected)
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

        public void GetPlayerCredentials(out string thePlatformId, out string thePlayerHandle)
        {
            thePlatformId = platformId;
            thePlayerHandle = playerHandle;
        }

        public void SetPlayerCredentials(string newPlatformId, string newPlayerHandle)
        {
            platformId = newPlatformId;
            playerHandle = newPlayerHandle;
        }

        public bool SetCreateSessionState(CreateSessionMessage newCreateSessionMessage)
        {
            if(isCreatingSession || createSessionMessage == null)
            {
                return false;
            }

            createSessionMessage = newCreateSessionMessage;

            return true;
        }

        public void ClearCreateSessionState()
        {
            isCreatingSession = false;
            createSessionMessage = null;
        }
    }
}
