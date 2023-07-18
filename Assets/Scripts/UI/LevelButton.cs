using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    private int level;

    [SerializeField] private Text tLevel;
    [SerializeField] Image iLevel;
    [SerializeField] Button bLevel;
    [SerializeField] Image star1;
    [SerializeField] Image star2;
    [SerializeField] Image star3;
    [SerializeField] Transform currentLevelHighlight;
    [SerializeField] Transform starHolder;

    [SerializeField] Sprite currentLevel;
    [SerializeField] Sprite lockedLevel;
    [SerializeField] Sprite passedLevel;

    [SerializeField] Sprite starGrey;
    [SerializeField] Sprite starGold;

    private void Awake()
    {
        bLevel.onClick.AddListener(() => GameManager.Instance.LoadLevel(level));
    }


    public void Setup(int level)
    {
        this.level = level;
        Reload();
    }

    public void Reload()
    {
        int currentLevel = GameManager.Instance.CurrentLevel;
        int numStar = PlayerPrefs.GetInt($"Level{level}_star", -1);

        tLevel.text = level.ToString();

        tLevel.gameObject.SetActive(currentLevel >= level);
        currentLevelHighlight.gameObject.SetActive(currentLevel == level);

        if (currentLevel == level)
        {
            iLevel.sprite = this.currentLevel;
        }
        else if (currentLevel > level)
        {
            iLevel.sprite = passedLevel;
        }
        else
        {
            iLevel.sprite = lockedLevel;
        }

        if (numStar < 0)
        {
            starHolder.gameObject.SetActive(false);
            return;
        }

        starHolder.gameObject.SetActive(true);
        star1.sprite = numStar > 0 ? starGold : starGrey;
        star2.sprite = numStar > 1 ? starGold : starGrey;
        star3.sprite = numStar > 2 ? starGold : starGrey;
    }

    private void OnEnable()
    {
        Reload();
    }
}
