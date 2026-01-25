using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // untuk menaruh audio(bgm / sfx)
    [Header("---------- Audio Clip ----------")]
    public AudioClip background;
    public AudioClip huh;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
    // untuk sfx control yang nanti dipanggil
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}