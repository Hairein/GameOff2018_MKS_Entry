using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

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
}
