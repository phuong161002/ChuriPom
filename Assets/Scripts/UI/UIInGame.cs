using Puzzle.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour, IPopUpContent
{
    [SerializeField] Text tLevelTitle;
    [SerializeField] Button bMenu;
    [SerializeField] UIStarTimer starTimer;
    int level;

    public UIStarTimer UIStarTimer => starTimer;

    private void Awake()
    {
        bMenu.onClick.AddListener(() => GameManager.Instance.OnPause());
    }

    public void Setup(int level, LevelData levelData)
    {
        this.level = level;
        tLevelTitle.text = "Level " + level;
        float time = levelData.time / 3f;
        starTimer.Setup(time, time, time);
    }
}