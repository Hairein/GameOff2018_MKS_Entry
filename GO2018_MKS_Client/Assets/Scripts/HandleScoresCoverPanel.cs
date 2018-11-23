using UnityEngine;
using UnityEngine.UI;

public class HandleScoresCoverPanel : MonoBehaviour
{
    private IngameSceneLogicScript ingameSceneLogicScript = null;

    public Text PlayerScore;
    public Text OpponentScore;

    void Update()
    {
        if (ingameSceneLogicScript == null)
        {
            GameObject ingameLogic = GameObject.Find("IngameLogic");
            if (ingameLogic != null)
            {
                ingameSceneLogicScript = ingameLogic.GetComponent<IngameSceneLogicScript>();
            }
        }

        if(ingameSceneLogicScript != null)
        {
            PlayerScore.text = ingameSceneLogicScript.PlayerHandle + " " + ingameSceneLogicScript.PlayerScore.ToString("D5");

            OpponentScore.text = ingameSceneLogicScript.OpponentHandle + " " + ingameSceneLogicScript.OpponentScore.ToString("D5");
        }
    }
}
