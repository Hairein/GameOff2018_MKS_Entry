using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarricadeLogic : MonoBehaviour
{
    private IngameSceneLogicScript ingameLogicScript = null;

    public float LifeCount = 1000.0f;
    public float MaxLifeCount = 1000.0f;

    public TextMesh LifeUnitsText;

    public UnitLogic[] influencingUnits;

    public float tapRatePerSec = 25.0f;

    public GameObject breakParticleSystem;

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
        if (LifeUnitsText != null)
        {
            int flatLifeCount = (int)LifeCount;
            LifeUnitsText.text = flatLifeCount.ToString();

            LifeUnitsText.transform.rotation = Camera.main.transform.rotation;
        }

        if(influencingUnits.Length == 0 || LifeCount <= 0.0f || LifeCount > MaxLifeCount)
        {
            if (breakParticleSystem.activeSelf)
            {
                breakParticleSystem.SetActive(false);
            }

            return;
        }

        float deltaTime = Time.deltaTime;
        float baseTapRateThisFrame = tapRatePerSec * deltaTime;

        if(LifeCount - baseTapRateThisFrame < 0.0f)
        {
            baseTapRateThisFrame = LifeCount;
        }

        int nosBuilders = 0;
        int nosDestroyers = 0;

        foreach (UnitLogic unitLogic in influencingUnits)
        {
            bool teamHasBarricadeBuildUpgrade = false;
            bool teamHasBarricadeBreakUpgrade = false;

            if(unitLogic.TeamNumber == 1)
            {
                teamHasBarricadeBuildUpgrade = ingameLogicScript.Team1HasUpgradedBarricadeBuild;
                teamHasBarricadeBreakUpgrade = ingameLogicScript.Team1HasUpgradedBarricadeBreak;
            }
            else
            {
                teamHasBarricadeBuildUpgrade = ingameLogicScript.Team2HasUpgradedBarricadeBuild;
                teamHasBarricadeBreakUpgrade = ingameLogicScript.Team2HasUpgradedBarricadeBreak;
            }

            BreederLogic breederLogic = unitLogic.gameObject.GetComponent<BreederLogic>();
            DroneLogic droneLogic = unitLogic.gameObject.GetComponent<DroneLogic>();

            if (breederLogic != null || droneLogic != null)
            {
                if (teamHasBarricadeBuildUpgrade && ingameLogicScript.TeamInBarricadeBuildMode)
                {
                    nosBuilders++;
                }

                if (teamHasBarricadeBreakUpgrade && ingameLogicScript.TeamInBarricadeBreakMode)
                {
                    nosDestroyers++;
                }
            }
        }

        if (nosBuilders > 0)
        {
            float totalBuildValue = nosBuilders * baseTapRateThisFrame;
            LifeCount += totalBuildValue;

            if (LifeCount > MaxLifeCount)
            {
                LifeCount = MaxLifeCount;
            }
        }

        if (nosDestroyers > 0)
        {
            float totalBreakValue = nosDestroyers * baseTapRateThisFrame;
            LifeCount -= totalBreakValue;

        }

        if (breakParticleSystem != null)
        {
            if(nosDestroyers > 0 && !breakParticleSystem.activeSelf)
            {
                breakParticleSystem.SetActive(true);
            }
            else if (nosDestroyers == 0 && breakParticleSystem.activeSelf)
            {
                breakParticleSystem.SetActive(false);
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
