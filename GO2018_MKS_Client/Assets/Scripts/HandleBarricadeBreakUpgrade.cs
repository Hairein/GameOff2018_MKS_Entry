using UnityEngine;
using UnityEngine.EventSystems;


public class HandleBarricadeBreakUpgrade : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject IngameSceneLogic = null;
    private IngameSceneLogicScript ingameSceneLogicScript = null;

    void Start()
    {
        if (IngameSceneLogic == null)
        {
            return;
        }
        ingameSceneLogicScript = IngameSceneLogic.GetComponent<IngameSceneLogicScript>();
    }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (ingameSceneLogicScript == null || ingameSceneLogicScript.IsShowingSelectionRectangle)
        {
            return;
        }

        ingameSceneLogicScript.IgnoreIngamePointerInput = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (ingameSceneLogicScript == null)
        {
            return;
        }

        ingameSceneLogicScript.UpgradeBarricadeBreak();

        ingameSceneLogicScript.IgnoreIngamePointerInput = false;
    }
}
