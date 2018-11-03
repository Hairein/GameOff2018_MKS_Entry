using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private TCPClientManager manager = new TCPClientManager();

    private string welcomeText = string.Empty;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // Read consistence data
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

        //Debug.Log("Platform ID: " + platformId);
        //Debug.Log("Player Handle: " + playerHandle);

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
        string tcpMessage = string.Empty;
        while(manager.ReceiveMessage(out tcpMessage))
        {
            //Debug.Log("TCP message received: " + tcpMessage);

            // Handle message according to type
            GenericMessage genericMessage = JsonConvert.DeserializeObject<GenericMessage>(tcpMessage);
            switch(genericMessage.Type)
            {
                case MessageType.welcome:
                    {
                        WelcomeMessage welcomeMessage = JsonConvert.DeserializeObject<WelcomeMessage>(tcpMessage);
                        welcomeText = welcomeMessage.text;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void HandleTCPMessage(string message)
    {
        Debug.Log("TCP message received from server: " + message);
    }

    public string GetWelcomeText()
    {
        return welcomeText;
    }
}
