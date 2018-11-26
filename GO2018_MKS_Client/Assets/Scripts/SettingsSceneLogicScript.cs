using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public Slider SfxSlider;
    public Slider MusicSlider;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }
        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        if (SfxSlider != null)
        {
            SfxSlider.value = gameLogicScriptComponent.SfxLevel;
        }

        if (MusicSlider != null)
        {
            MusicSlider.value = gameLogicScriptComponent.MusicLevel;
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

    public void OnValueChangeSfxLevel()
    {
        if (SfxSlider != null)
        {
            gameLogicScriptComponent.SfxLevel = SfxSlider.value;

            PlayerPrefs.SetFloat("sfxLevel", gameLogicScriptComponent.SfxLevel);
            PlayerPrefs.Save();
        }
    }

    public void OnValueChangeMusicLevel()
    {
        if (MusicSlider != null)
        {
            gameLogicScriptComponent.MusicLevel = MusicSlider.value;

            PlayerPrefs.SetFloat("musicLevel", gameLogicScriptComponent.MusicLevel);
            PlayerPrefs.Save();
        }
    }
}
