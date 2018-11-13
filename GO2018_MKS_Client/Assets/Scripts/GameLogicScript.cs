using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private TCPClientManager tcpClientManager = new TCPClientManager();

    private string welcomeText = "Connecting To Server...";

    public LoginAnswerMessage LoginAnswerMessage = null;

    public CreateSessionMessage createSessionMessage = null;
    public CreateSessionAnswerMessage createSessionAnswerMessage = null;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

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

        tcpClientManager.ConnectToTcpServer();

        LoginMessage loginMessage = new LoginMessage();
        loginMessage.PlatformId = platformId;
        loginMessage.PlayerHandle = playerHandle;
        tcpClientManager.SendMessageObject(loginMessage);
    }

    private void OnDestroy()
    {
        LogoutMessage logoutMessage = new LogoutMessage();
        tcpClientManager.SendMessageObject(logoutMessage);

        tcpClientManager.DisconnectFromTcpServer();
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
                }
                break;
            case MessageType.createSessionAnswer:
                {
                    createSessionAnswerMessage = JsonConvert.DeserializeObject<CreateSessionAnswerMessage>(message);
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
}
