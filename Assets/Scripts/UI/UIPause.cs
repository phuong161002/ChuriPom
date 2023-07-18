using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour, Puzzle.UI.IPopUpContent
{
    [SerializeField] private Button bResume;
    [SerializeField] private Button bHome;
    [SerializeField] private Button bSoundToggle;
    [SerializeField] private Button bMusicToggle;

    [SerializeField] private Sprite soundOff;
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private Sprite musicOn;

    private void Awake()
    {
        bResume.onClick.AddListener(() => GameManager.Instance.OnResume());
        bHome.onClick.AddListener(() => GameManager.Instance.LoadHome());
        bSoundToggle.onClick.AddListener(() =>
        {
            GameManager.Instance.SoundOn = !GameManager.Instance.SoundOn;
            bSoundToggle.image.sprite = GameManager.Instance.SoundOn ? soundOn : soundOff;
        });

        bMusicToggle.onClick.AddListener(() =>
        {
            GameManager.Instance.MusicOn = !GameManager.Instance.MusicOn;
            bMusicToggle.image.sprite = GameManager.Instance.MusicOn ? musicOn : musicOff;
        });
    }

    private void OnEnable()
    {
        bSoundToggle.image.sprite = GameManager.Instance.SoundOn ? soundOn : soundOff;
        bMusicToggle.image.sprite = GameManager.Instance.MusicOn ? musicOn : musicOff;
    }
}
