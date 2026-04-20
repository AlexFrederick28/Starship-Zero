using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(UIManager.instance.MasterSlider.value); // volume for master
    }

    public void TogglePauseUI(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            Debug.Log("pause UI: test123");

            if (UIManager.instance.pauseMenuParent != null)
            {
                Debug.Log("pause UI: not null, GAME STATE: " + GameState.instance.currentState);

                if (GameState.instance.currentState == GameState.States.Main || GameState.instance.currentState == GameState.States.RoomClear)
                {
                    if (GameState.instance.currentState != GameState.States.Paused)
                    {
                        Debug.Log("pause UI: is main or room clear");

                        if (UIManager.instance.pauseMenuParent.activeSelf == false)
                        {
                            UIManager.instance.pauseMenuParent.SetActive(true);
                            GameState.instance.ChangeStateToPaused();
                        }
                    }
                }

                else if (UIManager.instance.pauseMenuParent.activeSelf == true)
                {
                    UIManager.instance.pauseMenuParent.SetActive(false);
                    GameState.instance.ChangeToPreviousState();
                }


            }
            else
            {
                Debug.Log("pause UI: UI is null!");
            }


        }
     
    }

    public void CloseParentPauseUI()
    {
        UIManager.instance.pauseMenuParent.SetActive(false);
        GameState.instance.ChangeToPreviousState();
    }


    public void OpenPauseButtonsUI()
    {
        UIManager.instance.pauseButtonPanel.SetActive(true);
    }

    public void ClosePauseButtonsUI()
    {
        UIManager.instance.pauseButtonPanel.SetActive(false);
    }

    public void OpenControlUI()
    {
        UIManager.instance.controlPanel.SetActive(true);
    }

    public void CloseControlUI()
    {
        UIManager.instance.controlPanel.SetActive(false);
    }

    public void OpenVolumeUI()
    {
        UIManager.instance.audioPanel.SetActive(true);
    }

    public void CloseVolumeUI()
    {
        UIManager.instance.audioPanel.SetActive(false);
    }


}
