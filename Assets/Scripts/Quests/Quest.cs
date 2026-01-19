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
        public int level;
        public int currencyReward;
        public bool complete;
    }

    public string description;
    public QuestPrerequisite prerequisite;
}
