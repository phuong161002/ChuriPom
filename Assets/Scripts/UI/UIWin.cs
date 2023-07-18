using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button bReplay;
    [SerializeField] private Button bNext;
    [SerializeField] private Text tScore;
    [SerializeField] private Image star1Fill;
    [SerializeField] private Image star2Fill;
    [SerializeField] private Image star3Fill;

    private void Awake()
    {
        bReplay.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Hub.Hide(gameObject).Play();
            GameManager.Instance.OnReplay();
        });
        bNext.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Hub.Hide(gameObject).Play();
            GameManager.Instance.LoadNextLevel();
        });
    }

    public void Setup(int score, int numStar)
    {
        tScore.text = score.ToString();
        star1Fill.gameObject.SetActive(numStar >= 1);
        star2Fill.gameObject.SetActive(numStar >= 2);
        star3Fill.gameObject.SetActive(numStar >= 3);
    }
}
