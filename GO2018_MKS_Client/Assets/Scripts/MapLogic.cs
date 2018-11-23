using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject IngameCamera;
    private IngameCameraScript ingameCameraScript;

    public GameObject IngameSceneLogic;
    private IngameSceneLogicScript ingameSceneLogicScript;

    public GameObject map;
    private Texture2D mapTexture;

    public Texture2D BlueBreederTexture;
    public Texture2D BlueDroneTexture;
    public Texture2D OrangeBreederTexture;
    public Texture2D OrangeDroneTexture;
    public Texture2D FoodResourceTexture;
    public Texture2D TechResourceTexture;
    public Texture2D BarricadeResourceTexture;

    public Color MapFillColor;


    void Start()
    {
        if(IngameCamera == null || IngameSceneLogic == null)
        {
            return;
        }
        ingameCameraScript = IngameCamera.GetComponent<IngameCameraScript>();
        ingameSceneLogicScript = IngameSceneLogic.GetComponent<IngameSceneLogicScript>();

        map = GameObject.Find("MapRawImage");
        mapTexture = (Texture2D)map.GetComponent<RawImage>().mainTexture;
    }

    void OnGUI()
    {
        if(ingameSceneLogicScript == null)
        {
            return;
        }

        if (mapTexture != null)
        {
            for (int mapY = 0; mapY < mapTexture.height; mapY++)
            {
                for (int mapX = 0; mapX < mapTexture.width; mapX++)
                {
                    mapTexture.SetPixel(mapX, mapY, MapFillColor);
                }
            }

            mapTexture.Apply();
        }

        float x;
        float y;

        Vector2 mapDims = ingameSceneLogicScript.MapDimensions;
        Vector2 halfMapDims = new Vector2(mapDims.x / 2.0f, mapDims.y / 2.0f);

        foreach (GameObject barricade in ingameSceneLogicScript.Barricades)
        {
            x = ((barricade.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((barricade.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(BarricadeResourceTexture, (int)x, (int)y);
        }

        foreach (GameObject tech in ingameSceneLogicScript.TechSources)
        {
            x = ((tech.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((tech.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(TechResourceTexture, (int)x, (int)y);
        }

        foreach (GameObject food in ingameSceneLogicScript.FoodSources)
        {
            x = ((food.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((food.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(FoodResourceTexture, (int)x, (int)y);
        }

        foreach (GameObject drone in ingameSceneLogicScript.OpponentTeamDrones)
        {
            x = ((drone.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((drone.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            if (ingameSceneLogicScript.OpponentTeamNumber == 1)
            {
                DrawTextureInMap(BlueDroneTexture, (int)x, (int)y);
            }
            else
            {
                DrawTextureInMap(OrangeDroneTexture, (int)x, (int)y);
            }
        }

        foreach (GameObject drone in ingameSceneLogicScript.TeamDrones)
        {
            x = ((drone.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((drone.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            if (ingameSceneLogicScript.TeamNumber == 1)
            {
                DrawTextureInMap(BlueDroneTexture, (int)x, (int)y);
            }
            else
            {
                DrawTextureInMap(OrangeDroneTexture, (int)x, (int)y);
            }
        }

        if (ingameSceneLogicScript.OpponentTeamNumber == 1)
        {
            GameObject opponentBreeder = ingameSceneLogicScript.OpponentTeamBreeder;

            x = ((opponentBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((opponentBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(BlueBreederTexture, (int)x, (int)y);
        }
        else
        {
            GameObject opponentBreeder = ingameSceneLogicScript.OpponentTeamBreeder;

            x = ((opponentBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((opponentBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(OrangeBreederTexture, (int)x, (int)y);
        }

        if (ingameSceneLogicScript.TeamNumber == 1)
        {
            GameObject teamBreeder = ingameSceneLogicScript.TeamBreeder;

            x = ((teamBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((teamBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(BlueBreederTexture, (int)x, (int)y);
        }
        else
        {
            GameObject teamBreeder = ingameSceneLogicScript.TeamBreeder;

            x = ((teamBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
            y = ((teamBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

            DrawTextureInMap(OrangeBreederTexture, (int)x, (int)y);
        }
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

    private void DrawTextureInMap(Texture2D source, int x, int y)
    {
        if(source == null)
        {
            return;
        }

        for (int srcY = 0; srcY < source.height; srcY++)
        {
            for (int srcX = 0; srcX < source.width; srcX++)
            {
                Color srcPixel = source.GetPixel(srcX, srcY);

                int targetX = x + srcX - (source.width / 2);    
                int targetY = y + srcY - (source.height / 2);

                 mapTexture.SetPixel(targetX, targetY, srcPixel);
            }
        }

        mapTexture.Apply();
    }
}