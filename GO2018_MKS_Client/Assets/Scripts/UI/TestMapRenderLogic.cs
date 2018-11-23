using UnityEngine;
using UnityEngine.UI;

public class TestMapRenderLogic : MonoBehaviour
{
    public Texture2D stampTexture;

    void Start()
    {
        Texture2D texture = (Texture2D)GetComponent<RawImage>().mainTexture;
        if (texture != null)
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++) 
                {
                    if(x % 8 < 4)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.black);
                    }
                }
            }

            texture.Apply();
        }

    }

    void Update()
    {

    }
}
