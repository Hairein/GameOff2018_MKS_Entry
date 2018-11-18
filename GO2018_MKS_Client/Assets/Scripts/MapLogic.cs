using UnityEngine;
using UnityEngine.EventSystems;

public class MapLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject IngameCamera;
    private IngameCameraScript ingameCameraScript;

    public GameObject IngameSceneLogic;
    private IngameSceneLogicScript ingameSceneLogicScript;

    void Start()
    {
        if(IngameCamera == null || IngameSceneLogic == null)
        {
            return;
        }
        ingameCameraScript = IngameCamera.GetComponent<IngameCameraScript>();
        ingameSceneLogicScript = IngameSceneLogic.GetComponent<IngameSceneLogicScript>();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (ingameSceneLogicScript == null || ingameCameraScript == null ||  ingameSceneLogicScript.IsShowingSelectionRectangle || ingameCameraScript.IsDragging)
        {
            return;
        }

        ingameSceneLogicScript.IgnoreIngamePointerInput = true;

        Vector2 mousePosition = pointerEventData.position;
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();

        float xoffset = (mousePosition.x - rectTransform.offsetMin.x) / (rectTransform.offsetMax.x - rectTransform.offsetMin.x);
        float zoffset = (mousePosition.y - rectTransform.offsetMin.y) / (rectTransform.offsetMax.y - rectTransform.offsetMin.y);
        Vector2 offset = new Vector2(xoffset, zoffset);

        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            // Handle map centering
            ingameCameraScript.HandleMapRecenter(offset);
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            // Handle selected entities navigation to target
            ingameSceneLogicScript.HandleMapNavigation(offset);
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (ingameSceneLogicScript == null)
        {
            return;
        }

        ingameSceneLogicScript.IgnoreIngamePointerInput = false;
    }
}