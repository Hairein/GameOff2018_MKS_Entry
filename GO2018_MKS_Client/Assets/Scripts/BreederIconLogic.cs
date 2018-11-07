using UnityEngine;
using UnityEngine.UI;

public class BreederIconLogic : MonoBehaviour
{
    private IngameSceneLogicScript ingameLogicScript = null;

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
        if(ingameLogic == null)
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
        if(ingameLogicScript == null)
        {
            return;
        }

        GameObject breeder = ingameLogicScript.teamBreeder;
        if(breeder == null)
        {
            return;
        }

        UnitLogic unitLogicScript = breeder.GetComponent<UnitLogic>();
        if(unitLogicScript == null)
        {
            return;
        }

        // Food
        float foodValue = (int)unitLogicScript.FoodResourceCount;
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

            string foodValueText = ((int)foodValue).ToString();
            FoodText.text = foodValueText;
            FoodShadowText.text = foodValueText;

            previousFoodValue = foodValue;
        }

        // Tech
        float techValue = (int)unitLogicScript.TechResourceCount;
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

            string techValueText = ((int)techValue).ToString();
            TechText.text = techValueText;
            TechShadowText.text = techValueText;

            previousTechValue = techValue;
        }
    }
}
