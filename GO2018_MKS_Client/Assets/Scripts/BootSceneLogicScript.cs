using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public Text ClientVersionText;

    public Text ServerListText;
    public Dropdown ServerList;
    public Text HostText;
    public InputField HostInputField;
    public Text PortText;
    public InputField PortInputField;

    public Button LoginButton;

    public Text ErrorText;

    public string WebsiteURL = "http://www.micahkoleoso.de";

    private string host = string.Empty;
    private int port = 0;

    private int storedServerIndex = -1;

    void Start()
    {
        HostText.gameObject.SetActive(false);
        HostInputField.gameObject.SetActive(false);
        PortText.gameObject.SetActive(false);
        PortInputField.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(gameLogicScriptComponent == null)
        {
            GameObject gameLogic = GameObject.Find("GameLogic");
            gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

            return;
        }

        if(storedServerIndex == -1)
        {
            storedServerIndex = PlayerPrefs.GetInt("serverIndex", 0);
            ServerList.value = storedServerIndex;
        }

        if (ClientVersionText.text != gameLogicScriptComponent.ClientVersion)
        {
            ClientVersionText.text = gameLogicScriptComponent.ClientVersion;
        }

        if (string.IsNullOrEmpty(host))
        {
            host = gameLogicScriptComponent.DefaultHost;
        }

        if (port == 0)
        {
            port = gameLogicScriptComponent.DefaultPort;
        }

        if (gameLogicScriptComponent.DidLogin)
        {
            if (gameLogicScriptComponent.LoginAnswerMessage != null)
            {
                if (gameLogicScriptComponent.LoginAnswerMessage.Success)
                {
                    if(ServerListText.gameObject.activeSelf) ServerListText.gameObject.SetActive(false);
                    if (ServerList.gameObject.activeSelf) ServerList.gameObject.SetActive(false);

                    if (HostText.gameObject.activeSelf) HostText.gameObject.SetActive(false);
                    if (HostInputField.gameObject.activeSelf) HostInputField.gameObject.SetActive(false);
                    if (PortText.gameObject.activeSelf) PortText.gameObject.SetActive(false);
                    if (PortInputField.gameObject.activeSelf) PortInputField.gameObject.SetActive(false);

                    if (LoginButton.gameObject.activeSelf) LoginButton.gameObject.SetActive(false);

                    if (!ErrorText.gameObject.activeSelf) ErrorText.gameObject.SetActive(false);

                    SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
                    return;
                }
                else
                {
                    if (ErrorText.text != gameLogicScriptComponent.LoginAnswerMessage.Details)
                    {
                        ErrorText.text = gameLogicScriptComponent.LoginAnswerMessage.Details;
                    }

                    if (!ErrorText.gameObject.activeSelf) ErrorText.gameObject.SetActive(true);
                }
            }
        }
    }

    // UI handlers
    public void OnValueChangeServerListDropdown()
    {
        if (gameLogicScriptComponent == null)
        {
            return;
        }

        int selectedIndex = ServerList.value;
        switch(selectedIndex)
        {
            case 0:
                {
                    HostText.gameObject.SetActive(false);
                    HostInputField.gameObject.SetActive(false);
                    PortText.gameObject.SetActive(false);
                    PortInputField.gameObject.SetActive(false);

                    host = gameLogicScriptComponent.DefaultHost;
                    port = gameLogicScriptComponent.DefaultPort;
                }
                break;
            case 1:
                {
                    HostText.gameObject.SetActive(false);
                    HostInputField.gameObject.SetActive(false);
                    PortText.gameObject.SetActive(true);
                    PortInputField.gameObject.SetActive(true);

                    host = "127.0.0.1"; // localhost
                    port = PlayerPrefs.GetInt("port", 8080);

                    PortInputField.text = port.ToString();
                }
                break;
            case 2:
                {
                    HostText.gameObject.SetActive(true);
                    HostInputField.gameObject.SetActive(true);
                    PortText.gameObject.SetActive(true);
                    PortInputField.gameObject.SetActive(true);

                    host = PlayerPrefs.GetString("host", "http://localhost");
                    port = PlayerPrefs.GetInt("port", 8080);

                    HostInputField.text = host;
                    PortInputField.text = port.ToString();
                }
                break;
            default:
                break;
        }

        if(storedServerIndex != selectedIndex)
        {
            storedServerIndex = selectedIndex;

            PlayerPrefs.SetInt("serverIndex", storedServerIndex);
            PlayerPrefs.Save();
        }
    }

    public void OnClickLoginButton()
    {
        if (gameLogicScriptComponent == null)
        {
            return;
        }

        int selectedIndex = ServerList.value;
        switch (selectedIndex)
        {
            case 0:
                {
                    gameLogicScriptComponent.DoLogin(gameLogicScriptComponent.DefaultHost, gameLogicScriptComponent.DefaultPort);
                }
                break;
            case 1:
            case 2:
                {
                    gameLogicScriptComponent.DoLogin(host, port);
                }
                break;
            default:
                break;
        }
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }

    public void OnTextChangeHost()
    {
        host = HostInputField.text;

        PlayerPrefs.SetString("host", host.Trim());
        PlayerPrefs.Save();
    }

    public void OnTextChangePort()
    {
        string portText = PortInputField.text;

        port = 8080;
        if (gameLogicScriptComponent != null)
        {
            port = gameLogicScriptComponent.DefaultPort;
        }

        int newPort = 0;
        if(int.TryParse(portText.Trim(), out newPort))
        {
            port = newPort;
        }

        PlayerPrefs.SetInt("port", port);
        PlayerPrefs.Save();
    }

    public void OnClickWebsiteButton()
    {
        Application.OpenURL(WebsiteURL);
    }
}
