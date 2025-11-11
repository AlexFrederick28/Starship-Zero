using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum States { Paused, Main, RoomClear }
    public States currentState;
    public bool gamePaused = false;

    public void PauseAndResumeGame()
    {
        if (gamePaused == false)
        {
            Time.timeScale = 0f;
            currentState = States.Paused;
            gamePaused = true;
        }
        else if (gamePaused == true)
        {
            Time.timeScale = 1f;

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
