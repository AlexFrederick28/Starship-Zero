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
                // set a notification on the screen in big text letting the player know to clear the rest of the infested
                UIManager.instance.SpawnLargeNotification("Clear the Last of the Infested", screenTime, textSizeIncrease);
                //StartCoroutine(UIManager.instance.NewLargeNotification("Clear the Last of the Infested", screenTime, textSizeIncrease));
                value = timerLength;
            }
            currentTime = value;
        }
    }

    public float timerLength;
    public Vector2 minuteTimerLength;
    public Vector2 currentMinuteTime;

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

    [Space]
    [Header("Large Notification")]
    public float screenTime;
    public float textSizeIncrease;

    private void Start()
    {
        TranslateTimerToMinutesAndSeconds();
    }

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
            TranslateCurrentTimeToMinutesAndSeconds();
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

    private void TranslateTimerToMinutesAndSeconds()
    {
        // for the timer length outside of a room to display
        if (timerLength >= 60)
        {
            float minutes = Mathf.FloorToInt(timerLength / 60);
            float seconds = Mathf.FloorToInt(timerLength % 60);

            minuteTimerLength.x = minutes;
            minuteTimerLength.y = seconds;
        }
    }

    private void TranslateCurrentTimeToMinutesAndSeconds()
    {
        // display the current time when inside a room
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        currentMinuteTime.x = minutes;
        currentMinuteTime.y = seconds;
    }
}
