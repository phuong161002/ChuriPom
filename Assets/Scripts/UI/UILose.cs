using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UILose : MonoBehaviour, Puzzle.UI.IPopUpContent
{
    [SerializeField] private Button bExit;
    [SerializeField] private Button bTryAgain;

    private void Awake()
    {
        bExit.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadHome();
        });
        bTryAgain.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Hub.Hide(gameObject).Play();
            GameManager.Instance.OnReplay();
        });
    }

}
