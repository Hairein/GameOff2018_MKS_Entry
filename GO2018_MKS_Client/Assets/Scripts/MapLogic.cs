using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

public class MapLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject IngameCamera;
    private IngameCameraScript ingameCameraScript;

    public GameObject IngameSceneLogic;
    private IngameSceneLogicScript ingameSceneLogicScript = null;

    private GameLogicScript gameLogicScriptComponent = null;

    public GameObject map;
    private Texture2D mapTexture;

    public Color MapFillColor;

    public Texture2D FoodResourceTexture;
    public Texture2D TechResourceTexture;
    public Texture2D BarricadeResourceTexture;

    public RawImage BlueBreederIcon;
    public RawImage[] BlueDroneIcons;
    public RawImage OrangeBreederIcon;
    public RawImage[] OrangeDroneIcons;

    public float HalfMapWidthOffset = 128.0f; 
    public float HalfMapHeightOffset = 128.0f;

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

        if (ingameSceneLogicScript)
        {
            ingameSceneLogicScript.RedrawMap = true;
        }
    }

    void Update()
    {
        if (ingameSceneLogicScript == null)
        {
            return;
        }

        if (gameLogicScriptComponent == null)
        {
            GameObject gameLogic = GameObject.Find("GameLogic");
            if (gameLogic != null)
            {
                gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();
            }
            else
            {
                return;
            }
        }

        if(ingameSceneLogicScript.RedrawMap == true)
        {
            PrepareMapDisplay();

            ingameSceneLogicScript.RedrawMap = false;
        }

        HandleIcons();
    }

    private void PrepareMapDisplay()
    {
        if (mapTexture != null)
        {
            for (int mapY = 0; mapY < mapTexture.height; mapY++)
            {
                for (int mapX = 0; mapX < mapTexture.width; mapX++)
                {
                    mapTexture.SetPixel(mapX, mapY, MapFillColor);
                }
            }
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

        mapTexture.Apply();
    }

    private void HandleIcons()
    {
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
        if(rectTransform == null)
        {
            return;
        }

        float halfMapDimWidth = ingameSceneLogicScript.MapDimensions.x / 2.0f;
        float halfMapDimHeight = ingameSceneLogicScript.MapDimensions.y / 2.0f;

        float halfMapWidth = rectTransform.rect.width / 2.0f;
        float halfMapHeight = rectTransform.rect.height / 2.0f;

        if (ingameSceneLogicScript.TeamNumber == 1)
        {
            // Reposition Breeders
            Vector3 blueBreederPosition = new Vector3((ingameSceneLogicScript.TeamBreeder.transform.position.x / halfMapDimWidth) * halfMapWidth, 
                (ingameSceneLogicScript.TeamBreeder.transform.position.z / halfMapDimHeight) * halfMapHeight,
                0.0f);
            blueBreederPosition.x += HalfMapWidthOffset;
            blueBreederPosition.y += HalfMapHeightOffset;
            BlueBreederIcon.rectTransform.position = blueBreederPosition;

            Vector3 orangeBreederPosition = new Vector3((ingameSceneLogicScript.OpponentTeamBreeder.transform.position.x / halfMapDimWidth) * halfMapWidth, 
                (ingameSceneLogicScript.OpponentTeamBreeder.transform.position.z / halfMapDimHeight) * halfMapHeight,
                0.0f);
            orangeBreederPosition.x += HalfMapWidthOffset;
            orangeBreederPosition.y += HalfMapHeightOffset;
            OrangeBreederIcon.rectTransform.position = orangeBreederPosition;

            // Handle drones
            for (int indexDrones = 0; indexDrones <= 7; indexDrones++)
            {

                if (indexDrones < ingameSceneLogicScript.TeamDrones.Length)
                {
                    Vector3 blueDronePosition = new Vector3((ingameSceneLogicScript.TeamDrones[indexDrones].transform.position.x / halfMapDimWidth) * halfMapWidth, 
                        (ingameSceneLogicScript.TeamDrones[indexDrones].transform.position.z / halfMapDimHeight) * halfMapHeight,
                        0.0f);
                    blueDronePosition.x += HalfMapWidthOffset;
                    blueDronePosition.y += HalfMapHeightOffset;
                    BlueDroneIcons[indexDrones].rectTransform.position = blueDronePosition;

                    if (!BlueDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        BlueDroneIcons[indexDrones].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if(BlueDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        BlueDroneIcons[indexDrones].gameObject.SetActive(false);
                    }
                }

                if (indexDrones < ingameSceneLogicScript.OpponentTeamDrones.Length)
                {
                    Vector3 orangeDronePosition = new Vector3((ingameSceneLogicScript.OpponentTeamDrones[indexDrones].transform.position.x / halfMapDimWidth) * halfMapWidth, 
                        (ingameSceneLogicScript.OpponentTeamDrones[indexDrones].transform.position.z / halfMapDimHeight) * halfMapHeight,
                        0.0f);
                    orangeDronePosition.x += HalfMapWidthOffset;
                    orangeDronePosition.y += HalfMapHeightOffset;
                    OrangeDroneIcons[indexDrones].rectTransform.position = orangeDronePosition;

                    if (!OrangeDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        OrangeDroneIcons[indexDrones].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if(OrangeDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        OrangeDroneIcons[indexDrones].gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // Reposition Breeders
            Vector3 orangeBreederPosition = new Vector3((ingameSceneLogicScript.TeamBreeder.transform.position.x / halfMapDimWidth) * halfMapWidth, 
                (ingameSceneLogicScript.TeamBreeder.transform.position.z / halfMapDimHeight) * halfMapHeight, 
                0.0f);
            orangeBreederPosition.x += HalfMapWidthOffset;
            orangeBreederPosition.y += HalfMapHeightOffset;
            OrangeBreederIcon.rectTransform.position = orangeBreederPosition;

            Vector3 blueBreederPosition = new Vector3((ingameSceneLogicScript.OpponentTeamBreeder.transform.position.x / halfMapDimWidth) * halfMapWidth, 
                (ingameSceneLogicScript.OpponentTeamBreeder.transform.position.z / halfMapDimHeight) * halfMapHeight,
                0.0f);
            blueBreederPosition.x += HalfMapWidthOffset;
            blueBreederPosition.y += HalfMapHeightOffset;
            BlueBreederIcon.rectTransform.position = blueBreederPosition;

            // Handle drones
            for (int indexDrones = 0; indexDrones <= 7; indexDrones++)
            {
                if (indexDrones < ingameSceneLogicScript.TeamDrones.Length)
                {
                    Vector3 orangeDronePosition = new Vector3((ingameSceneLogicScript.TeamDrones[indexDrones].transform.position.x / halfMapDimWidth) * halfMapWidth, 
                        (ingameSceneLogicScript.TeamDrones[indexDrones].transform.position.z / halfMapDimHeight) * halfMapHeight,
                        0.0f);
                    orangeDronePosition.x += HalfMapWidthOffset;
                    orangeDronePosition.y += HalfMapHeightOffset;
                    OrangeDroneIcons[indexDrones].rectTransform.position = orangeDronePosition;

                    if (!OrangeDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        OrangeDroneIcons[indexDrones].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (OrangeDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        OrangeDroneIcons[indexDrones].gameObject.SetActive(false);
                    }
                }

                if (indexDrones < ingameSceneLogicScript.OpponentTeamDrones.Length)
                {
                    Vector3 blueDronePosition = new Vector3((ingameSceneLogicScript.OpponentTeamDrones[indexDrones].transform.position.x / halfMapDimWidth) * halfMapWidth, 
                        (ingameSceneLogicScript.OpponentTeamDrones[indexDrones].transform.position.z / halfMapDimHeight) * halfMapHeight,
                        0.0f);
                    blueDronePosition.x += HalfMapWidthOffset;
                    blueDronePosition.y += HalfMapHeightOffset;
                    BlueDroneIcons[indexDrones].rectTransform.position = blueDronePosition;

                    if (!BlueDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        BlueDroneIcons[indexDrones].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (BlueDroneIcons[indexDrones].gameObject.activeSelf)
                    {
                        BlueDroneIcons[indexDrones].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    /*void OnGUI()
    {
        if(ingameSceneLogicScript == null || gameLogicScriptComponent == null || gameLogicScriptComponent.SessionState != SessionState.ingame)
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
            if (opponentBreeder != null && opponentBreeder.activeSelf)
            {
                x = ((opponentBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
                y = ((opponentBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

                DrawTextureInMap(BlueBreederTexture, (int)x, (int)y);
            }
        }
        else
        {
            GameObject opponentBreeder = ingameSceneLogicScript.OpponentTeamBreeder;
            if (opponentBreeder != null && opponentBreeder.activeSelf)
            {
                x = ((opponentBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
                y = ((opponentBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

                DrawTextureInMap(OrangeBreederTexture, (int)x, (int)y);
            }
        }

        if (ingameSceneLogicScript.TeamNumber == 1)
        {
            GameObject teamBreeder = ingameSceneLogicScript.TeamBreeder;
            if (teamBreeder != null && teamBreeder.activeSelf)
            {
                x = ((teamBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
                y = ((teamBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

                DrawTextureInMap(BlueBreederTexture, (int)x, (int)y);
            }
        }
        else
        {
            GameObject teamBreeder = ingameSceneLogicScript.TeamBreeder;
            if (teamBreeder != null && teamBreeder.activeSelf)
            {
                x = ((teamBreeder.transform.position.x + halfMapDims.x) / mapDims.x) * mapTexture.width;
                y = ((teamBreeder.transform.position.z + halfMapDims.y) / mapDims.y) * mapTexture.height;

                DrawTextureInMap(OrangeBreederTexture, (int)x, (int)y);
            }
        }

        mapTexture.Apply();
    }*/

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