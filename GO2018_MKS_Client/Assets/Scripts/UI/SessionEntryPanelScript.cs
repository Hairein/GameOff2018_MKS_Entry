using UnityEngine;
using UnityEngine.UI;

public class SessionEntryPanelScript : MonoBehaviour
{
    public JoinSessionSceneLogicScript JoinSessionSceneLogicScript;
    public int EntryIndex = -1;

    public RawImage Mark;

    public bool IsSelected = false;

    public Text MapText;
    public Text OpponentHandleText;
    public Text TeamText;
    public Text DurationText;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnClicked()
    {
        if (JoinSessionSceneLogicScript != null)
        {
            JoinSessionSceneLogicScript.EntrySelected(EntryIndex);
        }
    }

    public void UpdateSelectionState()
    {
        Mark.gameObject.SetActive(IsSelected);
    }
}
