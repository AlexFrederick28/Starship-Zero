using System;
using UnityEngine;

[Serializable]
public class Quest 
{
    [Serializable]
    public class QuestPrerequisite
    {
        public string name;
        public int id;
    }

    public string description;
    public QuestPrerequisite prerequisite;
}
