using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GO2018_MKS_Server
{
    class Program
    {
        bool runFlag = true;

        List<ConnectedClientInfo> connectedClients = new List<ConnectedClientInfo>();

        static void Main(string[] args)
        {
            int serverListenPort = 13000;

            if(args.Length == 1)
            {
                string portText = args[0];
                int cmdPort = 0;
                if (int.TryParse(portText, out cmdPort))
                {
                    serverListenPort = cmdPort;
                }
            }

            Program program = new Program();
            program.Run(serverListenPort);
        }

        public void Run(int port)
        {
            Console.WriteLine("GO2018 MKS Server");

            Thread tcpListenerThread = new Thread(() => TcpHandlerThreadProc(port));
            tcpListenerThread.Start();

            PrintHelpMessage();

            while (runFlag)
            {
                string inputLine = Console.ReadLine();
                switch (inputLine.Trim())
                {
                    case "quit":
                    case "exit":
                        {
                            runFlag = false;
                        }
                        break;
                    case "help":
                        {
                            PrintHelpMessage();
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown command: " + inputLine);
                        break;
                }
            }

            tcpListenerThread.Join();
        }

        void PrintHelpMessage()
        {
            Console.WriteLine("Commands (help, quit, exit)");
        }

        private void TcpHandlerThreadProc(int port)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            while (runFlag)
            {
                AcceptClients(server);
                HandleClientMessages();
                WriteClientMessages();
                ClearLostClients();

                // TEMP Break for a bit
                Thread.Sleep(200);
            }

            server.Stop();

            foreach (ConnectedClientInfo client in connectedClients)
            {
                client.Dispose();
            }
            connectedClients.Clear();
        }

        // Connect players
        private void AcceptClients(TcpListener server)
        {
            while (server.Pending())
            {
                TcpClient tcpClient = server.AcceptTcpClient();
                Console.WriteLine("Connected: " + tcpClient.Client.Handle.ToString());

                tcpClient.LingerState = new LingerOption(false, 0);
                tcpClient.NoDelay = true;

                ConnectedClientInfo newClient = new ConnectedClientInfo(tcpClient);
                connectedClients.Add(newClient);

                WelcomeMessage welcomeMessage = new WelcomeMessage();
                newClient.AddMessage(welcomeMessage);
            }
        }

        // Read player messages
        private void HandleClientMessages()
        {
            foreach (ConnectedClientInfo client in connectedClients)
            {
                if (!client.IsConnected)
                {
                    continue;
                }

                while (true)
                {
                    string inputMessage = client.GetNextIncomingMessage();
                    if (string.IsNullOrEmpty(inputMessage))
                    {
                        break;
                    }

                    HandleIncomingMessage(client, inputMessage);                                        
                }            
            }
        }

        private void HandleIncomingMessage(ConnectedClientInfo client, string nextMessageText)
        {
            TcpClient tcpClient = client.GetTcpClient();

            // TEMP DEBUG
            Console.WriteLine("In - " + tcpClient.Client.Handle.ToString() + ": " + nextMessageText);

            GenericMessage genericMessage = JsonConvert.DeserializeObject<GenericMessage>(nextMessageText);
            switch (genericMessage.Type)
            {
                case MessageType.generic:
                    {
                        Console.WriteLine("Generic TCP message received: " + tcpClient.Client.Handle.ToString());
                    }
                    break;

                case MessageType.login:
                    {
                        Console.WriteLine("Login TCP message received: " + tcpClient.Client.Handle.ToString());

                        LoginMessage loginMessage = JsonConvert.DeserializeObject<LoginMessage>(nextMessageText);

                        bool verifyClient = true;
                        foreach(ConnectedClientInfo clientInfo in connectedClients)
                        {
                            if(client != clientInfo)
                            {                               
                                string clientInfoPlatformId = string.Empty;
                                string clientInfoPlayerHandle = string.Empty;
                                clientInfo.GetPlayerCredentials(out clientInfoPlatformId, out clientInfoPlayerHandle);

                                if (loginMessage.PlatformId == clientInfoPlatformId)
                                {
                                    verifyClient = false;
                                    break;
                                }
                            }
                        }

                        client.SetPlayerCredentials(loginMessage.PlatformId, loginMessage.PlayerHandle);

                        LoginAnswerMessage loginAnswer;
                        if (verifyClient)
                        {
                            loginAnswer = new LoginAnswerMessage(true, "OK");
                        }
                        else
                        {
                            loginAnswer = new LoginAnswerMessage(false, "Player already active in session");
                        }
                       client.AddMessage(loginAnswer);
                    }
                    break;
                case MessageType.logout:
                    {
                        Console.WriteLine("Logout TCP message received: " + tcpClient.Client.Handle.ToString());

                        // TEMP WORKAROUND - Close client connection
                        client.Dispose();
                    }
                    break;
                case MessageType.createSession:
                    {
                        Console.WriteLine("CreateSession TCP message received: " + tcpClient.Client.Handle.ToString());

                        CreateSessionMessage createSessionMessage = JsonConvert.DeserializeObject<CreateSessionMessage>(nextMessageText);

                        CreateSessionAnswerMessage createSessionAnswer;
                        if (client.SetCreateSessionState(createSessionMessage))
                        {
                            createSessionAnswer = new CreateSessionAnswerMessage(true, "OK");
                        }
                        else
                        {
                            createSessionAnswer = new CreateSessionAnswerMessage(false, "Player busy in session");
                        }
                        client.AddMessage(createSessionAnswer);
                    }
                    break;
                case MessageType.abortCreateSession:
                    {
                        Console.WriteLine("AbortCreateSessionMessage TCP message received: " + tcpClient.Client.Handle.ToString());

                        client.ClearCreateSessionState();
                    }
                    break;
                case MessageType.listSessions:
                    {
                        Console.WriteLine("ListSessions TCP message received: " + tcpClient.Client.Handle.ToString());

                        List<SessionInfo> listOfSessions = new List<SessionInfo>();
                        foreach(ConnectedClientInfo connectedClient in connectedClients)
                        {
                            if(connectedClient.IsCreatingSession)
                            {
                                string platformId;
                                string playerHandle;
                                connectedClient.GetPlayerCredentials(out platformId, out playerHandle);

                                SessionInfo newSessionInfo = new SessionInfo(connectedClient.CreateSessionMessage.MapName, 
                                    playerHandle, 
                                    connectedClient.CreateSessionMessage.OwnTeam == MessageLibraryUtitlity.SessionTeam.blue ? MessageLibraryUtitlity.SessionTeam.orange : MessageLibraryUtitlity.SessionTeam.blue,
                                    connectedClient.CreateSessionMessage.SessionSeconds);

                                listOfSessions.Add(newSessionInfo);
                            }
                        }

                        ListSessionsAnswerMessage listSessionsAnswer = new ListSessionsAnswerMessage();
                        if (listOfSessions.Count > 0)
                        {
                            listSessionsAnswer.Success = true;
                            listSessionsAnswer.Details = "OK";
                            listSessionsAnswer.Sessions = listOfSessions.ToArray();
                        }
                        else
                        {
                            listSessionsAnswer.Success = false;
                            listSessionsAnswer.Details = "No sessions available. Please refresh later";
                            listSessionsAnswer.Sessions = null;
                        }
                        client.AddMessage(listSessionsAnswer);
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Unhandled TCP message received: " + tcpClient.Client.Handle.ToString());
                    }
                    break;
            }
        }

        // Write player messages
        private void WriteClientMessages()
        {
            foreach (ConnectedClientInfo client in connectedClients)
            {
                if (!client.IsConnected)
                {
                    continue;
                }

                client.SendPendingMessages();
            }
        }

        private void ClearLostClients()
        {
            var connectedClientsArray = connectedClients.ToArray();
            connectedClients.Clear();

            for (int index = 0; index < connectedClientsArray.Length; index++)
            {
                ConnectedClientInfo client = connectedClientsArray[index];
                if(client.IsConnected)
                {
                    connectedClients.Add(client);
                    continue;
                }

                TcpClient tcpClient = client.GetTcpClient();

                Console.WriteLine("Client lost, clearing: " + tcpClient.Client.Handle.ToString());

                StorePlayerResults(client);

                client.Dispose();
            }
        }

        private void StorePlayerResults(ConnectedClientInfo client)
        {
            // TODO - Store persistent results of client before removing
        }
    }
}
