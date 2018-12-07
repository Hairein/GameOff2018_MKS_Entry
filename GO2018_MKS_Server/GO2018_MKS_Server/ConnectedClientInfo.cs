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

        public bool IsCreatingSession = false;
        public CreateSessionMessage CreateSessionMessage = null;

        public bool IsInActiveSession = false;
        public ActiveSessionInfo ActiveSession = null;

        public DateTime LastMessageTimestamp = DateTime.UtcNow;

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

                try
                {
                    NetworkStream stream = tcpClient.GetStream();
                    stream.Write(data, 0, data.Length);
                }
                catch
                {
                }
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
            if (IsCreatingSession)
            {
                return false;
            }

            IsCreatingSession = true;
            CreateSessionMessage = newCreateSessionMessage;

            return true;
        }

        public void ClearCreateSessionState()
        {
            IsCreatingSession = false;
            CreateSessionMessage = null;
        }

        public bool SetActiveSessionState(ActiveSessionInfo newActiveSession)
        {
            if (IsInActiveSession)
            {
                return false;
            }

            IsInActiveSession = true;
            ActiveSession = newActiveSession;

            return true;
        }

        public void ClearActiveSessionState()
        {
            IsInActiveSession = false;
            ActiveSession = null;
        }
    }
}
