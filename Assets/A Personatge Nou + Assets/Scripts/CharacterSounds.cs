using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip grabClip;
    [SerializeField] private AudioClip wallJumpClip; 

    [Header("Volumen")]
    [SerializeField] [Range(0f, 1f)] private float jumpVolume    = 0.8f;
    [SerializeField] [Range(0f, 1f)] private float dashVolume    = 1f;
    [SerializeField] [Range(0f, 1f)] private float landVolume    = 0.9f;
    [SerializeField] [Range(0f, 1f)] private float grabVolume    = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float wallJumpVolume = 0.8f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake  = false;
        audioSource.spatialBlend = 0f; 
    }

    public void PlayJump()     => Play(jumpClip,     jumpVolume);
    public void PlayDash()     => Play(dashClip,     dashVolume);
    public void PlayLand()     => Play(landClip,     landVolume);
    public void PlayGrab()     => Play(grabClip,     grabVolume);
    public void PlayWallJump() => Play(wallJumpClip ?? jumpClip, wallJumpVolume);

    private void Play(AudioClip clip, float volume)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}