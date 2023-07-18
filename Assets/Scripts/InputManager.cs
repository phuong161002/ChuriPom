using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector2 firstPoint;
    [SerializeField] private Board board;
    private Piece[] activePieces;
    private Vector2 offset;
    private bool isScrolling;
    private bool isMoving;
    private bool active = true;
    private (float, float) moveRange;
    private RollType rollType;
    private bool isOutOfMoveRange;
    private bool gameOver = true;
    private bool performingMove = false;

    public event System.Action OnPlayerMove;

    private void OnMouseDown()
    {
        if (!active || board == null || gameOver || performingMove)
        {
            return;
        }
        firstPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var coords = board.CalcCoordsFromPosition(firstPoint);
        if (board.CheckIfCoordsAreOnBoard(coords))
        {
            isMoving = true;
            return;
        }
    }

    private void OnMouseDrag()
    {
        if (!isMoving || !active || gameOver || performingMove)
            return;
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var delta = currentPos - firstPoint;
        {
            if (!isScrolling)
            {
                if (Vector2.Distance(firstPoint, currentPos) > 0.2f)
                {
                    isScrolling = true;
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        rollType = RollType.HORIZONTAL;
                        var coords = board.CalcCoordsFromPosition(firstPoint);
                        moveRange = board.GetRowMoveRange(coords.y);
                        activePieces = board.GetPiecesInRow(coords.y);
                    }
                    else
                    {
                        rollType = RollType.VERTICAL;
                        var coords = board.CalcCoordsFromPosition(firstPoint);
                        moveRange = board.GetColumnMoveRange(coords.x);
                        activePieces = board.GetPiecesInColumn(coords.x);
                    }

                    OnPlayerMove?.Invoke();
                }
            }
        }

        if (isScrolling)
        {
            var (max, min) = moveRange;
            //offset = new Vector2(Mathf.Clamp(direction.x * delta.x, min, max), Mathf.Clamp(direction.y * delta.y, min, max));

            switch (rollType)
            {
                case RollType.HORIZONTAL:
                    offset = new Vector2(delta.x, 0);
                    if (offset.x > max)
                    {
                        offset.x = max;
                        if (!isOutOfMoveRange)
                        {
                            isOutOfMoveRange = true;
                            board.OnPiecesMoveOutOfRange(activePieces, OutOfRangeType.RIGHT);
                        }
                    }
                    else if (offset.x < min)
                    {
                        offset.x = min;
                        if (!isOutOfMoveRange)
                        {
                            isOutOfMoveRange = true;
                            board.OnPiecesMoveOutOfRange(activePieces, OutOfRangeType.LEFT);
                        }
                    }
                    else
                    {
                        isOutOfMoveRange = false;
                    }
                    break;
                case RollType.VERTICAL:
                    offset = new Vector2(0, delta.y);
                    if (offset.y > max)
                    {
                        offset.y = max;
                        if (!isOutOfMoveRange)
                        {
                            isOutOfMoveRange = true;
                            board.OnPiecesMoveOutOfRange(activePieces, OutOfRangeType.TOP);
                        }
                    }
                    else if (offset.y < min)
                    {
                        offset.y = min;
                        if (!isOutOfMoveRange)
                        {
                            isOutOfMoveRange = true;
                            board.OnPiecesMoveOutOfRange(activePieces, OutOfRangeType.BOTTOM);
                        }
                    }
                    else
                    {
                        isOutOfMoveRange = false;
                    }
                    break;
                default:
                    break;
            }
            if (activePieces == null)
            {
                return;
            }
            foreach (var piece in activePieces)
            {
                piece?.SetOffset(offset);
            }
        }
    }

    private void OnMouseUp()
    {
        if (!active || board == null || gameOver || performingMove)
            return;
        isMoving = false;
        isScrolling = false;
        if (activePieces != null)
        {
            board.PerformMove(activePieces);
            activePieces = null;
        }
        rollType = RollType.NONE;
        offset = Vector2.zero;
        firstPoint = Vector2.zero;
        isOutOfMoveRange = false;
    }

    public void Enable()
    {
        Debug.Log("Enable Input");
        active = true;
    }

    public void Disable()
    {
        Debug.Log("Disable Input");
        active = false;
    }

    public void OnGameOver()
    {
        gameOver = true;
    }

    public void OnGameStarted()
    {
        gameOver = false;
        performingMove = false;
        active = true;
        isMoving = false;
        isScrolling = false;
        activePieces = null;
        rollType = RollType.NONE;
        offset = Vector2.zero;
        firstPoint = Vector2.zero;
        isOutOfMoveRange = false;
    }

    public void OnPerformMoveStarted()
    {
        performingMove = true;
    }

    public void OnPerformMoveCompleted()
    {
        performingMove = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && active)
        {
            Debug.Log("Lock Piece");
            board.LockPiece();
        }

        if(Input.GetKeyDown(KeyCode.H)  && active)
        {
            board.CreateHugePiece();
        }

        if (Input.GetKeyDown(KeyCode.E) && active)
        {
            board.CreateExplosivePiece();
        }

        //if (Input.GetKeyDown(KeyCode.C) && active)
        //{
        //    var move = BoardUtility.GetAvailableMove(board.data, board.hugePieceAnchor);
        //    //Debug.Log("Have Available Moves : " + BoardUtility.GetAvailableMove(board.data, board.hugePieceAnchor));
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    board.CreateFreezingPiece();
        //}

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    var (coords, direction, length, type) = board.GenerateHint();
        //    VFXManager.Instance.PlayHintEffect(board.CalcPositionFromCoords(coords + board.offset)
        //        + (Vector3)(type == HintType.Normal ? Vector2.zero : new Vector2(Board.SQUARE_SIZE / 2, Board.SQUARE_SIZE / 2)),
        //        direction, length, type);
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    GameManager.Instance.CurrentLevelManager.GameWin();
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    GameUIManager.Instance.PerFormLoseGame();
        //}

    }
}
