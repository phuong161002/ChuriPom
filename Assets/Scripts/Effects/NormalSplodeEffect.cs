using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSplodeEffect : IEffect
{
    private Vector2Int coords;
    private Board board;
    private PieceColor pieceColor;

    public NormalSplodeEffect(Board board, Vector2Int coords, PieceColor pieceColor)
    {
        this.board = board;
        this.coords = coords;
        this.pieceColor = pieceColor;
    }

    public void Play()
    {
        var position = board.CalcPositionFromCoords(coords + board.offset);
        VFXManager.Instance.PlaySplodeVFX(pieceColor, position);
    }
}
