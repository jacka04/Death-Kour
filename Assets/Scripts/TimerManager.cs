using UnityEngine;
using TMPro;
using PlayerSystem;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public PlayerController player;
    private float elapsedTime;
    private bool isTimerRunning = false;
    public float countdownValue = 3f;
    private bool isCountingDown = true;

    void Start()
    {
        if (player != null) player.InputBlocked = true;
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
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
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
            if (player != null) player.InputBlocked = false;
            Invoke("ClearGoText", 1f);
        }
    }

    void UpdateTimerDisplay()
    {
        timerText.text = GetFormattedTime();
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    void ClearGoText()
    {
        if (isTimerRunning) UpdateTimerDisplay();
    }
}