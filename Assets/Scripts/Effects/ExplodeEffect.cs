using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffect : IEffect
{
    private Board board;
    private Vector2Int coords;
    private PieceColor pieceColor;

    public ExplodeEffect(Board board, Vector2Int coords, PieceColor color)
    {
        this.board = board;
        this.coords = coords;
        this.pieceColor = color;
    }

    public void Play()
    {
        var position = board.CalcPositionFromCoords(coords + board.offset);
        VFXManager.Instance.PlaySplodeVFX(pieceColor, position, 3f);
        VFXManager.Instance.PlayExplodeVFX(position);
        SFXManager.Instance.PlayBigBangSFX();
        VFXManager.Instance.ShakeCamera();
    }
}
