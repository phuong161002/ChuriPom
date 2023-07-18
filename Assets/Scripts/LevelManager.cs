using Mono.Cecil;
using Puzzle.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class LevelManager : MonoBehaviour, IEditorValidator
{
    [SerializeField] protected EyeContainer[] containers;
    [SerializeField] protected Board board;
    [SerializeField] protected ScoreKeeper scoreKeeper;
    [field: SerializeField] public InputManager inputManager { get; protected set; }
    public ScoreKeeper ScoreKeeper { get => scoreKeeper; }
    protected LevelType typeLevel;
    protected bool isGameOver = false;
    public float hugePieceRatio;
    protected int numMoves;
    protected float time;
    protected LevelData levelData;

    public virtual void SetupLevel(LevelData levelData)
    {
        this.levelData = levelData;
        hugePieceRatio = levelData.hugePieceRatio;
        time = levelData.time;
    }

    public virtual void PostSetup()
    {

    }

    public virtual void OnPieceCleared(PieceData pieceData, Vector2Int coords, EffectChain effectChain)
    {

    }

    public virtual void OnFillPieceStarted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        numMoves++;
    }

    public virtual void OnFillPieceCompleted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {

    }

    public void GameWin()
    {
        if (isGameOver)
            return;
        isGameOver = true;
        Debug.Log("You Win!!");
        Hub.Get<UIInGame>(PopUpPath.POP_UP_UIINGAME).UIStarTimer.Stop();
        int rank = CalcRank();
        GameManager.Instance.OnWin(scoreKeeper.Score, rank);
    }

    public void GameLose()
    {
        if (isGameOver)
            return;
        isGameOver = true;
        Debug.Log("You Lose!!");
        GameManager.Instance.OnLose();
    }

    public virtual void OnClickHintButton()
    {

    }

    protected int CalcRank()
    {
        float num = (1 - Hub.Get<UIInGame>(PopUpPath.POP_UP_UIINGAME).UIStarTimer.ElapsedSeconds / levelData.time) * 3;
        if (num > 2f)
        {
            return 3;
        }
        else if (num > 1)
        {
            return 2;
        }
        else if (num > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void DisplayHintEffect()
    {
        //var (coords, direction, length, type) = board.GenerateHint();
        //VFXManager.Instance.PlayHintEffect(board.CalcPositionFromCoords(coords + board.offset)
        //        + (Vector3)(type == HintType.Normal ? Vector2.zero : new Vector2(Board.SQUARE_SIZE / 2, Board.SQUARE_SIZE / 2)),
        //        direction, length, type);
    }

    public void OnEditorValidate(GameObject context)
    {
        board = context.GetComponentInChildren<Board>();
        containers = context.GetComponentsInChildren<EyeContainer>();
        inputManager = context.GetComponentInChildren<InputManager>();
        scoreKeeper = context.GetComponentInChildren<ScoreKeeper>();
    }

    public void UnloadLevel()
    {
        PoolManager.Instance.ResetPool();
    }
}


