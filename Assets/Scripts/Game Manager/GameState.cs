using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum States { Paused, Main, RoomClear }
    public States currentState;
}
