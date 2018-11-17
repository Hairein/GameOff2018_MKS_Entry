using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{

    private TCPClientManager tcpClientManager = new TCPClientManager();
    public bool DidLogin = false;

    private string welcomeText = "Connecting To Server...";

    public string ClientVersion = "v1.0preAlpha";

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

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
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
        Debug.Log("TCP message received from server: " + message);

        // Handle message according to type
        GenericMessage genericMessage = JsonConvert.DeserializeObject<GenericMessage>(message);
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
}
