using UnityEngine;
using UnityEngine.AI;

public class UnitLogic : MonoBehaviour
{
    public int TeamNumber = 0;
    public bool IsSelected = false;
    
    public GameObject SelectionEffect;

    public float FoodResourceCount = 0.0f;
    public float TechResourceCount = 0.0f;

    void Start()
    {
        SetSelection(IsSelected);
    }

    void Update()
    {
    }

    public void SetSelection(bool flag)
    {
        IsSelected = flag;

        if (SelectionEffect != null)
        {
            SelectionEffect.SetActive(IsSelected);
        }
    }

    public void UpgradeSpeed(float increaseSpeedFactor)
    {
        NavMeshAgent navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        if(navMeshAgent != null)
        {
            navMeshAgent.speed *= increaseSpeedFactor;
        }
    }
}
