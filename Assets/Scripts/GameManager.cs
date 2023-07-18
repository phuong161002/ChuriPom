using DG.Tweening;
using Puzzle.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonMonobehavior<GameManager>
{
    [SerializeField] private LevelManager currentLevelManager;
    [SerializeField] private LevelFillBottleManager levelFillBottlePrefab;
    [SerializeField] private LevelFillColorManager levelFillColorPrefab;
    [SerializeField] private LevelReleaseLockManager levelReleaseLockPrefab;

    [SerializeField] private int _currentLevel = -1;

    [SerializeField] private Image iTransition;
    [SerializeField] private float transitionDuration = 0.8f;

    private int _playingLevel = -1;

    private UIHome uiHome;
    private UIInGame uiInGame;
    private UIPause uiPause;
    private UIWin uiWin;
    private UILose uiLose;


    public int CurrentLevel
    {
        get => _currentLevel;
        private set
        {
            _currentLevel = value;
            PlayerPrefs.SetInt("CurrLevel", _currentLevel);
            PlayerPrefs.Save();
        }
    }

    public bool SoundOn
    {
        get => _soundOn;
        set
        {
            _soundOn = value;
            if (SFXManager.Instance)
            {
                SFXManager.Instance.SetActive(value);
            }
            PlayerPrefs.SetInt("Sound", _soundOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public bool MusicOn
    {
        get => _musicOn;
        set
        {
            _musicOn = value;
            if (Music.Instance)
            {

                Music.Instance.SetActive(value);
            }
            PlayerPrefs.SetInt("Music", _musicOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private bool _soundOn;
    private bool _musicOn;

    protected override void Awake()
    {
        base.Awake();
        SoundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        MusicOn = PlayerPrefs.GetInt("Music", 1) == 1;

        uiHome = Hub.Get<UIHome>(PopUpPath.POP_UP_UIHOME);
        uiInGame = Hub.Get<UIInGame>(PopUpPath.POP_UP_UIINGAME);
        uiPause = Hub.Get<UIPause>(PopUpPath.POP_UP_UIPAUSE);
        uiWin = Hub.Get<UIWin>(PopUpPath.POP_UP_UIWIN);
        uiLose = Hub.Get<UILose>(PopUpPath.POP_UP_UILOSE);

        Hub.Add(PopUpPath.POP_UP_UIHOME, uiHome);
        Hub.Add(PopUpPath.POP_UP_UIINGAME, uiInGame);
        Hub.Add(PopUpPath.POP_UP_UIPAUSE, uiPause);
        Hub.Add(PopUpPath.POP_UP_UIWIN, uiWin);
        Hub.Add(PopUpPath.POP_UP_UILOSE, uiLose);

        //Hub.Show(uiHome.gameObject).OnComplete(() => uiHome.Show(() => Debug.Log("Show ok!"))).Play();
        
        uiHome.Show();
        //CurrentLevel = PlayerPrefs.GetInt("CurrLevel", 1);
    }

    public void LoadLevel(int level)
    {
        Debug.Log("Load Level " + level);
        _playingLevel = level;
        if (currentLevelManager != null)
        {
            currentLevelManager.UnloadLevel();
            DestroyImmediate(currentLevelManager.gameObject);
            currentLevelManager = null;
        }

        StartCoroutine(Load());


        IEnumerator Load()
        {
            if (uiHome.gameObject.activeSelf)
            {
                yield return uiHome.HideRoutine();
                iTransition.gameObject.SetActive(true);
            }
            else
            {
                iTransition.gameObject.SetActive(true);
                yield return iTransition.DOFade(1f, transitionDuration).From(0f).WaitForCompletion();
            }
            var levelData = Instantiate(Resources.Load<LevelData>($"Level/Level {level}"));
            switch (levelData.levelType)
            {
                case LevelType.FillBottle:
                    currentLevelManager = Instantiate(levelFillBottlePrefab);
                    break;
                case LevelType.FillColor:
                    currentLevelManager = Instantiate(levelFillColorPrefab);
                    break;
                case LevelType.UnlockPiece:
                    currentLevelManager = Instantiate(levelReleaseLockPrefab);
                    break;
                case LevelType.Instruction:
                    break;
                default:
                    break;
            }

            if (currentLevelManager)
            {
                currentLevelManager.SetupLevel(levelData);
            }
            uiInGame.Setup(level, levelData);
            Music.Instance.Play(GameAssets.InGameMusic);
            iTransition.DOFade(0f, transitionDuration).From(1f).OnComplete(() => iTransition.gameObject.SetActive(false));
            yield return Hub.Show(uiInGame.gameObject).Play().WaitForCompletion();
        }

    }


    public void OnPause()
    {
        Time.timeScale = 0f;
        Hub.Show(uiPause.gameObject).Play();
    }

    public void OnResume()
    {
        Time.timeScale = 1f;
        Hub.Hide(uiPause.gameObject).Play();
    }

    public void LoadHome()
    {
        Time.timeScale = 1f;
        _playingLevel = -1;
        if (currentLevelManager != null)
        {
            currentLevelManager.UnloadLevel();
            DestroyImmediate(currentLevelManager.gameObject);
            currentLevelManager = null;
        }

        StartCoroutine(Load());

        IEnumerator Load()
        {
            if (uiPause.gameObject.activeSelf)
            {
                yield return Hub.Hide(uiPause.gameObject).Play().WaitForCompletion();
            }
            if (uiLose.gameObject.activeSelf)
            {
                yield return Hub.Hide(uiLose.gameObject).Play().WaitForCompletion();
            }
            iTransition.gameObject.SetActive(true);
            yield return iTransition.DOFade(1f, transitionDuration).From(0f).WaitForCompletion();
            yield return Hub.Hide(uiInGame.gameObject).Play().WaitForCompletion();
            Music.Instance.Play(GameAssets.HomeMusic);
            uiHome.gameObject.SetActive(true);
            iTransition.gameObject.SetActive(false);
            yield return uiHome.ShowRoutine();
            //yield return Hub.Show(uiHome.gameObject).Play().WaitForCompletion();
        }
    }

    public void OnReplay()
    {
        LoadLevel(_playingLevel);
    }

    public void LoadNextLevel()
    {
        LoadLevel(_playingLevel + 1);
    }

    public void OnWin(int score, int numStars)
    {
        if (_playingLevel == _currentLevel)
        {
            CurrentLevel++;
        }
        uiWin.Setup(score, numStars);
        SFXManager.Instance.PlayGameWinSFX();
        Hub.Show(uiWin.gameObject);
    }

    public void OnLose()
    {
        Music.Instance.Stop();
        SFXManager.Instance.PlayGameLoseSFX();
        Hub.Show(uiLose.gameObject);
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnWin(10, 3);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnLose();
        }
    }
#endif
}
