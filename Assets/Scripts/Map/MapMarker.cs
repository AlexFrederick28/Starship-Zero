using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Used by quest objects or map references. Shows or hides a map marker object
/// </summary>
public class MapMarker : MonoBehaviour
{
    public SpriteRenderer markSprite;
    [Tooltip("If using a quest ID, this marker will automatically activate when the quest is activated")]
    public int questID;

    private void OnEnable()
    {
        QuestManager.OnActivateNewQuest += QuestActivatedMapMaker;
        QuestManager.OnQuestCompletion += QuestDeactivatedMapMarker;
    }

    private void OnDisable()
    {
        QuestManager.OnActivateNewQuest -= QuestActivatedMapMaker;
        QuestManager.OnQuestCompletion -= QuestDeactivatedMapMarker;
    }

    /// <summary>
    /// Manual activation of map marker
    /// </summary>
    public void ShowMapMarker()
    {
        if (markSprite.enabled == true) { return; }

        markSprite.enabled = true;
    }

    /// <summary>
    /// Manual deactivation of map marker
    /// </summary>
    public void HideMapMarker()
    {
        if (markSprite.enabled == false) { return; }

        markSprite.enabled = false;
    }

    public void QuestActivatedMapMaker(int activeQuestID)
    {
        if (activeQuestID != questID) { return; }
        if (markSprite.enabled == true) { return; }

        markSprite.enabled = true;
    }

    public void QuestDeactivatedMapMarker(int questID)
    {
        if (markSprite.enabled == false) { return; }

        markSprite.enabled = false;
    }
}
