using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LargeNotification : MonoBehaviour
{
    public float sizeIncrease;
    public float screenTime;
    public string description;
    public TextMeshProUGUI fontText;

    private void Start()
    {
        StartCoroutine(Execute());
    }

    public IEnumerator Execute()
    {
        Debug.Log("Spawned large notification");

        fontText.text = description;

        float timer = 0;
        while (timer < screenTime)
        {
            fontText.fontSize += sizeIncrease;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
