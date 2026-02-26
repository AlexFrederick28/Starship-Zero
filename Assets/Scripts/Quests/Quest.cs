using System;
using System.Numerics;
using System.Xml.Linq;
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

        public QuestPrerequisite(QuestPrerequisite other)
        {
            name = other.name;
            id = other.id;
            level = other.level;
            currencyReward = other.currencyReward;
            complete = other.complete;
        }
    }

    public string description;
    public QuestPrerequisite prerequisite;

    /// <summary>
    /// This class is used to copy the data from the scriptable object rather than use the scriptable object itself
    /// </summary>
    /// <param name="other"></param>
    public Quest(Quest other)
    {
        description = other.description;
        prerequisite = new QuestPrerequisite(other.prerequisite);
    }
}
