using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_Server
{
    class Program
    {
        string serverVersion = "v1.0preAlpha2";

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
            Console.WriteLine("GO2018 MKS Server [" + serverVersion + "]");
            Console.WriteLine("Listening on port: " + port.ToString());

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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            while (runFlag)
            {
                // ---
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }

                float deltaTimeMs = stopwatch.ElapsedMilliseconds;

                if (!stopwatch.IsRunning)
                {
                    stopwatch.Restart();
                }
                //---

                AcceptClients(server);

                ResetSessionData();
                HandleClientMessages();
                HandleSessions(deltaTimeMs);

                WriteClientMessages();
                ClearLostClients();

                // Break for a bit if empty server
                if (sessions.Count == 0)
                {
                    Thread.Sleep(250);
                }
                else
                {
                    Thread.Sleep(85);
                }
            }

            if(stopwatch.IsRunning)
            {
                stopwatch.Stop();
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

                    // Update client timestamp
                    client.LastMessageTimestamp = DateTime.UtcNow;

                    HandleIncomingMessage(client, inputMessage);                                        
                }            
            }
        }

        private void HandleIncomingMessage(ConnectedClientInfo client, string nextMessageText)
        {
            TcpClient tcpClient = client.GetTcpClient();

            GenericMessage genericMessage = JsonConvert.DeserializeObject<GenericMessage>(nextMessageText);

            // TEMP DEBUG
            //Console.WriteLine("In - " + tcpClient.Client.Handle.ToString() + ": (" + MessageTypeTexts.GetMessageTypeText(genericMessage.Type) + ")" + nextMessageText);

            switch (genericMessage.Type)
            {
                case MessageType.generic:
                    {
                    }
                    break;
                case MessageType.login:
                    {
                        LoginMessage loginMessage = JsonConvert.DeserializeObject<LoginMessage>(nextMessageText);

                        LoginAnswerMessage loginAnswer;

                        if(loginMessage.ClientVersion != serverVersion)
                        {
                            string mismatch = string.Format("Client [{0}] / Server [{1}] version mismatch. Unable to login.", loginMessage.ClientVersion, serverVersion);

                            loginAnswer = new LoginAnswerMessage(false, mismatch);
                            client.AddMessage(loginAnswer);
                            break;
                        }

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
                        if (client.IsCreatingSession)
                        {
                            client.ClearCreateSessionState();
                        }

                        InformSessionsOfLostClient(client);

                        // TEMP WORKAROUND - Close client connection
                        client.Dispose();
                    }
                    break;
                case MessageType.createSession:
                    {
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
                        if (client.IsCreatingSession)
                        {
                            client.ClearCreateSessionState();
                        }

                    }
                    break;
                case MessageType.listSessions:
                    {
                        List<SessionInfo> listOfSessions = new List<SessionInfo>();
                        foreach(ConnectedClientInfo connectedClient in connectedClients)
                        {
                            if(client == connectedClient)
                            {
                                continue;
                            }

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
                case MessageType.readySessionStart:
                    {
                        foreach(ActiveSessionInfo session in sessions)
                        {
                            if(client == session.player1 || client == session.player2)
                            {
                                session.ReadyStateCounter++;

                                if(session.ReadyStateCounter == 2)
                                {
                                    ReadySessionStartAnswerMessage readySessionStartAnswerMessage = new ReadySessionStartAnswerMessage(true, "OK");
                                    session.player1.AddMessage(readySessionStartAnswerMessage);
                                    session.player2.AddMessage(readySessionStartAnswerMessage);

                                    session.State = SessionState.ingame;
                                }
                            }
                        }
                    }
                    break;
                case MessageType.playerSessionLost:
                    {
                        InformSessionsOfLostClient(client);
                    }
                    break;
                case MessageType.playerUnitsNavigation:
                    {
                        PlayerUnitsNavigationMessage playerUnitsNavigationMessage = JsonConvert.DeserializeObject<PlayerUnitsNavigationMessage>(nextMessageText);

                        ActiveSessionInfo activeSession = FindActiveSessionFromClient(client);
                        if(activeSession != null)
                        {
                            if (client == activeSession.player1)
                            {
                                foreach (UnitNavigationCommand command in playerUnitsNavigationMessage.NavigationCommands)
                                {
                                    activeSession.CollectSessionUpdateAnswers.AddPlayer1UnitNavigationCommand(command);
                                }
                            }
                            else 
                            {
                                foreach (UnitNavigationCommand command in playerUnitsNavigationMessage.NavigationCommands)
                                {
                                    activeSession.CollectSessionUpdateAnswers.AddPlayer2UnitNavigationCommand(command);
                                }
                            }
                        }
                    }
                    break;
                case MessageType.playerUnitsUpdate: 
                    {
                        PlayerUnitsUpdateMessage playerUnitsUpdateMessage = JsonConvert.DeserializeObject<PlayerUnitsUpdateMessage>(nextMessageText);

                        ActiveSessionInfo activeSession = FindActiveSessionFromClient(client);
                        if (activeSession != null)
                        {
                            if (client == activeSession.player1)
                            {
                                foreach (UnitResourceState newState in playerUnitsUpdateMessage.UnitResourceStates)
                                {
                                    bool isHandled = false;
                                    foreach (UnitResourceState existingState in activeSession.CollectSessionUpdateAnswers.Player1UnitResourceStates)
                                    {
                                        if(newState.Name == existingState.Name)
                                        {
                                            isHandled = true;

                                            existingState.Position = newState.Position;

                                            existingState.FoodResourceCount = newState.FoodResourceCount;
                                            existingState.TechResourceCount = newState.TechResourceCount;

                                            break;
                                        }
                                    }

                                    if(!isHandled)
                                    {
                                        activeSession.CollectSessionUpdateAnswers.AddPlayer1UnitResourceStates(newState);
                                    }
                                }
                            }
                            else 
                            {
                                foreach (UnitResourceState newState in playerUnitsUpdateMessage.UnitResourceStates)
                                {
                                    bool isHandled = false;
                                    foreach (UnitResourceState existingState in activeSession.CollectSessionUpdateAnswers.Player2UnitResourceStates)
                                    {
                                        if (newState.Name == existingState.Name)
                                        {
                                            isHandled = true;

                                            existingState.Position = newState.Position;

                                            existingState.FoodResourceCount = newState.FoodResourceCount;
                                            existingState.TechResourceCount = newState.TechResourceCount;

                                            break;
                                        }
                                    }

                                    if (!isHandled)
                                    {
                                        activeSession.CollectSessionUpdateAnswers.AddPlayer2UnitResourceStates(newState);
                                    }
                                }
                            }
                        }
                     }
                    break;
                case MessageType.mineResourcesUpdate:
                    {
                        MinesUpdateMessage minesUpdateMessage = JsonConvert.DeserializeObject<MinesUpdateMessage>(nextMessageText);

                        ActiveSessionInfo activeSession = FindActiveSessionFromClient(client);
                        if (activeSession != null)
                        {
                            foreach (MineResourceState newState in minesUpdateMessage.MineResourceStates)
                            {
                                bool isHandled = false;
                                foreach (MineResourceState existingState in activeSession.CollectSessionUpdateAnswers.MineResourceStates)
                                {
                                    if (newState.Name == existingState.Name)
                                    {
                                        isHandled = true;

                                        existingState.ResourceCount = Math.Min(existingState.ResourceCount, newState.ResourceCount);
                                        break;
                                    }
                                }

                                if (!isHandled)
                                {
                                    activeSession.CollectSessionUpdateAnswers.AddMineResourceState(newState);
                                }
                            }
                        }
                    }
                    break;
                case MessageType.barricadesUpdate:
                    {
                        BarricadesUpdateMessage barricadesUpdateMessage = JsonConvert.DeserializeObject<BarricadesUpdateMessage>(nextMessageText);

                        ActiveSessionInfo activeSession = FindActiveSessionFromClient(client);
                        if (activeSession != null)
                        {
                            foreach (BarricadeResourceState newState in barricadesUpdateMessage.Barricades)
                            {
                                bool isHandled = false;
                                foreach (BarricadeResourceState existingState in activeSession.CollectSessionUpdateAnswers.BarricadeResourceStates)
                                {
                                    if (newState.Name == existingState.Name)
                                    {
                                        isHandled = true;

                                        existingState.ResourceCount = Math.Min(existingState.ResourceCount, newState.ResourceCount);
                                        break;
                                    }
                                }

                                if (!isHandled)
                                {
                                    activeSession.CollectSessionUpdateAnswers.AddBarricadeResourceState(newState);
                                }
                            }
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

                DateTime compareTimestamp = DateTime.UtcNow;
                TimeSpan compareTimeSpan = compareTimestamp - client.LastMessageTimestamp;

                if (client.IsConnected && compareTimeSpan.TotalMinutes <= 15.0)
                {
                    connectedClients.Add(client);
                    continue;
                }

                TcpClient tcpClient = client.GetTcpClient();

                if (compareTimeSpan.TotalMinutes <= 15.0)
                {
                    Console.WriteLine("Client disconnect lost, clearing: " + tcpClient.Client.Handle.ToString() + ", joined duration: " + client.GetClientConnectionDuration().ToString());
                }
                else
                {
                    Console.WriteLine("Client timeout lost, clearing: " + tcpClient.Client.Handle.ToString() + ", joined duration: " + client.GetClientConnectionDuration().ToString());
                }

                StorePlayerResults(client);

                InformSessionsOfLostClient(client);

                client.Dispose();
            }
        }

        private void StorePlayerResults(ConnectedClientInfo client)
        {
            // TODO - Store persistent results of client before removing
        }

        private void ResetSessionData()
        {
            for (int index = 0; index < sessions.Count; index++)
            {
                ActiveSessionInfo activeSession = sessions[index];

                activeSession.CollectSessionUpdateAnswers.Reset();
            }
        }

        private void HandleSessions(float deltaTime)
        {
            for (int index = 0; index < sessions.Count; index++)
            {
                ActiveSessionInfo activeSession = sessions[index];

                switch(activeSession.State)
                {
                    case SessionState.waiting:
                        {

                        }
                        break;
                    case SessionState.ingame:
                        {
                            activeSession.Update(deltaTime);

                            if(activeSession.CollectSessionUpdateAnswers.SessionTimeLeft <= 0.0f)
                            {
                                EndSessionAnswerMessage endSessionAnswerMessage = new EndSessionAnswerMessage();
                                activeSession.player1.AddMessage(endSessionAnswerMessage);
                                activeSession.player2.AddMessage(endSessionAnswerMessage);

                                activeSession.player1.ClearActiveSessionState();
                                activeSession.player2.ClearActiveSessionState();

                                RemoveSession(activeSession);
                            }
                        }
                        break;
                    case SessionState.ending:
                        {

                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void InformSessionsOfLostClient(ConnectedClientInfo client)
        {
            ActiveSessionInfo[] sessionsArray = sessions.ToArray();
            sessions = new List<ActiveSessionInfo>();

            for (int index = 0; index < sessionsArray.Length; index++)
            {
                ActiveSessionInfo session = sessionsArray[index];

                if (client == session.player1 || client == session.player2)
                {
                    OpponentSessionLostAnswerMessage opponentSessionLostAnswerMessage = new OpponentSessionLostAnswerMessage();
                    if (client == session.player1)
                    {
                        session.player2.AddMessage(opponentSessionLostAnswerMessage);
                    }
                    else if (client == session.player2)
                    {
                        session.player1.AddMessage(opponentSessionLostAnswerMessage);
                    }

                    session.player1.ClearCreateSessionState();
                    session.player1.ClearActiveSessionState();

                    session.player2.ClearCreateSessionState();
                    session.player2.ClearActiveSessionState();

                    continue;
                }

                sessions.Add(session);
            }
        }

        private ActiveSessionInfo FindActiveSessionFromClient(ConnectedClientInfo client)
        {
            ActiveSessionInfo activeSession = null;

            ActiveSessionInfo[] sessionsArray = sessions.ToArray();
            for (int index = 0; index < sessionsArray.Length; index++)
            {
                ActiveSessionInfo session = sessionsArray[index];

                if (client == session.player1 || client == session.player2)
                {
                    activeSession = session;
                    break;
                }
            }

            return activeSession;
        }

        private void RemoveSession(ActiveSessionInfo session)
        {
            ActiveSessionInfo[] sessionsArray = sessions.ToArray();

            sessions.Clear();

            for (int index = 0; index < sessionsArray.Length; index++)
            {
                ActiveSessionInfo checkSession = sessionsArray[index];

                if (session == checkSession)
                {
                    continue;
                }

                sessions.Add(checkSession);
            }
        }
    }
}
