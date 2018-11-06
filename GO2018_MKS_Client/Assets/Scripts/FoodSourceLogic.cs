using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSourceLogic : MonoBehaviour
{
    public float ResourceCount = 5000.0f;
    public float MaxResourceCount = 5000.0f;

    public TextMesh ResourceUnitsText;

    public UnitLogic[] influencingUnits;

    public float tapRatePerSec = 25.0f;

    void Start()
    {
    }

    void Update()
    {
        if (ResourceUnitsText != null)
        {
            int flatResourceCount = (int)ResourceCount;
            ResourceUnitsText.text = flatResourceCount.ToString();

            ResourceUnitsText.transform.rotation = Camera.main.transform.rotation;
        }

        if(influencingUnits.Length == 0 || ResourceCount <= 0.0f)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        float tapRateThisFrame = tapRatePerSec * deltaTime;

        if(ResourceCount - tapRateThisFrame < 0.0f)
        {
            tapRateThisFrame = ResourceCount;
        }

        float tappedValue = tapRateThisFrame / influencingUnits.Length;

        foreach(UnitLogic unit in influencingUnits)
        {
            if(unit.FoodResourceCount < unit.MaxFoodResourceCount)
            {
                float newFoodValue = tappedValue;
                if(unit.FoodResourceCount +  newFoodValue > unit.MaxFoodResourceCount)
                {
                    newFoodValue = unit.MaxFoodResourceCount - unit.FoodResourceCount;
                }

                unit.FoodResourceCount += newFoodValue;
                ResourceCount -= newFoodValue;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        UnitLogic unitLogicScript = other.gameObject.GetComponent<UnitLogic>();
        if(unitLogicScript != null)
        {
            List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
            foreach(UnitLogic unit in influencingUnits)
            {
                listOfUnitsInfluencing.Add(unit);
            }

            listOfUnitsInfluencing.Add(unitLogicScript);

            influencingUnits = listOfUnitsInfluencing.ToArray();
        }
    }

    void OnTriggerExit(Collider other)
    {
        UnitLogic unitLogicScript = other.gameObject.GetComponent<UnitLogic>();
        if (unitLogicScript != null)
        {
            List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
            foreach (UnitLogic unit in influencingUnits)
            {
                if(unitLogicScript == unit)
                {
                    continue;
                }

                listOfUnitsInfluencing.Add(unit);
            }

            influencingUnits = listOfUnitsInfluencing.ToArray();
        }
    }
}
