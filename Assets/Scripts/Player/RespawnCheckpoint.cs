using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour 
{
    public Transform respawnPoint;
    public Transform retryInfestedRoomPoint;
    public bool respawnActive = false;

    public void Respawn()
    {
        GameState.instance.player.transform.position = respawnPoint.position;
        GameState.instance.player.ResetPlayerStatsOnRespawn();
        Spawning.instance.ResetInfestedRoom();
        Spawning.instance.GetComponentInParent<Room>().playerInsideRoom = false;
        Spawning.instance.enabled = false;
        GameState.instance.ChangeStateToMain();
    }

    public void RetryInfestedRoom()
    {
        GameState.instance.player.transform.position = retryInfestedRoomPoint.position;
        GameState.instance.player.ResetPlayerStatsOnRespawn();
        Spawning.instance.ResetInfestedRoom();
    }
}
