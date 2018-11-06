using UnityEngine;

public class UnitLogic : MonoBehaviour
{
    public int TeamNumber = 0;
    public bool IsSelected = false;
    
    public GameObject SelectionEffect;

    public float FoodResourceCount = 0.0f;
    public float MaxFoodResourceCount = 300.0f;

    public float TechResourceCount = 0.0f;
    public float MaxTechResourceCount = 300.0f;

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
}
