using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfestedDoor : Door
{
    public Spawning spawning;
    //public Room parentRoom;
    public string difficultyName;
    private float roomDifficulty;
    private bool questEnabled = false;

    public override void OnInteract()
    {
        if (nearDoor == false) { return; }
        if (GameState.instance.playerInventory.weaponLoadoutList.Count <= 0)
        {
            if (gameObject.activeInHierarchy == true)
            {
                StartCoroutine(UIManager.instance.NewNotification("No Weapon Equipped!"));
            }
            return;
        }
        // if the player has some sort of UI open, make sure they close it before trying to enter an infested room
        if (GameState.instance.currentState == GameState.States.OpenUI) 
        {
            UIManager.instance.CloseAllInteractiveUI();
            GameState.instance.ChangeStateToMain();
        }
        if (spawning.entryQuestActivated == true)
        {
            for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
            {
                if (room.questID == QuestManager.instance.activeQuests[i].prerequisite.id)
                {
                    if (GameState.instance.currentState != GameState.States.RoomClear && room.playerInsideRoom == false)
                    {
                        // if the quest is active, enables and sends quest level to spawner as well as activates respawn checkpoint
                        GameState.instance.ChangeStateToRoomClear();
                        spawning.enabled = true;
                        spawning.questLevel = new Vector2(QuestManager.instance.activeQuests[i].prerequisite.level, GameState.instance.player.CurrentExperience);
                        GetComponent<RespawnCheckpoint>().enabled = true;
                        questEnabled = true;
                        break;
                    }
                    else if (GameState.instance.currentState != GameState.States.RoomClear && room.playerInsideRoom == true)
                    {
                        // leaving the infested room and disabling spawning and respawn
                        GameState.instance.ChangeStateToMain();
                        spawning.enabled = false;
                        GetComponent<RespawnCheckpoint>().enabled = false;
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
        else
        {
            if (GameState.instance.currentState != GameState.States.RoomClear && GetComponentInParent<Room>().playerInsideRoom == false)
            {
                // enables spawning
                base.OnInteract();
                GameState.instance.ChangeStateToRoomClear();
                spawning.enabled = true;
                GetComponent<RespawnCheckpoint>().enabled = true;
                return;
            }
            else if (GameState.instance.currentState != GameState.States.RoomClear && GetComponentInParent<Room>().playerInsideRoom == true)
            {
                // disables spawning
                base.OnInteract();
                GameState.instance.ChangeStateToMain();
                spawning.enabled = false;
                GetComponent<RespawnCheckpoint>().enabled = false;
                return;
            }
        }
    }
}
