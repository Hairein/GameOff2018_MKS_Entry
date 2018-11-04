using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    private bool showSelectionRectangle = true;
    public Texture2D RectSelectionTexture;
    private Rect selectionRect = new Rect(100, 100, 800, 600);
    private Color selectionRectColor = new Color(1.0f, 1.0f, 0.75f, 0.25f);

    public GameObject[] SelectedEntities;
    private bool mapNavigationOrderIssued = false;

    public Vector2 MapDimensions = new Vector2(48.0f, 48.0f);

    public int TeamNumber = 1;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }
        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();
    }

    void Update()
    {
        if (gameLogicScriptComponent == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        Vector3 mousePosition = Input.mousePosition;

        if (Input.GetMouseButtonUp(1))
        {
            if (mapNavigationOrderIssued)
            {
                mapNavigationOrderIssued = false;
            }
            else
            {
                Vector3 clickHitPosition = Vector3.zero;
                if (GetScreenHitInWorld(mousePosition, out clickHitPosition))
                {
                    SetNavigationTarget(clickHitPosition);
                }
            }
        }

    }

    private void OnGUI()
    {
        //if (showSelectionRectangle && RectSelectionTexture != null)
        //{
        //    GUI.color = selectionRectColor;
        //    GUI.DrawTexture(selectionRect, RectSelectionTexture);
        //    GUI.color = Color.white;
        //}
    }

    private bool GetScreenHitInWorld(Vector3 screenPosition, out Vector3 hitPosition)
    {
        hitPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1000.0f))
        {
            return false;
        }

        hitPosition = hit.point;
        return true;
    }

    private Vector3 CalculateWorldPositionFromMapOffsets(Vector2 offset)
    {
        float xOffset = (offset.x - 0.5f) * MapDimensions.x;
        float zOffset = (offset.y - 0.5f) * MapDimensions.y;

        Vector3 position = new Vector3(xOffset, 0.0f, zOffset);
        return position;
    }

    private void SetNavigationTarget(Vector3 target)
    {
        if(SelectedEntities.Length == 0)
        {
            return;
        }

        foreach (GameObject navGameObject in SelectedEntities)
        {
            NavMeshAgent agent = navGameObject.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.destination = target;
                agent.isStopped = false;
            }
        }
    }

    public void HandleMapNavigation(Vector2 offset)
    {
        mapNavigationOrderIssued = true;

        Vector3 worldOffset = CalculateWorldPositionFromMapOffsets(offset);
        Vector3 newTargetPosition = new Vector3(worldOffset.x, 0.0f, worldOffset.z);
        SetNavigationTarget(newTargetPosition);
    }

    public Vector3 GetCameraStartPosition()
    {
        GameObject targetGameObject;
        if (TeamNumber == 1)
        {
            targetGameObject = GameObject.Find("Team1SpawnSetup");
        }
        else
        {
            targetGameObject = GameObject.Find("Team2SpawnSetup");
        }

        if (targetGameObject == null)
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }

        Vector3 position = targetGameObject.transform.position;
        position.y = 0.0f;
        return position;
    }
}
