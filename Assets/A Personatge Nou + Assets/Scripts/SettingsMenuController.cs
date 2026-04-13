using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;

    void OnEnable()
    {
        // Cada vez que abres el panel de configuración,
        // el slider refleja el volumen actual
        if (AudioManager.Instance != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.GetVolume();
        }
    }

    // Conecta este método al evento OnValueChanged del Slider en el Inspector
    public void OnVolumeSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(value);
        }
    }
}