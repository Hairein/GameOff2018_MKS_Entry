using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSourceLogic : MonoBehaviour
{
    private IngameSceneLogicScript ingameLogicScript = null;

    public float ResourceCount = 5000.0f;
    public float MaxResourceCount = 5000.0f;

    public TextMesh ResourceUnitsText;

    public UnitLogic[] influencingUnits;

    public float tapRatePerSec = 25.0f;

    public GameObject foodParticleSystem;

    void Start()
    {
        GameObject ingameLogic = GameObject.Find("IngameLogic");
        if (ingameLogic == null)
        {
            return;
        }

        ingameLogicScript = ingameLogic.GetComponent<IngameSceneLogicScript>();
    }

    void Update()
    {
        bool didReduceResources = false;

        if (ResourceUnitsText != null)
        {
            int flatResourceCount = (int)ResourceCount;
            ResourceUnitsText.text = flatResourceCount.ToString();

            ResourceUnitsText.transform.rotation = Camera.main.transform.rotation;
        }

        if(influencingUnits.Length == 0 || ResourceCount <= 0.0f)
        {
            if (!didReduceResources && foodParticleSystem.activeSelf)
            {
                foodParticleSystem.SetActive(false);
            }

            return;
        }

        float deltaTime = Time.deltaTime;
        float tapRateThisFrame = tapRatePerSec * deltaTime;

        if(ResourceCount - tapRateThisFrame < 0.0f)
        {
            tapRateThisFrame = ResourceCount;
        }

        float tappedValue = (tapRateThisFrame) / influencingUnits.Length;

        foreach(UnitLogic unit in influencingUnits)
        {
            tappedValue *= ingameLogicScript.GetTeamFoodCollectFactor(unit.TeamNumber);

            BreederLogic breederLogic = unit.gameObject.GetComponent<BreederLogic>();
            DroneLogic droneLogic = unit.GetComponent<DroneLogic>();

            float maxFoodResourceCount = 0;
            if (breederLogic != null)
            {
                maxFoodResourceCount = ingameLogicScript.GetBreederMaxFoodResource(unit.TeamNumber);
            }
            else if (droneLogic != null)
            {
                maxFoodResourceCount = ingameLogicScript.GetDroneMaxFoodResource(unit.TeamNumber);
            }

            if (unit.FoodResourceCount < maxFoodResourceCount)
            {
                float newFoodValue = tappedValue; 
                
                if (unit.FoodResourceCount +  newFoodValue > maxFoodResourceCount)
                {
                    newFoodValue = maxFoodResourceCount - unit.FoodResourceCount;
                }

                unit.FoodResourceCount += newFoodValue;
                if(unit.FoodResourceCount > maxFoodResourceCount)
                {
                    unit.FoodResourceCount = maxFoodResourceCount;
                }

                ResourceCount -= newFoodValue;
                didReduceResources = true;
            }
        }

        if (didReduceResources && !foodParticleSystem.activeSelf)
        {
            foodParticleSystem.SetActive(true);
        }
        else if (!didReduceResources && foodParticleSystem.activeSelf)
        {
            foodParticleSystem.SetActive(false);
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

            if (!listOfUnitsInfluencing.Contains(unitLogicScript))
            {
                listOfUnitsInfluencing.Add(unitLogicScript);
            }

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
