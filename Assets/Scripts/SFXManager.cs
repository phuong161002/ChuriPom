using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : SingletonMonobehavior<SFXManager>
{
    [SerializeField] private int numTracks;
    private AudioSource[] audioSources;

    [SerializeField] AudioClip chuzzledSFX;
    [SerializeField] AudioClip[] popSFX;
    [SerializeField] AudioClip[] popShriekSFX;
    [SerializeField] AudioClip lockClankSFX;
    [SerializeField] AudioClip breakLockSFX;
    [SerializeField] AudioClip lockSFX;
    [SerializeField] AudioClip hurkSFX;
    [SerializeField] AudioClip badMoveSFX;
    [SerializeField] AudioClip bottledSFX;
    [SerializeField] AudioClip bigPopSFX;
    [SerializeField] AudioClip bigBangSFX;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip gameLoseSFX;
    [SerializeField] AudioClip gameWinSFX;
    [SerializeField] AudioClip starWinSFX;


    protected override void Awake()
    {
        base.Awake();
        audioSources = new AudioSource[numTracks];
        for(int i = 0; i < numTracks; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
        }

        SetActive(PlayerPrefs.GetInt("Sound", 1) == 1);
    }

    public void Play(AudioClip audioClip, int track = 0, float volume = 1f)
    {
        audioSources[track].PlayOneShot(audioClip, volume);
    }

    public void PlayPopSFX(int combo)
    {
        Play(popSFX[Mathf.Clamp(combo, 0, popSFX.Length - 1)], 0, 0.9f);
        int num = Random.Range(0, popShriekSFX.Length);
        Play(popShriekSFX[num]);
    }

    public void PlayLockClankSFX()
    {
        Play(lockClankSFX);
    }

    public void PlayLockSFX()
    {
        Play(lockSFX);
    }

    public void PlayBreakLockSFX()
    {
        Play(breakLockSFX);
    }

    public void PlayHurkSFX()
    {
        Play(hurkSFX);
    }

    public void PlayBadMoveSFX()
    {
        Play(badMoveSFX);
    }

    public void PlayBottledSFX()
    {
        Play(bottledSFX);
    }

    public void PlayBigPopSFX()
    {
        Play(bigPopSFX);
    }

    public void PlayBigBangSFX()
    {
        Play(bigBangSFX);
    }

    public void PlayClickSFX()
    {
        Play(clickSFX);
    }

    public void PlayGameLoseSFX()
    {
        Play(gameLoseSFX);
    }

    public void PlayGameWinSFX()
    {
        Play(gameWinSFX);
    }

    public void PlayStarWinSFX()
    {
        Play(starWinSFX);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
