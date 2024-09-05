using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    // AudioClips for various game sounds and bgm
    public AudioClip bgm;
    public AudioClip buttonSelect;
    public AudioClip gunshot;
    public AudioClip explosion;
    public AudioClip enemyAttack;
    public AudioClip enemyDeath;
    public AudioClip playerDeath;
    public AudioClip winGame;

    private void Start()
    {
        // Set the background music clip and play it
        musicSource.clip = bgm;
        musicSource.Play();
    }

    /// <summary>
    /// Plays a specified sfx
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    public void PlaySFX(AudioClip clip)
    {
        // Play the provided sound effect as a one shot
        SFXSource.PlayOneShot(clip);
    }
}