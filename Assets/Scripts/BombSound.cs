using UnityEngine;

public class BombSound : MonoBehaviour
{
    private AudioSource myAudioSource;

    void Start()
    {
        // Jika bom meledak, suara AudioSource otomatis hilang karena objek hancur
        myAudioSource = GetComponent<AudioSource>();
        if(myAudioSource != null)
        {
            myAudioSource.clip = AudioManager.Instance.sumbuTerbakar;
            myAudioSource.loop = true;
            myAudioSource.Play();
        }
    }
}
