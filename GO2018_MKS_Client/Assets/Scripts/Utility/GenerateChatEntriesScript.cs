using UnityEngine;

public class GenerateChatEntriesScript : MonoBehaviour
{
    private bool runOnce = true;

    public ChatTextEntryManagerScript RunnerScript;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
    }

    void Update()
    {
        if(runOnce)
        {
            if(RunnerScript != null)
            {
                RunnerScript.AddEntry("Testing line 1", false);
                RunnerScript.AddEntry("Testing line 2", false);
                RunnerScript.AddEntry("Testing line 3", true);
                RunnerScript.AddEntry("Testing line 4", false);
                RunnerScript.AddEntry("Testing line 5", true);
            }

            runOnce = false;
        }
    }
}
