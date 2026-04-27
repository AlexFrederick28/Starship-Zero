using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    private bool readyToDestroy = false;
    private void OnEnable()
    {
        Destroy(gameObject, 5.0f);
    }

    private void Update()
    {
        if (readyToDestroy == false)
        {
            Destroy(gameObject, 5.0f);
            readyToDestroy = true;
        }
    }
}
