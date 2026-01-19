using System.ComponentModel;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] protected float currentTime;
    public float CurrentTime
    {
        get { return currentTime; }
        private set
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value > timerLength)
            {
                value = timerLength;
            }
            currentTime = value;
        }
    }
    [SerializeField] protected float timerLength;
    public float TimerLength
    {
        get { return timerLength; }
        set
        {
            timerLength = value;
        }
    }
    [SerializeField] protected float currentDifficulty;
    public float CurrentDifficulty
    {
        get { return currentDifficulty; }
        private set
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value > maxDifficulty)
            {
                value = maxDifficulty;
            }

            currentDifficulty = value;
        }
    }
    [Tooltip("max difficulty / timer length = The lower the number, the higher the max difficulty can go - effecting how many scaling segments there will be in a single run")]
    [SerializeField] protected float maxDifficulty;
    [SerializeField] protected float scalingSegments;

    public virtual void Update()
    {
        // this is here for testing, realistically you would have this timer start when the player enters a room
        StartTimer();
        DifficultyScaling();
    }

    private void StartTimer()
    {
        if (currentTime < timerLength)
        {
            currentTime += Time.deltaTime;
        }
    }

    private void DifficultyScaling()
    {
        if (scalingSegments != (timerLength / maxDifficulty) / 60)
        {
            scalingSegments = (timerLength / maxDifficulty) / 60;
        }

        // scaling the difficulty based off of the current time and the amount of segments (How many times there will be a difficulty increase)
        currentDifficulty = ((timerLength / 60) + currentTime) / maxDifficulty / 60;
    }
}
