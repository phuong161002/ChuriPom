using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugePieceTouchBorderEffect : IEffect
{
    private OutOfRangeType type;
    private Vector2Int hugePieceAnchor;
    private Board board;

    public HugePieceTouchBorderEffect(OutOfRangeType type, Vector2Int hugePieceAnchor, Board board)
    {
        this.type = type;
        this.hugePieceAnchor = hugePieceAnchor;
        this.board = board;
    }

    public void Play()
    {
        var position = board.CalcPositionFromCoords(hugePieceAnchor + board.offset);
        switch (this.type)
        {
            case OutOfRangeType.LEFT:
                position.y += Board.SQUARE_SIZE / 2;
                position.x -= hugePieceAnchor.x * Board.SQUARE_SIZE + Board.SQUARE_SIZE / 2;
                break;
            case OutOfRangeType.RIGHT:
                position.y += Board.SQUARE_SIZE / 2;
                position.x += (Board.WIDTH - hugePieceAnchor.x - 1) * Board.SQUARE_SIZE + Board.SQUARE_SIZE / 2;
                break;
            case OutOfRangeType.TOP:
                position.x += Board.SQUARE_SIZE / 2;
                position.y += (Board.HEIGHT - hugePieceAnchor.y - 1) * Board.SQUARE_SIZE + Board.SQUARE_SIZE / 2;
                break;
            case OutOfRangeType.BOTTOM:
                position.x += Board.SQUARE_SIZE / 2;
                position.y -= hugePieceAnchor.y * Board.SQUARE_SIZE + Board.SQUARE_SIZE / 2;
                break;
        }
        var type = (this.type == OutOfRangeType.LEFT || this.type == OutOfRangeType.RIGHT) ? RollType.VERTICAL : RollType.HORIZONTAL;
        CameraShaker.Instance.Shake(0.1f, 0.02f);
        VFXManager.Instance.PlayMindBenderEffect(position, type);
        SFXManager.Instance.PlayHurkSFX();
    }
}
