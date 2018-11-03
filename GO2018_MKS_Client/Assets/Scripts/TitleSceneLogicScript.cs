using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneLogicScript : MonoBehaviour
{
    GameLogicScript gameLogicScriptComponent = null;

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
        if (gameLogicScriptComponent == null)
        {
            return;
        }

        GameObject welcomeText = GameObject.Find("WelcomeText");
        if (welcomeText != null && string.IsNullOrEmpty(welcomeText.GetComponent<Text>().text))
        {
            welcomeText.GetComponent<Text>().text = gameLogicScriptComponent.GetWelcomeText();
        }
    }
}
