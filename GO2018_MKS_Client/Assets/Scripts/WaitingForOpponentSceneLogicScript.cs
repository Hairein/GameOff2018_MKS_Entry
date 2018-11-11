using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingForOpponentSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public RectTransform WaitingCursorRectTransform;
    public float RollAngle = 0.0f;
    public float RollDegreesPerSecond = 180.0f;

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
        float deltaTime = Time.deltaTime;

        if(WaitingCursorRectTransform != null)
        {
            WaitingCursorRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, RollAngle);
        }

        RollAngle -= RollDegreesPerSecond * deltaTime;
        while(RollAngle < 0.0f)
        {
            RollAngle += 360.0f;
        }
    }

    // Button Handlers
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("CreateSessionScene", LoadSceneMode.Single);
    }
}
