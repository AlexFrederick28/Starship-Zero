using System;
using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour 
{
    public Transform respawnPoint;
    public Transform retryPoint;
    //public bool respawnActive = false;

    private void OnEnable()
    {
        if (GameState.instance != null)
        {
            GameState.instance.latestCheckpoint = this;
        }
    }

    private void OnDisable()
    {
        if (GameState.instance.latestCheckpoint == this)
        {
            GameState.instance.latestCheckpoint = null;
        }
    }

    public void Respawn()
    {
        GameState.instance.OnPlayerRespawn?.Invoke();
    }

    public void RetryInfestedRoom()
    {
        GameState.instance.OnPlayerRetry?.Invoke();
    }
}
