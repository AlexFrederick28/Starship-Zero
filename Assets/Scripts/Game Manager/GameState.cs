using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    public enum States { Paused, Main, RoomClear, OpenUI }
    public States currentState;
    public bool gamePaused = false;
    private States previousState;
    public PlayerBase player;
    public Transform playerTransform;
    public Inventory playerInventory;
    public RespawnCheckpoint latestCheckpoint;

    // respawn and retry is invoked by the respawn chekpoint once the player has clicked one of the buttons
    public Action OnPlayerRespawn;
    public Action OnPlayerRetry;
    public Action OnPlayerLevelUp;

    [Space]
    [Header("Audio")]
    [SerializeField] protected float musicVolume;

    public static GameState instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ChangeStateToMain();
    }

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }

        OnPlayerRespawn += PlayerRespawnFromCheckpoint;
        OnPlayerRetry += PlayerRetryFromCheckpoint;
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }

        OnPlayerRespawn -= PlayerRespawnFromCheckpoint;
        OnPlayerRetry -= PlayerRetryFromCheckpoint;
    }

    public void PauseAndResumeGame()
    {
        if (gamePaused == false)
        {
            Time.timeScale = 0f;
            previousState = currentState;
            currentState = States.Paused;
            Debug.Log("Game paused");
            gamePaused = true;
        }
        else if (gamePaused == true)
        {
            Time.timeScale = 1f;
            currentState = previousState;
            Debug.Log("Game un-paused");
            gamePaused = false;
        }
    }

    public void ChangeStateToMain()
    {
        StopAllCoroutines();
        currentState = States.Main;
        StartCoroutine(SoundManager.instance.PlayMusicClipCoroutine(SoundManager.instance.mainMusic, transform, musicVolume, true));
        Debug.Log("State changed to main");
    }

    public void ChangeStateToRoomClear()
    {
        StopAllCoroutines();
        currentState = States.RoomClear;
        StartCoroutine(SoundManager.instance.PlayMusicClipCoroutine(SoundManager.instance.battleMusic, transform, musicVolume, true));
        Debug.Log("State changed to room clear");
    }

    public void ChangeStateToOpenUI()
    {
        previousState = currentState;
        currentState = States.OpenUI;
        Debug.Log("State changed to OpenUI");
    }

    public void ChangeToPreviousState()
    {
        currentState = previousState;
    }

    public void PlayerRespawnFromCheckpoint()
    {
        player.transform.position = latestCheckpoint.respawnPoint.position;
        ChangeStateToMain();
    }

    public void PlayerRetryFromCheckpoint()
    {
        player.transform.position = latestCheckpoint.retryPoint.position;
    }
}
