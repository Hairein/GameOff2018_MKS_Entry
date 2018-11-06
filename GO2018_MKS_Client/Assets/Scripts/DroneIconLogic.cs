using UnityEngine;
using UnityEngine.UI;

public class DroneIconLogic : MonoBehaviour
{
    private IngameSceneLogicScript ingameLogicScript = null;

    public int droneIndex = 0;

    public RectTransform FoodBgBar;
    public RectTransform FoodFgBar;

    public Text FoodText;
    public Text FoodShadowText;

    public RectTransform TechBgBar;
    public RectTransform TechFgBar;

    public Text TechText;
    public Text TechShadowText;

    private float previousFoodValue = -1.0f;
    private float previousTechValue = -1.0f;

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
        UpdateIconDisplay();
    }

    public void UpdateIconDisplay()
    {
        if (ingameLogicScript == null)
        {
            return;
        }

        if(droneIndex >= ingameLogicScript.teamDrones.Length)
        {
            return;
        }

        GameObject drone = ingameLogicScript.teamDrones[droneIndex];
        if (drone == null)
        {
            return;
        }

        UnitLogic unitLogicScript = drone.GetComponent<UnitLogic>();
        if (unitLogicScript == null)
        {
            return;
        }

        // Food
        float foodValue = unitLogicScript.FoodResourceCount;
        if (foodValue != previousFoodValue)
        {
            float foodRatio = unitLogicScript.FoodResourceCount / unitLogicScript.MaxFoodResourceCount;

            if (foodValue > 0.0f)
            {
                if (!FoodFgBar.gameObject.activeSelf)
                {
                    FoodFgBar.gameObject.SetActive(true);
                }

                FoodFgBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FoodBgBar.rect.width * foodRatio);
            }
            else
            {
                if (FoodFgBar.gameObject.activeSelf)
                {
                    FoodFgBar.gameObject.SetActive(false);
                }
            }

            string foodValueText = foodValue.ToString();
            FoodText.text = foodValueText;
            FoodShadowText.text = foodValueText;

            previousFoodValue = foodValue;
        }

        // Tech
        float techValue = unitLogicScript.TechResourceCount;
        if (techValue != previousTechValue)
        {
            float techRatio = unitLogicScript.TechResourceCount / unitLogicScript.MaxTechResourceCount;

            if (techValue > 0.0f)
            {
                if (!TechFgBar.gameObject.activeSelf)
                {
                    TechFgBar.gameObject.SetActive(true);
                }

                TechFgBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TechBgBar.rect.width * techRatio);
            }
            else
            {
                if (TechFgBar.gameObject.activeSelf)
                {
                    TechFgBar.gameObject.SetActive(false);
                }
            }

            string techValueText = techValue.ToString();
            TechText.text = techValueText;
            TechShadowText.text = techValueText;

            previousTechValue = techValue;
        }
    }
}
