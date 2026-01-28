using UnityEngine;
using TMPro;
using PlayerSystem;

public class EndLevel : MonoBehaviour
{
    public GameObject summaryPanel;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI finalCoinsText;

    public TimerManager timerManager;
    public CoinManager coinManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.InputBlocked = true;
            }

            if (timerManager != null)
            {
                timerManager.StopTimer();
                finalTimeText.text = "Tiempo: " + timerManager.GetFormattedTime();
            }

            if (coinManager != null)
            {
                finalCoinsText.text = "Monedas: " + coinManager.coinCount.ToString();
            }

            if (summaryPanel != null)
            {
                summaryPanel.SetActive(true);
            }
        }
    }
}