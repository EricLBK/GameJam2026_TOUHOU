using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundScript : MonoBehaviour
{
    public enum SoundEffects
    {
       EnemyBullet,
       ActivateFocus,
       EnemyHit,
       EnemyDeath,
    }
    public static PlaySoundScript Instance { get; private set; }
    private AudioSource _audioSource;
    private Dictionary<SoundEffects, AudioClip> enumToSound;

    public AudioClip EnemyHit;

    public AudioClip EnemyDeath;
    public AudioClip AudioFocus;
    public AudioClip EnemyBullet;
    public AudioClip MenuClickConfirm;
    public AudioClip MenuClickHover;
    public AudioClip PlayerDamage;
    public AudioClip PlayerDeath;
    public AudioClip PlayerPickUpItem;
    public AudioClip PlayerShootFocus;
    public AudioClip PlayerShootNormal;
    public AudioClip PlayerShootNormalLoop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundEffects sounds)
    {

        switch (sounds)
        {
            case SoundEffects.EnemyHit:
                _audioSource.PlayOneShot(EnemyHit);
                break;
            case SoundEffects.EnemyDeath:
                _audioSource.PlayOneShot(EnemyDeath);
                break;
        }
    }
    

}
