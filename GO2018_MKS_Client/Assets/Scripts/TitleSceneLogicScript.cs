using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public InputField PlayerHandleInputField;
    public string PlayerHandle;

    GameObject welcomeText;
    Text welcomeTextComponent;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }

        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        PlayerHandle = PlayerPrefs.GetString("playerHandle", "Player");
        if(PlayerHandleInputField != null)
        {
            PlayerHandleInputField.text = PlayerHandle;
        }

        welcomeText = GameObject.Find("WelcomeText");
        if (welcomeText != null)
        {
            welcomeTextComponent = welcomeText.GetComponent<Text>();
        }
    }

    void Update()
    {
        if (gameLogicScriptComponent == null)
        {
            return;
        }

        if (welcomeTextComponent != null)
        {
            welcomeTextComponent.text = gameLogicScriptComponent.GetWelcomeText();
        }
    }

    // Dev Shortcuts
    public void OnClickTestMapButton()
    {
        SceneManager.LoadScene("TestMap1Scene", LoadSceneMode.Single);
    }

    // Button Handlers
    public void OnClickCreateButton()
    {
        SceneManager.LoadScene("CreateSessionScene", LoadSceneMode.Single);
    }

    public void OnClickJoinButton()
    {
        SceneManager.LoadScene("JoinSessionScene", LoadSceneMode.Single);
    }

    public void OnClickSettingsButton()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }

    public void OnClickHelpButton()
    {
        SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }
     
    public void OnTextChangePlayerHandle()
    {
        if (PlayerHandleInputField != null)
        {
            PlayerHandle = PlayerHandleInputField.text;

            PlayerPrefs.SetString("playerHandle", PlayerHandle);
            PlayerPrefs.Save();
        }
    }
}
