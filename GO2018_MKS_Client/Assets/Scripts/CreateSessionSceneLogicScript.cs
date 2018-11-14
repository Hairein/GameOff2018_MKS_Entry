using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateSessionSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public Dropdown MapsDropdown;
    public Dropdown TeamsDropdown;
    public Dropdown TimesDropdown;

    public int SelectedMapIndex;
    public int SelectedTeamIndex;
    public int SelectedSessionTimeIndex;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }

        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        Dropdown mapsDropdown = MapsDropdown.GetComponent<Dropdown>();
        if (mapsDropdown != null)
        {
            mapsDropdown.value = SelectedMapIndex;
        }

        Dropdown teamsDropDown = TeamsDropdown.GetComponent<Dropdown>();
        if (teamsDropDown != null)
        {
            teamsDropDown.value = SelectedTeamIndex;
        }

        Dropdown timesDropDown = TimesDropdown.GetComponent<Dropdown>();
        if (timesDropDown != null)
        {
            timesDropDown.value = SelectedSessionTimeIndex;
        }
    }

    void Update()
    {
    }

    // Button Handlers
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

    public void OnClickCreateButton()
    {
        if (gameLogicScriptComponent != null)
        {
            gameLogicScriptComponent.CreateSession(SelectedMapIndex, SelectedTeamIndex, SelectedSessionTimeIndex);

            SceneManager.LoadScene("WaitingForOpponentScene", LoadSceneMode.Single);
        }
    }

    public void OnValueChangeMap()
    {
        SelectedMapIndex = MapsDropdown.value;
    }

    public void OnValueChangeTeam()
    {
        SelectedTeamIndex = TeamsDropdown.value;
    }

    public void OnValueChangeSessionTime()
    {
        SelectedSessionTimeIndex = TimesDropdown.value;
    }
}
