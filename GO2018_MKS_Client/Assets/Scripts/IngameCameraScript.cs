using UnityEngine;
using UnityEngine.AI;

public class IngameCameraScript : MonoBehaviour
{
    public Vector3 CameraStartPosition = Vector3.zero;   
    public Vector3 ViewOffset = new Vector3(0.0f, 20.0f, -10.5f);

    public bool IsDragging = false;
    public float DragRelayFactor = 0.1f;
    private Vector3 dragStartPoint = Vector3.zero;
    private Vector3 dragStartCameraPosition = Vector3.zero;

    public GameObject IngameSceneLogic;
    private IngameSceneLogicScript ingameSceneLogicScript;

    void Start()
    {
        if (IngameSceneLogic == null)
        {
            return;
        }
        ingameSceneLogicScript = IngameSceneLogic.GetComponent<IngameSceneLogicScript>();

        Camera.main.transform.position = ingameSceneLogicScript.GetCameraStartPosition() + ViewOffset;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        Vector3 mousePosition = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
        }

        if (Input.GetMouseButtonUp(1))
        {
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (!IsDragging)
            {
                IsDragging = true;

                dragStartPoint = mousePosition;
                dragStartCameraPosition = Camera.main.transform.position;
            }
        }
        else if (Input.GetMouseButtonUp(2))
        {
            if (IsDragging)
            {
                IsDragging = false;
            }
        }

        // Handle mouse panning
        if (IsDragging)
        {
            Vector3 dragOffset = dragStartPoint - mousePosition;
            float xOffset = dragStartCameraPosition.x + (dragOffset.x * DragRelayFactor);
            float yOffset = dragStartCameraPosition.y;
            float zOffset = dragStartCameraPosition.z + (dragOffset.y * DragRelayFactor);

            Vector3 dragOffsetCameraPosition = new Vector3(xOffset, yOffset, zOffset);
            Camera.main.transform.position = dragOffsetCameraPosition;
        }
    }

    private Vector3 CalculateWorldPositionFromMapOffsets(Vector2 offset)
    {
        float xOffset = (offset.x - 0.5f) * ingameSceneLogicScript.MapDimensions.x;
        float zOffset = (offset.y - 0.5f) * ingameSceneLogicScript.MapDimensions.y;

        Vector3 position = new Vector3(xOffset, 0.0f, zOffset);
        return position;
    }

    public void HandleMapRecenter(Vector2 offset)
    {
        Vector3 cameraOffset = CalculateWorldPositionFromMapOffsets(offset);
        cameraOffset.x += ViewOffset.x;
        cameraOffset.z += ViewOffset.z;

        Vector3 newCameraPosition = new Vector3(cameraOffset.x, ViewOffset.y, cameraOffset.z);
        Camera.main.transform.position = newCameraPosition;
    }
}
