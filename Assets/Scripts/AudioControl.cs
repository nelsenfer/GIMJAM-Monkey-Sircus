using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("musicVolume", volume);
    }
    public void SetSfxVolume(float volume)
    {
        sfxMixer.SetFloat("sfxVolume", volume);
    }
}
