using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : SingletonMonobehavior<Music>
{
    private AudioSource audioSource;

    [SerializeField] AudioClip firstMusicToPlay;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        Play(firstMusicToPlay);
        SetActive(PlayerPrefs.GetInt("Music", 1) == 1);
    }

    public void Play(AudioClip audioClip)
    {
        if (audioSource.clip != null)
        {
            audioSource.Stop();
        }
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }

    public void Stop()
    {
        audioSource.Pause();
    }
}
