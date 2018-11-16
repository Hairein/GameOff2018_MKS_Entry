using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_Server
{
    class Program
    {
        bool runFlag = true;

        List<ConnectedClientInfo> connectedClients = new List<ConnectedClientInfo>();
        int maxPlayersAllowed = 128;

        List<ActiveSessionInfo> sessions = new List<ActiveSessionInfo>();
        int maxActiveSessionsAllowed = 64;

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
                    case "players":
                        {
                            PrintPlayers();
                        }
                        break;
                    case "sessions":
                        {
                            PrintSessions();
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
            Console.WriteLine("Commands (help, quit, exit, players, sessions)");
        }

        void PrintPlayers()
        {
            Console.WriteLine(string.Format("Players: {0}/{1}", connectedClients.Count, maxPlayersAllowed));
        }

        void PrintSessions()
        {
            Console.WriteLine(string.Format("Sessions: {0}/{1}", sessions.Count, maxActiveSessionsAllowed));
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

                        LoginAnswerMessage loginAnswer;
                        if (connectedClients.Count >= maxPlayersAllowed)
                        {
                            loginAnswer = new LoginAnswerMessage(false, "Maximum number of players supported already logged in. Please login again later");
                            client.AddMessage(loginAnswer);
                            break;
                        }

                        bool verifyClient = true;
                        foreach (ConnectedClientInfo clientInfo in connectedClients)
                        {
                            if (client != clientInfo)
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

                        if (!verifyClient)
                        {
                            loginAnswer = new LoginAnswerMessage(false, "Player already active in session");
                            client.AddMessage(loginAnswer);
                            break;
                        }

                        loginAnswer = new LoginAnswerMessage(true, "OK");
                        client.AddMessage(loginAnswer);
                    }
                    break;
                case MessageType.logout:
                    {
                        Console.WriteLine("Logout TCP message received: " + tcpClient.Client.Handle.ToString());

                        // TEMP WORKAROUND - Close client connection
                        if(client.IsCreatingSession)
                        {
                            client.ClearCreateSessionState();
                        }

                        client.Dispose();
                    }
                    break;
                case MessageType.createSession:
                    {
                        Console.WriteLine("CreateSession TCP message received: " + tcpClient.Client.Handle.ToString());

                        CreateSessionMessage createSessionMessage = JsonConvert.DeserializeObject<CreateSessionMessage>(nextMessageText);

                        CreateSessionAnswerMessage createSessionAnswer;
                        if(sessions.Count >= maxActiveSessionsAllowed)
                        {
                            createSessionAnswer = new CreateSessionAnswerMessage(false, "Maximum number of sessions reached. Please create a session again later");
                            client.AddMessage(createSessionAnswer);
                            break;
                        }

                        if(client.IsInActiveSession)
                        {
                            createSessionAnswer = new CreateSessionAnswerMessage(false, "Player is busy in an existing active session");
                            client.AddMessage(createSessionAnswer);
                            break;
                        }

                        bool createSessionResult = client.SetCreateSessionState(createSessionMessage);
                        if (!createSessionResult)
                        {
                            createSessionAnswer = new CreateSessionAnswerMessage(false, "Player is busy in an existing session");
                            client.AddMessage(createSessionAnswer);
                            break;
                        }

                        createSessionAnswer = new CreateSessionAnswerMessage(true, "OK");
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
                        if (listOfSessions.Count == 0)
                        {
                            listSessionsAnswer.Success = false;
                            listSessionsAnswer.Details = "No sessions available. Please refresh later";
                            listSessionsAnswer.Sessions = null;
                            client.AddMessage(listSessionsAnswer);
                            break;
                        }

                        listSessionsAnswer.Success = true;
                        listSessionsAnswer.Details = "OK";
                        listSessionsAnswer.Sessions = listOfSessions.ToArray();
                        client.AddMessage(listSessionsAnswer);
                    }
                    break;
                case MessageType.joinSession:
                    {
                        Console.WriteLine("JoinSessionMessage TCP message received: " + tcpClient.Client.Handle.ToString());

                        JoinSessionMessage joinSessionMessage = JsonConvert.DeserializeObject<JoinSessionMessage>(nextMessageText);

                        SessionInfo chosenSession = joinSessionMessage.session;

                        foreach (ConnectedClientInfo sessionSourceClient in connectedClients)
                        {
                            if (!sessionSourceClient.IsCreatingSession)
                            {
                                continue;
                            }

                            JoinSessionAnswerMessage joinSessionAnswer = null;
                            StartCreatedSessionAnswerMessage startCreatedSession = null;

                            if (sessionSourceClient.IsInActiveSession)
                            {
                                joinSessionAnswer = new JoinSessionAnswerMessage(false, "Opponent is busy in an existing active session");
                                client.AddMessage(joinSessionAnswer);
                                break;
                            }

                            if (client.IsInActiveSession)
                            {
                                joinSessionAnswer = new JoinSessionAnswerMessage(false, "Player is busy in an existing active session");
                                client.AddMessage(joinSessionAnswer);
                                break;
                            }

                            string sourcePlatformId = string.Empty;
                            string sourcePlayerHandle = string.Empty;
                            sessionSourceClient.GetPlayerCredentials(out sourcePlatformId, out sourcePlayerHandle);

                            string joinPlatformId = string.Empty;
                            string joinPlayerHandle = string.Empty;
                            client.GetPlayerCredentials(out joinPlatformId, out joinPlayerHandle);

                            SessionTeam opponentTeam = sessionSourceClient.CreateSessionMessage.OwnTeam == SessionTeam.blue ? SessionTeam.orange : SessionTeam.blue;

                            if (chosenSession.MapName != sessionSourceClient.CreateSessionMessage.MapName
                                && chosenSession.OpponentHandle != sourcePlayerHandle
                                && chosenSession.SuggestedTeam != opponentTeam
                                && chosenSession.DurationSeconds != sessionSourceClient.CreateSessionMessage.SessionSeconds)
                            {
                                joinSessionAnswer = new JoinSessionAnswerMessage(false, "Mismatch in the session parameters. Please restart the game");
                                client.AddMessage(joinSessionAnswer);
                                break;
                            }

                            ActiveSessionInfo newActiveSessionInfo = new ActiveSessionInfo(sessionSourceClient, client, sessionSourceClient.CreateSessionMessage.MapName, sessionSourceClient.CreateSessionMessage.SessionSeconds);
                            sessions.Add(newActiveSessionInfo);

                            sessionSourceClient.ClearCreateSessionState();
                            sessionSourceClient.SetActiveSessionState(newActiveSessionInfo);

                            client.SetActiveSessionState(newActiveSessionInfo);

                            startCreatedSession = new StartCreatedSessionAnswerMessage(true, "OK", joinPlayerHandle);
                            sessionSourceClient.AddMessage(startCreatedSession);

                            joinSessionAnswer = new JoinSessionAnswerMessage(true, "OK");
                            client.AddMessage(joinSessionAnswer);
                        }
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
