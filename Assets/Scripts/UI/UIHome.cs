using DG.Tweening;
using Puzzle.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour, IPopUpContent
{
    [SerializeField] private RectTransform levelContentRectTransform;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private UILevel uiLevel;


    [Header("Settings")]
    [SerializeField] private Button bSoundToggle;
    [SerializeField] private Button bMusicToggle;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;

    [Header("Level List")]
    [SerializeField] int numLevel;
    [SerializeField] LevelButton levelButtonPrefab;
    [SerializeField] Transform levelButtonsContainer;
    Dictionary<int, LevelButton> levelsDict;

    [Header("For animation")]
    [SerializeField] private Image black;
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    [SerializeField] private Image logo;
    [SerializeField] private float slideDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private Ease logoSlideEase;

    private void Awake()
    {
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

        InitLevels();
    }

    private void InitLevels()
    {
        levelsDict = new Dictionary<int, LevelButton>();
        for (int i = 0; i < numLevel; i++)
        {
            var levelButton = Instantiate(levelButtonPrefab, levelButtonsContainer);
            int level = i + 1;
            levelButton.Setup(level);
            levelsDict.Add(level, levelButton);
        }
    }

    private void OnEnable()
    {
        bSoundToggle.image.sprite = GameManager.Instance.SoundOn ? soundOn : soundOff;
        bMusicToggle.image.sprite = GameManager.Instance.MusicOn ? musicOn : musicOff;

        Canvas.ForceUpdateCanvases();
        var pos = levelsDict[GameManager.Instance.CurrentLevel].transform.position;
        var deltaY = transform.InverseTransformPoint(pos).y;
        levelContentRectTransform.position -= new Vector3(0, deltaY, 0);
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        var pos = levelsDict[GameManager.Instance.CurrentLevel].transform.position;
        var deltaY = transform.InverseTransformPoint(pos).y;
        levelContentRectTransform.position -= new Vector3(0, deltaY, 0);
    }

    public void Show(Action callback = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowRoutine(callback));
    }

    public void Hide(Action callback = null)
    {
        StartCoroutine(HideRoutine(callback));
    }

    public IEnumerator ShowRoutine(Action callback = null)
    {
        black.gameObject.SetActive(true);
        bSoundToggle.image.enabled = false;
        bMusicToggle.image.enabled = false;
        var logoPosition = logo.transform.position;
        logo.transform.position = logoPosition + Vector3.up * 400;
        top.transform.DOMoveY(top.transform.position.y, slideDuration)
            .From(top.transform.position.y + top.preferredHeight)
            .SetEase(Ease.Linear);
        bottom.transform.DOMoveY(bottom.transform.position.y, slideDuration)
            .From(bottom.transform.position.y - bottom.preferredHeight)
            .SetEase(Ease.Linear);
        yield return new WaitForSeconds(slideDuration);
        yield return logo.transform.DOMoveY(logoPosition.y, slideDuration)
            .From(logoPosition.y + 400)
            .SetEase(logoSlideEase).WaitForCompletion();
        bSoundToggle.image.enabled = true;
        bMusicToggle.image.enabled = true;
       
        bSoundToggle.image.DOFade(1f, 0.2f).From(0f);
        bMusicToggle.image.DOFade(1f, 0.2f).From(0f);
        yield return black.DOFade(0f, fadeDuration).From(1f).WaitForCompletion();
        black.gameObject.SetActive(false);
        callback?.Invoke();
    }

    public IEnumerator HideRoutine(Action callback = null)
    {
        black.gameObject.SetActive(true);

        bSoundToggle.image.DOFade(0f, 0.2f).From(1f);
        bMusicToggle.image.DOFade(0f, 0.2f).From(1f);
        yield return black.DOFade(1f, fadeDuration).From(0f).WaitForCompletion();


        var logoPosition = logo.transform.position;
        var topPosition = top.transform.position;
        var bottomPosition = bottom.transform.position;

        yield return logo.transform.DOMoveY(logoPosition.y + 400, slideDuration)
           .From(logoPosition.y)
           .SetEase(Ease.InBack).WaitForCompletion();

        top.transform.DOMoveY(topPosition.y + top.preferredHeight, slideDuration)
            .From(topPosition.y)
            .SetEase(Ease.Linear);
        bottom.transform.DOMoveY(bottomPosition.y - bottom.preferredHeight, slideDuration)
            .From(bottomPosition.y)
            .SetEase(Ease.Linear);
        yield return new WaitForSeconds(slideDuration);

        gameObject.SetActive(false);

        black.gameObject.SetActive(false);
        logo.transform.position = logoPosition;
        top.transform.position = topPosition;
        bottom.transform.position = bottomPosition;

        callback?.Invoke();
    }
}
