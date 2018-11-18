using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

public class GameLogicScript : MonoBehaviour
{

    private TCPClientManager tcpClientManager = new TCPClientManager();
    public bool DidLogin = false;

    private string welcomeText = "Connecting To Server...";

    public string ClientVersion;

    public string DefaultHost;
    public int DefaultPort;

    public LoginAnswerMessage LoginAnswerMessage = null;

    public CreateSessionMessage createSessionMessage = null;
    public CreateSessionAnswerMessage createSessionAnswerMessage = null;

    public ListSessionsMessage listSessionsMessage = null;
    public ListSessionsAnswerMessage listSessionsAnswerMessage = null;

    public JoinSessionMessage joinSessionMessage = null;
    public JoinSessionAnswerMessage joinSessionAnswerMessage = null;

    public StartCreatedSessionAnswerMessage startCreatedSessionAnswerMessage = null;

    public SessionState SessionState = SessionState.none;
    public SessionResult SessionResult = SessionResult.pending;

    public ReadySessionStartMessage readySessionStartMessage = null;
    public ReadySessionStartAnswerMessage readySessionStartAnswerMessage = null;
    public OpponentSessionLostAnswerMessage opponentSessionLostAnswerMessage = null;
    public PlayerSessionLostMessage playerSessionLostMessage = null;

    public SessionUpdateAnswerMessage sessionUpdateAnswerMessage = null;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        // TODO Check if player still in active session, 
        // Report to server about abort
        if (SessionState == SessionState.waiting || SessionState == SessionState.ingame)
        {
            SetSessionSurrender();
        }

