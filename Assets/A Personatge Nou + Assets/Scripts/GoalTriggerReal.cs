using UnityEngine;

public class GoalTriggerReal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;

        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();

        if (LevelCompleteUI.Instance != null)
            LevelCompleteUI.Instance.ShowLevelComplete();
    }
}