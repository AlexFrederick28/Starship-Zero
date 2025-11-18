using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [Tooltip("What the dialogue is about - what part of the narrative is it")]
    public string topic;
    public string[] dialogueText;
    public bool completedTopic = false;
    public bool completedPrerequisite = false;
    public bool isQuest = false;
    public Quest quest;
}
