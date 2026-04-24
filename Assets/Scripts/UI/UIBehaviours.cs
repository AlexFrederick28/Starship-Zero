using UnityEngine;

public class UIBehaviours : MonoBehaviour
{
    public RectTransform rectTransform;
    public Vector2 sizeChangeValue;
    
    public void EnlargeTransform()
    {
        rectTransform.sizeDelta += sizeChangeValue;
    }

    public void ShrinkTransform()
    {
        rectTransform.sizeDelta -= sizeChangeValue;
    }
}
