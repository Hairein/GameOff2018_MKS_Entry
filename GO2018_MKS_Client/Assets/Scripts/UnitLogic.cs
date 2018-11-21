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

    public UnitLogic[] InfluencingTeamUnits;
    public UnitLogic[] InfluencingOpponentUnits;

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
        if (unitLogicScript != null)
        {
            if (TeamNumber == unitLogicScript.TeamNumber)
            {
                List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
                foreach (UnitLogic unit in InfluencingTeamUnits)
                {
                    listOfUnitsInfluencing.Add(unit);
                }

                if(!listOfUnitsInfluencing.Contains(unitLogicScript))
                {
                    listOfUnitsInfluencing.Add(unitLogicScript);
                }

                InfluencingTeamUnits = listOfUnitsInfluencing.ToArray();
            }
            else
            {
                List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
                foreach (UnitLogic unit in InfluencingOpponentUnits)
                {
                    listOfUnitsInfluencing.Add(unit);
                }

                if(!listOfUnitsInfluencing.Contains(unitLogicScript))
                {
                    listOfUnitsInfluencing.Add(unitLogicScript);
                }

                InfluencingOpponentUnits = listOfUnitsInfluencing.ToArray();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        UnitLogic unitLogicScript = other.gameObject.GetComponent<UnitLogic>();
        if (unitLogicScript != null)
        {
            if (TeamNumber == unitLogicScript.TeamNumber)
            {
                List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
                foreach (UnitLogic unit in InfluencingTeamUnits)
                {
                    if (unitLogicScript == unit)
                    {
                        continue;
                    }

                    listOfUnitsInfluencing.Add(unit);
                }

                InfluencingTeamUnits = listOfUnitsInfluencing.ToArray();
            }
            else
            {
                List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
                foreach (UnitLogic unit in InfluencingOpponentUnits)
                {
                    if (unitLogicScript == unit)
                    {
                        continue;
                    }

                    listOfUnitsInfluencing.Add(unit);
                }

                InfluencingOpponentUnits = listOfUnitsInfluencing.ToArray();
            }
        }
    }
}
