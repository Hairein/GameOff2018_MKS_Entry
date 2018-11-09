using UnityEngine;
using UnityEngine.EventSystems;


public class HandleTechCollectPlusUpgrade : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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

        ingameSceneLogicScript.IgnorePointerInput = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (ingameSceneLogicScript == null)
        {
            return;
        }

        ingameSceneLogicScript.UpgradeTechCollectPlus();

        ingameSceneLogicScript.IgnorePointerInput = false;
    }
}
