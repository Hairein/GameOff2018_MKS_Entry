using UnityEngine;
using UnityEngine.EventSystems;

public class MapLogic : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(ingameCameraScript == null || IngameSceneLogic == null)
        {
            return;
        }

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
            ingameSceneLogicScript.HandleMapNavigation(offset);
        }
    }
}