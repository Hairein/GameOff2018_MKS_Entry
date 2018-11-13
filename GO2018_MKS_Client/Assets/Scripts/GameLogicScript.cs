using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private TCPClientManager manager = new TCPClientManager();

    private string welcomeText = "Connecting To Server...";

    public string SessionMapScene = string.Empty;
    public int SessionTeamNumber = 1;
    public int SessionTimeSeconds = 5 * 60;

    public LoginAnswerMessage LoginAnswerMessage = null;

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

        manager.ConnectToTcpServer();

        LoginMessage loginMessage = new LoginMessage();
        loginMessage.PlatformId = platformId;
        loginMessage.PlayerHandle = playerHandle;
        string message = JsonConvert.SerializeObject(loginMessage);
        manager.SendMessage(message);
    }

    private void OnDestroy()
    {
        LogoutMessage logoutMessage = new LogoutMessage();
        string message = JsonConvert.SerializeObject(logoutMessage);
        manager.SendMessage(message);

        manager.DisconnectFromTcpServer();
    }

    void Start()
    {
    }

    void Update()
    {
        while(true)
        {
            string tcpMessage = manager.ReceiveMessage();
            if(string.IsNullOrEmpty(tcpMessage))
            {
                break;
            }

            HandleTCPMessage(tcpMessage);
        }
    }

    public void HandleTCPMessage(string message)
    {
        //Debug.Log("TCP message received from server: " + message);

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

    public void ReportCreateSessionChoices(int mapIndex, int teamIndex, int timeIndex)
    {
        SessionMapScene = string.Format("Map{0}", mapIndex);

        SessionTeamNumber =  teamIndex == 0 ? 1 : 2;
    
        SessionTimeSeconds = timeIndex * 150;
    }
}
