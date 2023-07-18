using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Eye : MonoBehaviour
{
    public int Value { get; set; } = 1;

    private Vector2 startPosition;

    public void Setup(Vector2 startPosition)
    {
        this.startPosition = startPosition;
    }

    public void MoveTo(EyeContainer container)
    {
        StartCoroutine(MoveRoutine(container));
    }

    private IEnumerator MoveRoutine(EyeContainer container, Action completedCallback = null)
    {
        var endPos = container.EndPosition;
        float x = Random.Range(-8f, 8f);
        float duration = Random.Range(0.5f, 1f);
        transform.GetChild(0).transform.DOLocalMoveX(x, duration / 2).SetEase(Ease.Linear).From(0)
            .onComplete += () => transform.GetChild(0).transform.DOLocalMoveX(0, duration / 2).SetEase(Ease.Linear);
        yield return transform.DOMove(endPos, duration).SetEase(Ease.InOutBack, 2f).From(startPosition).WaitForCompletion();
        container.OnEyeReceived(this);
        this.gameObject.SetActive(false);
        completedCallback?.Invoke();
    }
}
