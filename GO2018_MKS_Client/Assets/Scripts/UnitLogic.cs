using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitLogic : MonoBehaviour
{
    public int TeamNumber = 0;
    public bool IsSelected = false;
    
    public GameObject SelectionEffect;

    public float FoodResourceCount = 0.0f;
    public float TechResourceCount = 0.0f;

    public UnitLogic[] influencingOpponentUnits;

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

    void OnTriggerEnter(Collider other)
    {
        UnitLogic unitLogicScript = other.gameObject.GetComponent<UnitLogic>();
        if (unitLogicScript != null && TeamNumber != unitLogicScript.TeamNumber)
        {
            List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
            foreach (UnitLogic unit in influencingOpponentUnits)
            {
                listOfUnitsInfluencing.Add(unit);
            }

            listOfUnitsInfluencing.Add(unitLogicScript);

            influencingOpponentUnits = listOfUnitsInfluencing.ToArray();
        }
    }

    void OnTriggerExit(Collider other)
    {
        UnitLogic unitLogicScript = other.gameObject.GetComponent<UnitLogic>();
        if (unitLogicScript != null && TeamNumber != unitLogicScript.TeamNumber)
        {
            List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
            foreach (UnitLogic unit in influencingOpponentUnits)
            {
                if (unitLogicScript == unit)
                {
                    continue;
                }

                listOfUnitsInfluencing.Add(unit);
            }

            influencingOpponentUnits = listOfUnitsInfluencing.ToArray();
        }
    }
}
