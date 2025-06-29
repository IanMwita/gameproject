using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip soundClip;

    public void PlaySound()
    {
        audioSource.PlayOneShot(soundClip);
    }
}
