using DG.Tweening;
using UnityEngine;

public class LockedPiece : Piece
{
    [SerializeField] SpriteRenderer lockSpriteRenderer;

    public void HideLock()
    {
        lockSpriteRenderer.enabled = false;
    }

    public void ShowLock()
    {
        lockSpriteRenderer.enabled = true;
    }

    public void Warning()
    {
        lockSpriteRenderer.DOColor(UnityEngine.Color.red, 0.2f).onComplete += () =>
        {
            lockSpriteRenderer.DOColor(UnityEngine.Color.white, 0.1f);
        };
        //Debug.Log("Warning Lock Piece");
    }
}
