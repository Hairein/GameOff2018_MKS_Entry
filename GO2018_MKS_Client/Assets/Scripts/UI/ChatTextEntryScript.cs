using UnityEngine;
using UnityEngine.UI;
using System;

public class ChatTextEntryScript : MonoBehaviour
{
    public Text ChatText;
    public Text ChatShadowText;

    public Color OwnChatColor;
    public Color OtherChatColor;

    public bool IsOwnChat = false;

    public string Text = string.Empty;

    public bool IsAging = true;
    public float Age = 0.0f;

    void Start()
    {
        UpdateProperties();
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (IsAging)
        {
            Age += deltaTime;
        }
    }

    public void UpdateProperties()
    {
        if (ChatText != null)
        {
            ChatText.text = Text;
            ChatText.color = IsOwnChat ? OwnChatColor : OtherChatColor;
        }

        if (ChatShadowText != null)
        {
            ChatShadowText.text = Text;
        }
    }
}