        DoLogout();
    }

    void Start()
    {
    }

    void Update()
    {
        while(true)
        {
            string tcpMessage = tcpClientManager.ReceiveMessage();
            if(string.IsNullOrEmpty(tcpMessage))
            {
                break;
            }

            HandleTCPMessage(tcpMessage);
        }
    }

    public void DoLogin(string host, int port)
    {
        DidLogin = false;

        // Read persistent data
        string platformId = PlayerPrefs.GetString("platformId", string.Empty);
        if (string.IsNullOrEmpty(platformId))
        {
            // Generate new platform ID
            Guid guid = Guid.NewGuid();
            platformId = guid.ToString();

            PlayerPrefs.SetString("platformId", platformId);
            PlayerPrefs.Save();
        }

        string playerHandle = PlayerPrefs.GetString("playerHandle", string.Empty);
        if (string.IsNullOrEmpty(playerHandle))
        {
            // Generate default player name
            playerHandle = "Player";

            PlayerPrefs.SetString("playerHandle", playerHandle);
            PlayerPrefs.Save();
        }

        tcpClientManager.ConnectToTcpServer(host, port);

        LoginMessage loginMessage = new LoginMessage();
        loginMessage.PlatformId = platformId;
        loginMessage.PlayerHandle = playerHandle;
        loginMessage.ClientVersion = ClientVersion;
        tcpClientManager.SendMessageObject(loginMessage);
    }

    public void DoLogout()
    {
        if(DidLogin == false)
        {
            return;
        }

        DidLogin = false;

        LogoutMessage logoutMessage = new LogoutMessage();
        tcpClientManager.SendMessageObject(logoutMessage);

        tcpClientManager.DisconnectFromTcpServer();
    }

    public void HandleTCPMessage(string message)
    {
        // Handle message according to type
        GenericMessage genericMessage = JsonConvert.DeserializeObject<GenericMessage>(message);

        // DEBUG only, hide in production
        //string incomingMessageText = string.Format("TCP message ({0}) received from server: {1}", MessageTypeTexts.GetMessageTypeText(genericMessage.Type), message);
        //Debug.Log(incomingMessageText);

        switch(genericMessage.Type)
        {
            case MessageType.welcome:
                {
                    WelcomeMessage welcomeMessage = JsonConvert.DeserializeObject<WelcomeMessage>(message);
                    welcomeText = welcomeMessage.Text;
                }
                break;
            case MessageType.loginAnswer:
                {
                    LoginAnswerMessage = JsonConvert.DeserializeObject<LoginAnswerMessage>(message);

                    DidLogin = true;
                }
                break;
            case MessageType.createSessionAnswer:
                {
                    createSessionAnswerMessage = JsonConvert.DeserializeObject<CreateSessionAnswerMessage>(message);
                }
                break;
            case MessageType.listSessionsAnswer:
                {
                    listSessionsAnswerMessage = JsonConvert.DeserializeObject<ListSessionsAnswerMessage>(message);
                }
                break;
            case MessageType.joinSessionAnswer:
                {
                    joinSessionAnswerMessage = JsonConvert.DeserializeObject<JoinSessionAnswerMessage>(message);
                }
                break;
            case MessageType.startCreatedSessionAnswer:
                {
                    startCreatedSessionAnswerMessage = JsonConvert.DeserializeObject<StartCreatedSessionAnswerMessage>(message);
                }
                break;
            case MessageType.readySessionStartAnswer:
                {
                    readySessionStartAnswerMessage = JsonConvert.DeserializeObject<ReadySessionStartAnswerMessage>(message);
                    SessionState = SessionState.ingame;
                }
                break;
             case MessageType.opponentSessionLostAnswer:
                {
                    opponentSessionLostAnswerMessage = JsonConvert.DeserializeObject<OpponentSessionLostAnswerMessage>(message);
                }
                break;
             case MessageType.sessionUpdateAnswer:
                {
                    sessionUpdateAnswerMessage = JsonConvert.DeserializeObject<SessionUpdateAnswerMessage>(message);
                }
                break;
           default:
                {
                    Debug.Log("Generic/Unknown TCP message received.");
                }
                break;
        }
    }

    public string GetWelcomeText()
    {
        return welcomeText;
    }

    public void CreateSession(int mapIndex, int teamIndex, int timeIndex)
    {
        if(mapIndex < 0 || mapIndex >= MessageLibraryUtitlity.SessionMapNames.Length)
        {
            return;
        }

        if (teamIndex < 0 || teamIndex > 2)
        {
            return;
        }

        if (timeIndex < 0 || timeIndex > MessageLibraryUtitlity.SessionDurationSeconds.Length)
        {
            return;
        }

        createSessionAnswerMessage = null;
        joinSessionAnswerMessage = null;
        startCreatedSessionAnswerMessage = null;

        string mapName = MessageLibraryUtitlity.SessionMapNames[mapIndex];
        MessageLibraryUtitlity.SessionTeam team = teamIndex == 0 ? MessageLibraryUtitlity.SessionTeam.blue : MessageLibraryUtitlity.SessionTeam.orange;
        int seconds = MessageLibraryUtitlity.SessionDurationSeconds[timeIndex];
        createSessionMessage = new CreateSessionMessage(mapName, team, seconds);
        tcpClientManager.SendMessageObject(createSessionMessage);
    }

    public void AbortCreateSession()
    {
        AbortCreateSessionMessage abortCreateSessionMessage = new AbortCreateSessionMessage();
        tcpClientManager.SendMessageObject(abortCreateSessionMessage);
    }

    public void GetSessionsList()
    {
        listSessionsAnswerMessage = null;

        listSessionsMessage = new ListSessionsMessage();
        tcpClientManager.SendMessageObject(listSessionsMessage);
    }

    public void JoinSession(int sessionIndex)
    {
        if(listSessionsAnswerMessage == null || listSessionsAnswerMessage.Sessions.Length == 0)
        {
            return;
        }

        createSessionAnswerMessage = null;
        joinSessionAnswerMessage = null;
        startCreatedSessionAnswerMessage = null;

        joinSessionMessage = new JoinSessionMessage(listSessionsAnswerMessage.Sessions[sessionIndex]);
        tcpClientManager.SendMessageObject(joinSessionMessage);
    }

    public void SetSessionReady()
    {
        SessionState = SessionState.waiting;
        SessionResult = SessionResult.pending;

        opponentSessionLostAnswerMessage = null;
        readySessionStartAnswerMessage = null;
        playerSessionLostMessage = null;

        readySessionStartMessage = new ReadySessionStartMessage();
        tcpClientManager.SendMessageObject(readySessionStartMessage);
    }

    public void SetSessionSurrender()
    {
        SessionState = SessionState.ending;
        SessionResult = SessionResult.lost;

        playerSessionLostMessage = new PlayerSessionLostMessage();
        tcpClientManager.SendMessageObject(playerSessionLostMessage);
    }

    public void SetSessionWon()
    {
        SessionState = SessionState.ending;
        SessionResult = SessionResult.won;
    }

    public void ClearSessionUpdateMessage()
    {
        sessionUpdateAnswerMessage = null;
    }

    public void EmitUnitNavigationCommands(GameObject[] units, Vector3 navigationTarget)
    {
        WorldCoordinate position = new WorldCoordinate(navigationTarget.x, navigationTarget.y, navigationTarget.z);

        List<string> names = new List<string>();
        foreach(GameObject unit in units)
        {
            names.Add(unit.name);
        }

        UnitNavigationCommand command = new UnitNavigationCommand(names.ToArray(), position);

        List<UnitNavigationCommand> commands = new List<UnitNavigationCommand>();
        commands.Add(command);

        PlayerUnitsNavigationMessage playerUnitsNavigationMessage = new PlayerUnitsNavigationMessage(commands.ToArray());
        tcpClientManager.SendMessageObject(playerUnitsNavigationMessage);
    }

    public void EmitUnitStateCommands(GameObject[] units)
    {
        List<UnitResourceState> states = new List<UnitResourceState>();

        foreach(GameObject unit in units)
        {
            UnitLogic unitLogic = unit.GetComponent<UnitLogic>();
            if(unitLogic == null)
            {
                continue;
            }

            UnitType unitType = UnitType.breeder;
            DroneLogic droneLogic = unit.GetComponent<DroneLogic>();
            if(droneLogic != null)
            {
                unitType = UnitType.drone;
            }

            UnitResourceState newState = new UnitResourceState(unit.name, unitType, unitLogic.FoodResourceCount, unitLogic.TechResourceCount);
            states.Add(newState);
        }

        if (states.Count > 0)
        {
            PlayerUnitsUpdateMessage playerUnitsUpdateMessage = new PlayerUnitsUpdateMessage(states.ToArray());
            tcpClientManager.SendMessageObject(playerUnitsUpdateMessage);
        }
    }

    public void EmitMineStateCommands(GameObject[] mines)
    {
        List<MineResourceState> states = new List<MineResourceState>();

        foreach (GameObject mine in mines)
        {
            FoodSourceLogic foodLogic = mine.GetComponent<FoodSourceLogic>();
            if (foodLogic != null)
            {
                MineResourceState newState = new MineResourceState(mine.name, MineType.food, foodLogic.ResourceCount);
                states.Add(newState);
            }
            else 
            {
                TechSourceLogic techLogic = mine.GetComponent<TechSourceLogic>();
                if (techLogic != null)
                {
                    MineResourceState newState = new MineResourceState(mine.name, MineType.tech, techLogic.ResourceCount);
                    states.Add(newState);
                }
            }
        }

        if (states.Count > 0)
        {
            MinesUpdateMessage minesUpdateMessage = new MinesUpdateMessage(states.ToArray());
            tcpClientManager.SendMessageObject(minesUpdateMessage);
        }
    }

    public void EmitBarricadeStateCommands(GameObject[] barricades)
    {
        List<BarricadeResourceState> states = new List<BarricadeResourceState>();

        foreach (GameObject barricade in barricades)
        {
            WorldCoordinate position = new WorldCoordinate(barricade.transform.position.x, barricade.transform.position.y, barricade.transform.position.z);

            BarricadeLogic barricadeLogic = barricade.GetComponent<BarricadeLogic>();
            if (barricadeLogic != null)
            {
                BarricadeResourceState newState = new BarricadeResourceState(barricade.name, position, barricadeLogic.LifeCount);
                states.Add(newState);
            }
         }

        if (states.Count > 0)
        {
            BarricadesUpdateMessage barricadesUpdateMessage = new BarricadesUpdateMessage(states.ToArray());
            tcpClientManager.SendMessageObject(barricadesUpdateMessage);
        }
    }
}
