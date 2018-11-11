using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSessionSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public RectTransform WaitingCursorRectTransform;
    public float RollAngle = 0.0f;
    public float RollDegreesPerSecond = 180.0f;

    public GameObject SessionEntryTemplate;
    public GameObject SessionContent;
    public GameObject[] SessionEntryList;

    public int SelectedEntryIndex = 0;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }

        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        SelectedEntryIndex = 0;
        BuildSessionEntries(SelectedEntryIndex);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (WaitingCursorRectTransform != null)
        {
            WaitingCursorRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, RollAngle);
        }

        RollAngle -= RollDegreesPerSecond * deltaTime;
        while (RollAngle < 0.0f)
        {
            RollAngle += 360.0f;
        }
    }

    private void BuildSessionEntries(int selectedIndex)
    {
        if(SessionContent == null || SessionEntryTemplate == null)
        {
            return;
        }

        List<GameObject> listOfEntries = new List<GameObject>();
        for (int index = 0; index < 16; index++)
        {
            GameObject newEntry = GameObject.Instantiate(SessionEntryTemplate, SessionContent.transform); 
            newEntry.SetActive(true);

            SessionEntryPanelScript newSessionEntryPanelScript = newEntry.GetComponent<SessionEntryPanelScript>();
            if (newSessionEntryPanelScript != null)
            {
                newSessionEntryPanelScript.IsSelected = index == selectedIndex;
                newSessionEntryPanelScript.UpdateSelectionState();

                newSessionEntryPanelScript.EntryIndex = index;
            }

            listOfEntries.Add(newEntry);
        }

        SessionEntryList = listOfEntries.ToArray();
    }

    public void EntrySelected(int selectedEntryIndex)
    {
        // Deselect old entry
        SessionEntryPanelScript oldSessionEntryPanelScript = SessionEntryList[SelectedEntryIndex].GetComponent<SessionEntryPanelScript>();
        if(oldSessionEntryPanelScript != null)
        {
            oldSessionEntryPanelScript.IsSelected = false;
            oldSessionEntryPanelScript.UpdateSelectionState();
        }

        // Select new entry
        SessionEntryPanelScript newSessionEntryPanelScript = SessionEntryList[selectedEntryIndex].GetComponent<SessionEntryPanelScript>();
        if(newSessionEntryPanelScript != null)
        {
            newSessionEntryPanelScript.IsSelected = true;
            newSessionEntryPanelScript.UpdateSelectionState();

            SelectedEntryIndex = selectedEntryIndex;
        }
    }

    // Button Handlers
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

    public void OnClickJoinButton()
    {
        // TODO - Join selected game if still free
    }

    public void OnClickRefreshButton()
    {
        // TODO - Refresh server session list
    }
}
