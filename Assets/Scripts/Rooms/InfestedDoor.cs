using UnityEngine;
using UnityEngine.SceneManagement;

public class InfestedDoor : Door
{
    private bool questEnabled = false;

    public override void OnInteract()
    {
        for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
        {
            if (GetComponent<Spawning>().questID == QuestManager.instance.activeQuests[i].prerequisite.id)
            {
                if (GameState.instance.currentState != GameState.States.RoomClear && GetComponentInParent<Room>().playerInsideRoom == false)
                {
                    // if the quest is active, enables and sends quest level to spawner as well as activates respawn checkpoint
                    GameState.instance.ChangeStateToRoomClear();
                    GetComponent<Spawning>().enabled = true;
                    GetComponent<Spawning>().questLevel = new Vector2(QuestManager.instance.activeQuests[i].prerequisite.level, GameState.instance.player.CurrentExperience);
                    GetComponent<RespawnCheckpoint>().respawnActive = true;
                    questEnabled = true;
                    break;
                }
                else if (GameState.instance.currentState != GameState.States.RoomClear && GetComponentInParent<Room>().playerInsideRoom == true)
                {
                    // leaving the infested room and disabling spawning and respawn
                    GameState.instance.ChangeStateToMain();
                    GetComponent<Spawning>().enabled = false;
                    GetComponent<RespawnCheckpoint>().respawnActive = false;
                    break;
                }
            }
            else if (i == QuestManager.instance.activeQuests.Count)
            {
                // if no active quests match the assigned quest ID
                questEnabled = false;
                break;
            }
            else
            {
                // if the quest ID doesnt match, keep cycling through active quests
                continue;
            }
        }

        if (questEnabled == true)
        {
            // if the matching quest is enabled, enter room
            base.OnInteract();
        }
    }
}
