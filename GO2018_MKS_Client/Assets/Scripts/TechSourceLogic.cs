using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechSourceLogic : MonoBehaviour
{
    private IngameSceneLogicScript ingameLogicScript = null;

    public float ResourceCount = 444.0f;
    public float MaxResourceCount = 3000.0f;

    public TextMesh ResourceUnitsText;

    public UnitLogic[] influencingUnits;

    public float tapRatePerSec = 20.0f;

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
        if (ResourceUnitsText != null)
        {
            int flatResourceCount = (int)ResourceCount;
            ResourceUnitsText.text = flatResourceCount.ToString();

            ResourceUnitsText.transform.rotation = Camera.main.transform.rotation;
        }

        if (influencingUnits.Length == 0 || ResourceCount <= 0.0f)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        float tapRateThisFrame = tapRatePerSec * deltaTime;

        if (ResourceCount - tapRateThisFrame < 0.0f)
        {
            tapRateThisFrame = ResourceCount;
        }

        float tappedValue = tapRateThisFrame / influencingUnits.Length;

        foreach (UnitLogic unit in influencingUnits)
        {
            BreederLogic breederLogic = unit.gameObject.GetComponent<BreederLogic>();
            DroneLogic droneLogic = unit.GetComponent<DroneLogic>();

            float maxTechResourceCount = 0;
            if(breederLogic != null)
            {
                maxTechResourceCount = ingameLogicScript.GetBreederMaxTechResource(unit.TeamNumber);
            }
            else if (droneLogic != null)
            {
                maxTechResourceCount = ingameLogicScript.GetDroneMaxTechResource(unit.TeamNumber);
            }

            if (unit.TechResourceCount < maxTechResourceCount)
            {
                float newTechValue = tappedValue;
                if (unit.TechResourceCount + newTechValue > maxTechResourceCount)
                {
                    newTechValue = maxTechResourceCount - unit.TechResourceCount;
                }

                unit.TechResourceCount += newTechValue;
                ResourceCount -= newTechValue;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        UnitLogic unitLogicScript = other.gameObject.GetComponent<UnitLogic>();
        if (unitLogicScript != null)
        {
            List<UnitLogic> listOfUnitsInfluencing = new List<UnitLogic>();
            foreach (UnitLogic unit in influencingUnits)
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
                if (unitLogicScript == unit)
                {
                    continue;
                }

                listOfUnitsInfluencing.Add(unit);
            }

            influencingUnits = listOfUnitsInfluencing.ToArray();
        }
    }
}
