using GO2018_MKS_MessageLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatTextEntryManagerScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;
    private IngameSceneLogicScript ingameSceneLogicScript = null;

    public List<GameObject> chatTexts = new List<GameObject>();

    public GameObject ChatTextsParent;
    public GameObject ChatTextPrefab;

    public Vector2 StartPosition;
    public Vector2 LineOffset;

    public float ChatTextMaxAge = 15.0f;

    public InputField ChatInput;

    void Start()
    {
    }

    void Update()
    {
        if (gameLogicScriptComponent == null)
        {
            GameObject gameLogic = GameObject.Find("GameLogic");
            if (gameLogic == null)
            {
                return;
            }
            gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();
        }

        if (ingameSceneLogicScript == null)
        {
            GameObject ingameLogic = GameObject.Find("IngameLogic");
            if (ingameLogic == null)
            {
                return;
            }
            ingameSceneLogicScript = ingameLogic.GetComponent<IngameSceneLogicScript>();
        }

        SessionChatAnswerMessage[] incomingChatMessages = gameLogicScriptComponent.ReadPendingSessionChatMessages();
        if(incomingChatMessages != null)
        {
            for(int index = 0;  index < incomingChatMessages.Length; index++)
            {
                AddEntry(incomingChatMessages[index].Message, false);
            }
        }

        UpdateEntries();
        PlaceEntries();

        // Ensure key presses are only used for chat text now
        if (ChatInput != null && ingameSceneLogicScript != null)
        {
            if (ChatInput.isFocused && !ingameSceneLogicScript.IgnoreIngameKeyInput)
            {
                ingameSceneLogicScript.IgnoreIngameKeyInput = true;
            }
            else if (!ChatInput.isFocused && ingameSceneLogicScript.IgnoreIngameKeyInput)
            {
                ingameSceneLogicScript.IgnoreIngameKeyInput = false;
            }
        }
    }

    public void OnNewLine()
    {
        if (ChatInput == null || gameLogicScriptComponent == null)
        {
            return;
        }

        string message = ChatInput.text.Trim();
        if (string.IsNullOrWhiteSpace(message))
        {
            ClearInput();
            return;
        }

        gameLogicScriptComponent.SendChatMessage(message);

        AddEntry(message, true);

        ClearInput();
    }

    public void AddEntry(string newEntry, bool isOwnChat)
    {
        GameObject newEntryObject = Instantiate(ChatTextPrefab, ChatTextsParent.transform);
        if(newEntryObject != null)
        {
            ChatTextEntryScript script = newEntryObject.GetComponent<ChatTextEntryScript>();
            if(script != null)
            {
                script.Text = newEntry;
                script.IsOwnChat = isOwnChat;
                script.UpdateProperties();
            }

            chatTexts.Add(newEntryObject);
        }
    }

    private void UpdateEntries()
    {
        if(chatTexts.Count == 0)
        {
            return;
        }

        GameObject[] arrayOfEntries = chatTexts.ToArray();
        chatTexts.Clear();
        for (int index = 0; index < arrayOfEntries.Length; index++)
        {
            ChatTextEntryScript script = arrayOfEntries[index].GetComponent<ChatTextEntryScript>();
            if (script == null)
            {
                continue;
            }

            if(script.Age <= ChatTextMaxAge)
            {
                chatTexts.Add(arrayOfEntries[index]);
            }
            else
            {
                Destroy(arrayOfEntries[index]);
            }
        }
    }

    private void PlaceEntries()
    {
        if(chatTexts.Count == 0)
        {
            return;
        }

        for (int index = 0; index < chatTexts.Count; index++)
        {
            int offsetIndex = (chatTexts.Count - 1) - index;

            ChatTextEntryScript script = chatTexts[index].GetComponent<ChatTextEntryScript>();
            if (script == null)
            {
                continue;
            }

            RectTransform transform = chatTexts[index].GetComponent<RectTransform>();
            if(transform == null)
            {
                continue;
            }

            Vector3 position = transform.localPosition;
            position.Set(StartPosition.x + (offsetIndex * LineOffset.x), StartPosition.y + (offsetIndex * LineOffset.y), 0.0f);
            transform.localPosition = position;
        }
    }

    public void ClearInput()
    {
        ChatInput.text = string.Empty;

        // ChatInput.DeactivateInputField();
    }
}
