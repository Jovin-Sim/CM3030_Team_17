using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

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
        musicSource.clip = bgm;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}