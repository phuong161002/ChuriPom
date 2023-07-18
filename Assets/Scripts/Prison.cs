using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison : MonoBehaviour
{
    [SerializeField] Transform CenterTransform;
    [SerializeField] GameObject barPrefab;
    [SerializeField] float moveRange = 2f;
    [SerializeField] DetainedPiece detainedPiece;

    private const float WIDTH = 1.5f;
    private GameObject[] bars;
    private int numLockedPiece;
    private int numBar;

    public int Size { get => numLockedPiece; }

    public void Setup(int numBar)
    {
        this.numBar = numBar;
        bars = new GameObject[numBar];
        float delta = WIDTH / (numBar - 1);
        int num = numBar / 2;
        int num2 = numBar % 2;

        for (int i = 0; i < numBar; i++)
        {
            int num3 = i % 2;

            float deltaX = delta * (num - (i / 2) - 0.5f * (1 - num2)) * (num3 == 1 ? 1 : -1);
            bars[i] = Instantiate(barPrefab, CenterTransform);
            bars[i].transform.position = (Vector2)CenterTransform.position + new Vector2(deltaX, 0);
        }
        numLockedPiece = numBar;
    }

    public void LockPrison(int count)
    {
        numLockedPiece = Mathf.Clamp(numLockedPiece + count, 0, int.MaxValue);
        UpdateBar();
    }

    public void UnlockPrison(int count)
    {
        numLockedPiece = Mathf.Clamp(numLockedPiece - count, 0, int.MaxValue);
        UpdateBar();
    }

    private void UpdateBar()
    {
        //Debug.Log(numLockedPiece);
        int currentBar = Mathf.Clamp(numLockedPiece, 0, numBar);
        for (int i = 0; i < numBar - currentBar; i++)
        {
            var endPos = new Vector2(bars[i].transform.position.x, CenterTransform.position.y - moveRange);
            bars[i].transform.DOMove(endPos, 1f);
        }

        for (int i = numBar - currentBar; i < numBar; i++)
        {
            var endPos = new Vector2(bars[i].transform.position.x, CenterTransform.position.y);
            bars[i].transform.DOMove(endPos, 1f);
        }
    }

    public void OnReleased()
    {
        detainedPiece.ReleasePiece();
    }
}
