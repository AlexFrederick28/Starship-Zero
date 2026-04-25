using UnityEngine;

public class UIBehaviours : MonoBehaviour
{
    public RectTransform rectTransform;
    public Vector2 sizeChangeValue;
    public Vector2 originalSize;
    
    public void EnlargeTransform()
    {
        rectTransform.sizeDelta += sizeChangeValue;
    }

    public void ShrinkTransform()
    {
        rectTransform.sizeDelta -= sizeChangeValue;
    }

    public void ChangeBackToOriginalSize()
    {
        rectTransform.sizeDelta = originalSize;
    }

    public void ChangeEnabledStatus(GameObject obj)
    {
        if (obj.activeSelf == true)
        {
            obj.SetActive(false);
        }
        else 
        {
            obj.SetActive(true);
        }
    }
}
