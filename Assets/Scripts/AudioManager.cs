using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background;

    private void Start()
    {
        musicSource.clip = background; 
        musicSource.Play(); 
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}