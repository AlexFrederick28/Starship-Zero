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
            if (value < timerLength)
            {
                if (timerReachedMaxLength == true) { timerReachedMaxLength = false; } // reset timer if it has been manipulated somehow
            }
            if (value >= timerLength)
            {
                timerReachedMaxLength = true; // can be a win condition for the player
                value = timerLength;
            }
            currentTime = value;
        }
    }

    public float timerLength;

    [SerializeField] protected float currentDifficulty;
    public float CurrentDifficulty
    {
        get { return currentDifficulty; }
        private set
        {
            if (value < 1)
            {
                value = 1;
            }

            currentDifficulty = value;
        }
    }
    [Tooltip("(max difficulty / timer length) / 60 = (tracked in minutes) The lower the number, the higher the max difficulty can go - effecting how many scaling segments there will be in a single run")]
    public float difficultyMultiplier;
    [SerializeField] protected float scalingSegments;
    public bool timerReachedMaxLength = false;
    public bool timerPaused = false;

    public virtual void Update()
    {
        // timer starts when entering a room
        StartTimer();
        DifficultyScaling();
    }

    private void StartTimer()
    {
        if (timerPaused == true) { return; }
        if (CurrentTime < timerLength)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    public virtual void PauseSpawning()
    {
        if (timerPaused == false)
        {
            timerPaused = true;
        }
        else if (timerPaused == true)
        {
            timerPaused = false;
        }
    }

    private void DifficultyScaling()
    {
        if (scalingSegments != (timerLength / difficultyMultiplier) / 60)
        {
            scalingSegments = (timerLength / difficultyMultiplier) / 60;
        }

        CurrentDifficulty = ((timerLength / 60) + CurrentTime) / difficultyMultiplier / 60;
    }
}
