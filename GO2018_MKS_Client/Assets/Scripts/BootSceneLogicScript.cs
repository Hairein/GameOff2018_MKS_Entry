using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLogicScript : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
