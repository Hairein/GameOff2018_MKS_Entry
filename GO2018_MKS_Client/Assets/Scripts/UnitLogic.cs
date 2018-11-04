using UnityEngine;

public class UnitLogic : MonoBehaviour
{
    public int TeamNumber = 0;
    public bool IsSelected = false;
    
    public GameObject SelectionEffect;

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
