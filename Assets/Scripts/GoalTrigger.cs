using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public TimerManager timerManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (timerManager != null)
            {
                timerManager.StopTimer();
            }
        }
    }
}