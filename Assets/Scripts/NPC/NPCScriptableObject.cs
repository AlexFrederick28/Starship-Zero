using UnityEngine;

[CreateAssetMenu(fileName = "NPCScriptableObject", menuName = "Scriptable Objects/NPCScriptableObject")]
public class NPCScriptableObject : ScriptableObject
{
    public string nameNPC;
    public enum NPCType { Questline, Narrative, QuestlineNarrative }
    public NPCType typeNPC;
}
