using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum States { Paused, Main, RoomClear }
    public States currentState;
    public bool gamePaused = false;
    private States previousState;
    public PlayerBase player;

    public static GameState instance;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void PauseAndResumeGame()
    {
        if (gamePaused == false)
        {
            Time.timeScale = 0f;
            previousState = currentState;
            currentState = States.Paused;
            gamePaused = true;
        }
        else if (gamePaused == true)
        {
            Time.timeScale = 1f;
            currentState = previousState;
            gamePaused = false;
        }
    }

    public void ChangeStateToMain()
    {
        currentState = States.Main;
    }

    public void ChangeStateToRoomClear()
    {
        currentState = States.RoomClear;
    }
}
