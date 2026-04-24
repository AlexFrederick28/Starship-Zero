using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public RectTransform mapRect;

    public Vector2 openedMapSize;
    public Vector2 minimisedMapSize;
    public Vector2 mapAnchorPosition;
    private bool mapOpen = false;

    public static MapManager instance;

    public RectMask2D mapRectMask;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        if (instance != this)
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }
    }

    public void OpenOrCloseMapWithKey(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        Debug.Log("Used Map");
        if (mapOpen == false)
        {
            mapRectMask.enabled = false;
            mapRect.sizeDelta = openedMapSize;
            mapRect.anchoredPosition = mapAnchorPosition;
            mapOpen = true;
            Debug.Log("Enlarged Map");
        }
        else if (mapOpen == true)
        {
            mapRectMask.enabled = true;
            mapRect.sizeDelta = minimisedMapSize;
            mapRect.anchoredPosition = mapAnchorPosition;
            mapOpen = false;
            Debug.Log("Minimised Map");
        }
    }

    public void OpenMap(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (mapOpen == true) { return; }

        mapRectMask.enabled = false;
        mapRect.sizeDelta = openedMapSize;
        mapRect.anchoredPosition = mapAnchorPosition;
        mapOpen = true;
    }

    public void CloseMap(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (mapOpen == false) { return; }

        mapRectMask.enabled = true;
        mapRect.sizeDelta = minimisedMapSize;
        mapRect.anchoredPosition = mapAnchorPosition;
        mapOpen = false;
    }
}
