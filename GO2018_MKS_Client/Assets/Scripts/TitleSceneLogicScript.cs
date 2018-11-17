using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    private GameObject welcomeText;
    private Text welcomeTextComponent;

    public Text ClientVersionText;

    public Text PlayerHandleText;
    public InputField PlayerHandleInputField;
    public string PlayerHandle;

    private bool loginCompleted = false;
    public GameObject LoginFailPanel;
    public Text LoginFailDetails;

    public Button createSessionButton;
    public Button joinSessionButton;
    public Button settingsButton;
    public Button helpButton;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }

        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        ClientVersionText.text = gameLogicScriptComponent.ClientVersion;

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

        createSessionButton.gameObject.SetActive(true);
        joinSessionButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        helpButton.gameObject.SetActive(true);
        PlayerHandleText.gameObject.SetActive(true);
        PlayerHandleInputField.gameObject.SetActive(true);
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

        if(!loginCompleted)
        {
            if(gameLogicScriptComponent.LoginAnswerMessage != null)
            {
                loginCompleted = true;

                if(!gameLogicScriptComponent.LoginAnswerMessage.Success)
                {
                    LoginFailDetails.text = gameLogicScriptComponent.LoginAnswerMessage.Details;
                    LoginFailPanel.gameObject.SetActive(true);
                }
                else
                {
                    createSessionButton.gameObject.SetActive(true);
                    joinSessionButton.gameObject.SetActive(true);
                    settingsButton.gameObject.SetActive(true);
                    helpButton.gameObject.SetActive(true);
                    PlayerHandleText.gameObject.SetActive(true);
                    PlayerHandleInputField.gameObject.SetActive(true);
                }
            }
        }
    }

    // Dev Shortcuts
    public void OnClickTestMapMorpholiteButton()
    {
        SceneManager.LoadScene("MapMorpholite", LoadSceneMode.Single);
    }

    public void OnClickTestMapSunsetButton()
    {
        SceneManager.LoadScene("MapSunset", LoadSceneMode.Single);
    }

    public void OnClickTestMapOverlordButton()
    {
        SceneManager.LoadScene("MapOverlord", LoadSceneMode.Single);
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
            PlayerHandle = PlayerHandleInputField.text.Trim();

            PlayerPrefs.SetString("playerHandle", PlayerHandle);
            PlayerPrefs.Save();
        }
    }
}
