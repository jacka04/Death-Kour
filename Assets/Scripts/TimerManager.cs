using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public PlayerController player;

    private float elapsedTime;
    private bool isTimerRunning = false;
    private bool isCountingDown = true;
    private float countdownValue = 3;

    void Start()
    {
        if (player != null) player.enabled = false;
        isTimerRunning = false;
        isCountingDown = true;
    }

    void Update()
    {
        if (isCountingDown)
        {
            HandleCountdown();
        }
        else if (isTimerRunning)
        {
            UpdateChronometer();
        }
    }

    void HandleCountdown()
    {
        countdownValue -= Time.deltaTime;

        if (countdownValue > 0)
        {
            timerText.text = Mathf.Ceil(countdownValue).ToString();
        }
        else
        {
            timerText.text = "GO!";
            isCountingDown = false;
            isTimerRunning = true;
            if (player != null) player.enabled = true;
            Invoke("ClearGoText", 1f);
        }
    }

    void UpdateChronometer()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        timerText.color = Color.yellow;
    }


}