using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeSplodeEffect : IEffect
{
    private Vector2Int coords;
    private Board board;
    private PieceColor pieceColor;

    public HugeSplodeEffect(Board board, Vector2Int coords, PieceColor pieceColor)
    {
        this.board = board;
        this.coords = coords;
        this.pieceColor = pieceColor;
    }

    public void Play()
    {
        var position = board.CalcPositionFromCoords(coords + board.offset) + new Vector3(Board.SQUARE_SIZE / 2, Board.SQUARE_SIZE / 2);
        SFXManager.Instance.PlayBigPopSFX();
        VFXManager.Instance.PlaySplodeVFX(pieceColor, position, 2f);
    }
}
