using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coinCount = 0;

    public TextMeshProUGUI coinText;

    public void AddCoin()
    {
        coinCount += 1;
        coinText.text = coinCount.ToString();
    }

    void Start()
    {
        coinText.text = coinCount.ToString();
    }
}