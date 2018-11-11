using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public float SfxLevel = 0.0f;
    public float MusicLevel = 0.0f;

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

        // Read persistent data
        SfxLevel = PlayerPrefs.GetFloat("sfxLevel", 1.0f);
        if(SfxSlider != null)
        {
            SfxSlider.value = SfxLevel;
        }

        MusicLevel = PlayerPrefs.GetFloat("musicLevel", 0.25f);
        if (MusicSlider != null)
        {
            MusicSlider.value = MusicLevel;
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
            SfxLevel = SfxSlider.value;

            PlayerPrefs.SetFloat("sfxLevel", SfxLevel);
            PlayerPrefs.Save();
        }
    }

    public void OnValueChangeMusicLevel()
    {
        if (MusicSlider != null)
        {
            MusicLevel = MusicSlider.value;

            PlayerPrefs.SetFloat("musicLevel", MusicLevel);
            PlayerPrefs.Save();
        }
    }
}
