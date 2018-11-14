using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpSceneLogicScript : MonoBehaviour
{
    void Start()
    {
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
