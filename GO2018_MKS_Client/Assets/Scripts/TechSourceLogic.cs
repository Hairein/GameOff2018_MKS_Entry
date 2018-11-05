using UnityEngine;
using UnityEngine.UI;

public class TechSourceLogic : MonoBehaviour
{
    public float ResourceCount = 444.0f;
    public float MaxResourceCount = 3000.0f;

    public GameObject ResourceUnitsText;

    void Start()
    {
    }

    void Update()
    {
        if (ResourceUnitsText != null)
        {
            Text resourcesText = ResourceUnitsText.GetComponent<Text>();
            if(resourcesText != null)
            {
                int flatResourceCount = (int)ResourceCount;
                resourcesText.text = flatResourceCount.ToString();
            }

            ResourceUnitsText.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
