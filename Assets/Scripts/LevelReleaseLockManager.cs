using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReleaseLockManager : LevelManager
{
    [SerializeField] private Prison prison;
    private Vector2Int[] initialLockCoords;

    private LockBar lockBar;
    private int lockBarCount;
    private bool mustLockPiece;
    private bool hasUnlockedPiece;

    public override void SetupLevel(LevelData levelData)
    {
        base.SetupLevel(levelData);
        foreach (var container in containers)
        {
            if (container is LockBar lockBar)
            {
                this.lockBar = lockBar;
                break;
            }
        }
        typeLevel = LevelType.UnlockPiece;
        initialLockCoords = levelData.initialLockCoords;
        lockBar.Size = levelData.lockBarSize;
        prison.Setup(levelData.initialLockCoords.Length);
    }

    public override void PostSetup()
    {
        foreach(var coords in levelData.initialLockCoords)
        {
            board.LockPieceAtCoords(coords);
        }
    }

    public override void OnPieceCleared(PieceData pieceData, Vector2Int coords, EffectChain effectChain)
    {
        if (pieceData.type == PieceType.LOCKED)
        {
            hasUnlockedPiece = true;
            prison.UnlockPrison(1);
        }

        if (effectChain != null)
        {
            lockBarCount++;
            effectChain.AddEffect(new EyeToContainerEffect(board.CalcPositionFromCoords(coords + board.offset), pieceData.eyeColor, lockBar));
        }
    }

    public override void OnFillPieceStarted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        base.OnFillPieceStarted(data, hugePieceAnchor);
        if(lockBarCount >= lockBar.Size)
        {
            mustLockPiece = true;
        }
    }

    public override void OnFillPieceCompleted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        if (prison.Size <= 0)
        {
            prison.OnReleased();
            GameWin();
            return;
        }

        if(hasUnlockedPiece)
        {
            hasUnlockedPiece = false;
            lockBarCount = 0;
            lockBar.ResetLockBar();
            return;
        }

        if(mustLockPiece)
        {
            mustLockPiece = false;
            lockBarCount = 0;
            prison.LockPrison(1);
            board.LockPiece();
            lockBar.ResetLockBar();
        }
        if (BoardUtils.GetAvailableMove(board.data, board.hugePieceAnchor) == null)
        {
            GameLose();
        }
        inputManager.OnPerformMoveCompleted();
    }

    public override void OnClickHintButton()
    {
        DisplayHintEffect();
    }
}
