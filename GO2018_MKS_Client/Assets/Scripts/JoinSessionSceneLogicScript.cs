using GO2018_MKS_MessageLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public ScrollRect sessionListScrollRect;
    public Button sessionJoinButton;
    public Text NoSessionsText;

    public bool rebuildSessionList = true;

    public bool doJoinSessionHandling = false;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }

        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        PrepareSessionListRefresh();
    }

    void Update()
    {
        if(gameLogicScriptComponent == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        if (gameLogicScriptComponent.listSessionsAnswerMessage == null)
        {
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
        else
        {
            if (rebuildSessionList == true)
            {
                WaitingCursorRectTransform.gameObject.SetActive(false);

                if (gameLogicScriptComponent.listSessionsAnswerMessage.Success)
                {
                    HandleSuccessfulSessionListUpdate();
                }
                else
                {
                    HandleFailedSessionListUpdate();
                }

                rebuildSessionList = false;
            }
        }

        if(doJoinSessionHandling)
        {
            if(gameLogicScriptComponent.joinSessionAnswerMessage != null)
            {
                if(gameLogicScriptComponent.joinSessionAnswerMessage.Success)
                {
                    // Goto ingame scene
                    string finalMapName = string.Format("Map{0}", gameLogicScriptComponent.joinSessionMessage.session.MapName);
                    SceneManager.LoadScene(finalMapName, LoadSceneMode.Single);
                }
                else
                {
                    // Go back 
                    SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
                }
            }
        }
    }

    private void PrepareSessionListRefresh()
    {
        rebuildSessionList = true;

        sessionListScrollRect.gameObject.SetActive(false);
        sessionJoinButton.gameObject.SetActive(false);
        NoSessionsText.gameObject.SetActive(false);
        WaitingCursorRectTransform.gameObject.SetActive(true);

        gameLogicScriptComponent.GetSessionsList();
    }

    private void HandleSuccessfulSessionListUpdate()
    {
        SelectedEntryIndex = 0;
        BuildSessionEntries(SelectedEntryIndex);

        sessionListScrollRect.gameObject.SetActive(true);
        sessionJoinButton.gameObject.SetActive(true);
        NoSessionsText.gameObject.SetActive(false);
    }

    private void HandleFailedSessionListUpdate()
    {
        NoSessionsText.gameObject.SetActive(true);
    }

    private void BuildSessionEntries(int selectedIndex)
    {
        if(SessionContent == null || SessionEntryTemplate == null)
        {
            return;
        }

        SessionInfo[] sessionInfoArray = gameLogicScriptComponent.listSessionsAnswerMessage.Sessions;

        List<GameObject> listOfEntries = new List<GameObject>();
        for (int index = 0; index < sessionInfoArray.Length; index++)
        {
            SessionInfo sessionInfo = sessionInfoArray[index];

            GameObject newEntry = GameObject.Instantiate(SessionEntryTemplate, SessionContent.transform); 
            newEntry.SetActive(true);

            SessionEntryPanelScript newSessionEntryPanelScript = newEntry.GetComponent<SessionEntryPanelScript>();
            if (newSessionEntryPanelScript != null)
            {
                newSessionEntryPanelScript.IsSelected = index == selectedIndex;
                newSessionEntryPanelScript.UpdateSelectionState();

                newSessionEntryPanelScript.EntryIndex = index;

                newSessionEntryPanelScript.MapText.text = "Map: " + sessionInfo.MapName;
                newSessionEntryPanelScript.OpponentHandleText.text = "Opponent: " + sessionInfo.OpponentHandle;
                newSessionEntryPanelScript.TeamText.text = sessionInfo.SuggestedTeam == MessageLibraryUtitlity.SessionTeam.blue ? "Team: Blue" : "Team: Orange";

                int minutes = sessionInfo.DurationSeconds / 60;
                string minutesText = minutes.ToString("D2");                
                int seconds = sessionInfo.DurationSeconds % 60;
                string secondsText = seconds.ToString("D2");
                newSessionEntryPanelScript.DurationText.text = "Time: " + minutesText + ":" + secondsText;
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
        gameLogicScriptComponent.JoinSession(SelectedEntryIndex);

        sessionListScrollRect.gameObject.SetActive(false);
        sessionJoinButton.gameObject.SetActive(false);
        NoSessionsText.gameObject.SetActive(false);
        WaitingCursorRectTransform.gameObject.SetActive(true);

        doJoinSessionHandling = true;
    }

    public void OnClickRefreshButton()
    {
        PrepareSessionListRefresh();
    }
}
