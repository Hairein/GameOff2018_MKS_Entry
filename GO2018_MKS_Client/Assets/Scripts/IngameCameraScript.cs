using UnityEngine;
using UnityEngine.AI;

public class IngameCameraScript : MonoBehaviour
{
    public GameObject followedEntity;

    public Vector3 viewOffset = new Vector3(0.0f, 17.5f, -10.25f);

    void Update()
    {
        if (followedEntity != null)
        {
            Vector3 newPosition = new Vector3(followedEntity.transform.position.x + viewOffset.x, followedEntity.transform.position.y + viewOffset.y, followedEntity.transform.position.z + viewOffset.z);
            this.gameObject.transform.position = newPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Pressed left click.");
        }
        if (Input.GetMouseButtonUp(1))
        {
            //Debug.Log("Pressed right click.");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                print("Hit something!");

                NavMeshAgent agent = followedEntity.GetComponent<NavMeshAgent>();
                if(agent != null)
                {
                    agent.destination = hit.point;
                    agent.isStopped = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            //Debug.Log("Pressed middle click.");
        }
    }
}
