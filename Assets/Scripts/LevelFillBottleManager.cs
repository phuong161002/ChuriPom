using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFillBottleManager : LevelManager
{
    private Bottle bottle;
    private LockBar lockBar;

    private bool haveEyeToLockBar;

    private int bottleCount;
    private int lockBarCount;
    private bool mustLockPiece;

    public override void SetupLevel(LevelData levelData)
    {
        base.SetupLevel(levelData);
        foreach (var container in containers)
        {
            if (container is Bottle bottle)
            {
                this.bottle = bottle;
            }
            else if (container is LockBar lockBar)
            {
                this.lockBar = lockBar;
            }
        }
        typeLevel = LevelType.FillBottle;
        bottle.SetMaxVolumn(levelData.bottleVolumn);
        lockBar.Size = levelData.lockBarSize;
    }

    public override void OnPieceCleared(PieceData pieceData, Vector2Int coords, EffectChain effectChain)
    {
        if (effectChain != null)
        {
            if (!haveEyeToLockBar)
            {
                lockBarCount++;
                effectChain.AddEffect(new EyeToContainerEffect(board.CalcPositionFromCoords(coords + board.offset), pieceData.eyeColor, lockBar));
                haveEyeToLockBar = true;
            }
            else
            {
                bottleCount++;
                effectChain.AddEffect(new EyeToContainerEffect(board.CalcPositionFromCoords(coords + board.offset), pieceData.eyeColor, bottle));
            }
        }
    }

    public override void OnFillPieceStarted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        base.OnFillPieceStarted(data, hugePieceAnchor);
        if (lockBarCount >= lockBar.Size)
        {
            mustLockPiece = true;
        }
        haveEyeToLockBar = false;
    }

    public override void OnFillPieceCompleted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        if (bottleCount >= bottle.maxVolumn)
        {
            inputManager.Disable();
            GameWin();
            return;
        }

        if (mustLockPiece)
        {
            mustLockPiece = false;
            lockBarCount = 0;
            board.LockPiece();
            lockBar.ResetLockBar();
        }

        if (BoardUtils.GetAvailableMove(board.data, board.hugePieceAnchor) == null)
        {
            GameLose();
        }
        inputManager.OnPerformMoveCompleted();
    }
}
