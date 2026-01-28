using System;
using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour 
{
    public Transform respawnPoint;
    public Transform retryInfestedRoomPoint;
    public bool respawnActive = false;

    public Action OnPlayerRespawn;
    public Action OnPlayerRetry;

    private void OnEnable()
    {
        GameState.instance.latestCheckpoint = this;
    }

    private void OnDisable()
    {
        GameState.instance.latestCheckpoint = null;
    }

    public void Respawn()
    {
        OnPlayerRespawn?.Invoke();

        GameState.instance.player.transform.position = respawnPoint.position;
        GameState.instance.player.ResetPlayerStatsOnRespawn();
        //Spawning.instance.ResetInfestedRoom
        Spawning.instance.GetComponentInParent<Room>().playerInsideRoom = false;
        Spawning.instance.enabled = false;
        GameState.instance.ChangeStateToMain();
    }

    public void RetryInfestedRoom()
    {
        OnPlayerRetry?.Invoke();

        GameState.instance.player.transform.position = retryInfestedRoomPoint.position;
        GameState.instance.player.ResetPlayerStatsOnRespawn();
        Spawning.instance.ResetInfestedRoom();
    }
}
