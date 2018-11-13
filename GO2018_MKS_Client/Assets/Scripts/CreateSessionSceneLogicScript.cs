using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateSessionSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public Dropdown mapDropdown;
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
        SelectedMapIndex = mapDropdown.value;
    }

    public void OnValueChangeTeam()
    {
        SelectedTeamIndex = mapDropdown.value;
    }

    public void OnValueChangeSessionTime()
    {
        SelectedSessionTimeIndex = mapDropdown.value;
    }
}
