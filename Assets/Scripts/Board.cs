using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour, IEditorValidator
{
    [SerializeField] private PieceFactory pieceCreator;
    [SerializeField] private Transform centerPoint;
    public InputManager inputManager;
    [SerializeField] private LevelManager level;

    public Action OnBoardInitialized;

    public const int WIDTH = 6;
    public const int HEIGHT = 6;
    public const float SQUARE_SIZE = 0.85f;
    public const float ROLLBACK_SPEED = 10f;
    public const float FILL_SPEED = 4f;
    public const float DELAY_FILL_PIECES = 0.1f;
    public const float DELAY_CHAIN_EFFECT = 0.2f;

    public int VirtualWidth = WIDTH * 2;
    public int VirtualHeight = HEIGHT * 2;


    private Piece[,] _virtualBoard;
    public PieceData[,] data;
    public Vector2Int offset;
    public Vector2Int hugePieceAnchor = invalidCoords;

    public static Vector2Int invalidCoords = new Vector2Int(-1, -1);
    public static Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    private static Vector2Int[] offsetOfBottomLeftAnchor = new Vector2Int[]
       {
            Vector2Int.zero,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.up + Vector2Int.right
       };

    private static Vector2Int[] offsetOfExplosion = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up + Vector2Int.left,
        Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.down + Vector2Int.right,
    };
    private int combo;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        do
        {
            InitBoard();
            level.PostSetup();
        }
        while (BoardUtils.GetAvailableMove(data, hugePieceAnchor) == null);
        _virtualBoard = new Piece[data.GetLength(0) * 2, data.GetLength(1) * 2];
        UpdateVirtualBoard(data, hugePieceAnchor);

        inputManager.OnGameStarted();
    }

    private void InitBoard()
    {
        data = new PieceData[WIDTH, HEIGHT];
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                data[x, y] = RandomPiece(PieceType.NORMAL);
            }
        }
        var matches = BoardUtils.CheckMatches(this);
        while (matches.Count > 0)
        {
            foreach (var cluster in matches)
            {
                var coords = cluster.GetCoords(cluster.Size / 2);
                data[coords.x, coords.y] = RandomPiece(PieceType.NORMAL);
            }

            matches = BoardUtils.CheckMatches(this  );
        }

        offset = new Vector2Int(WIDTH / 2, HEIGHT / 2);
    }

    private PieceData RandomPiece(PieceType pieceType)
    {
        return new PieceData()
        {
            pieceColor = (PieceColor)Random.Range(0, (int)PieceColor.COUNT),
            type = pieceType,
            eyeColor = (EyeColor)Random.Range(0, Enum.GetValues(typeof(EyeColor)).Length),
        };
    }

    public Vector3 CalcPositionFromCoords(Vector2Int coords)
    {
        return centerPoint.position
            + new Vector3(coords.x * SQUARE_SIZE - (VirtualWidth * SQUARE_SIZE) / 2, coords.y * SQUARE_SIZE - (VirtualHeight * SQUARE_SIZE) / 2)
            + new Vector3(SQUARE_SIZE / 2, SQUARE_SIZE / 2);
    }

    public Vector2Int CalcCoordsFromPosition(Vector3 position)
    {
        Vector3 delta = position - centerPoint.position + new Vector3((VirtualWidth) * SQUARE_SIZE / 2, (VirtualHeight) * SQUARE_SIZE / 2);
        return new Vector2Int(Mathf.FloorToInt(delta.x / SQUARE_SIZE), Mathf.FloorToInt(delta.y / SQUARE_SIZE));
    }

    public void UpdateVirtualBoard(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                UpdatePieceOnVirtualBoard(offset.x + x, offset.y + y, data[x, y]);
            }
        }

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < offset.y; y++)
            {
                UpdatePieceOnVirtualBoard(offset.x + x, y, data[x, HEIGHT - offset.y + y]);
            }

            for (int y = 0; y < HEIGHT - offset.y; y++)
            {
                UpdatePieceOnVirtualBoard(offset.x + x, offset.y + HEIGHT + y, data[x, y]);
            }
        }

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < offset.x; x++)
            {
                UpdatePieceOnVirtualBoard(x, offset.y + y, data[WIDTH - offset.x + x, y]);
            }

            for (int x = 0; x < WIDTH - offset.x; x++)
            {
                UpdatePieceOnVirtualBoard(offset.x + WIDTH + x, offset.y + y, data[x, y]);
            }
        }

        // Update position of all pieces
        for (int x = 0; x < _virtualBoard.GetLength(0); x++)
        {
            for (int y = 0; y < _virtualBoard.GetLength(1); y++)
            {
                if (_virtualBoard[x, y] != null)
                {
                    if (_virtualBoard[x, y] is HugePiece hugePiece)
                    {
                        bool active = new Vector2Int(x - offset.x, y - offset.y) == hugePieceAnchor;
                        hugePiece.ActiveSpriteRenderer(active);
                    }
                    _virtualBoard[x, y].Setup(new Vector2Int(x, y), this);
                }
            }
        }
    }

    private void UpdatePieceOnVirtualBoard(int x, int y, PieceData pieceData)
    {
        Piece piece = _virtualBoard[x, y];
        if (_virtualBoard[x, y] == null)
        {
            _virtualBoard[x, y] = pieceCreator.CreatePieceOfType(pieceData.type, pieceData.pieceColor, pieceData.eyeColor);
        }
        else if (piece.Type != pieceData.type)
        {
            _virtualBoard[x, y] = pieceCreator.CreatePieceOfType(pieceData.type, pieceData.pieceColor, pieceData.eyeColor);
            DestroyPiece(piece);
        }
        else if (piece.Color != pieceData.pieceColor)
        {
            _virtualBoard[x, y] = pieceCreator.CreatePieceOfType(pieceData.type, pieceData.pieceColor, pieceData.eyeColor);
            DestroyPiece(piece);
        }
    }

    public void DestroyPiece(Piece piece)
    {
        piece.gameObject.SetActive(false);
    }

    public (float, float) GetRowMoveRange(int row)
    {
        if (CheckIfRowLocked(row - offset.y))
        {
            return (0.1f, -0.1f);
        }
        if (hugePieceAnchor != invalidCoords && (hugePieceAnchor.y + offset.y == row || hugePieceAnchor.y + offset.y + 1 == row))
        {
            float max = (WIDTH - hugePieceAnchor.x - 2) * SQUARE_SIZE + 0.1f;
            float min = -((hugePieceAnchor.x) * SQUARE_SIZE + 0.1f);
            return (max, min);
        }

        return (float.MaxValue, float.MinValue);
    }

    private bool CheckIfRowLocked(int row)
    {
        if (hugePieceAnchor != invalidCoords && (hugePieceAnchor.y == row || hugePieceAnchor.y + 1 == row))
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (data[x, hugePieceAnchor.y].type == PieceType.LOCKED || data[x, hugePieceAnchor.y + 1].type == PieceType.LOCKED
                    || data[x, hugePieceAnchor.y].type == PieceType.FREEZING || data[x, hugePieceAnchor.y + 1].type == PieceType.FREEZING)
                {
                    return true;
                }
            }
        }
        else
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (data[x, row].type == PieceType.LOCKED || data[x, row].type == PieceType.FREEZING)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Piece[] GetPiecesInRow(int index)
    {
        var list = new List<Piece>();

        if (hugePieceAnchor != invalidCoords && (hugePieceAnchor.y + offset.y == index || hugePieceAnchor.y + offset.y + 1 == index))
        {
            for (int i = 0; i < VirtualWidth; i++)
            {
                if (_virtualBoard[i, hugePieceAnchor.y + offset.y] != null)
                {
                    list.Add(_virtualBoard[i, hugePieceAnchor.y + offset.y]);
                }
                if (_virtualBoard[i, hugePieceAnchor.y + offset.y + 1] != null)
                {
                    list.Add(_virtualBoard[i, hugePieceAnchor.y + offset.y + 1]);
                }
            }
            return list.ToArray();
        }

        for (int i = 0; i < VirtualWidth; i++)
        {
            if (_virtualBoard[i, index] != null)
            {
                list.Add(_virtualBoard[i, index]);
            }
        }
        return list.ToArray();
    }

    public (float, float) GetColumnMoveRange(int col)
    {
        if (CheckIfColumnLocked(col - offset.x))
        {
            return (0.1f, -0.1f);
        }
        if (hugePieceAnchor != invalidCoords && (hugePieceAnchor.x + offset.x == col || hugePieceAnchor.x + offset.x + 1 == col))
        {
            float max = (HEIGHT - hugePieceAnchor.y - 2) * SQUARE_SIZE + 0.1f;
            float min = -((hugePieceAnchor.y) * SQUARE_SIZE + 0.1f);
            return (max, min);
        }
        return (float.MaxValue, float.MinValue);
    }

    private bool CheckIfColumnLocked(int column)
    {
        if (hugePieceAnchor != invalidCoords && (hugePieceAnchor.x == column || hugePieceAnchor.x + 1 == column))
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (data[hugePieceAnchor.x, y].type == PieceType.LOCKED || data[hugePieceAnchor.x + 1, y].type == PieceType.LOCKED
                    || data[hugePieceAnchor.x, y].type == PieceType.FREEZING || data[hugePieceAnchor.x + 1, y].type == PieceType.FREEZING)
                {
                    return true;
                }
            }
        }
        else
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (data[column, y].type == PieceType.LOCKED || data[column, y].type == PieceType.FREEZING)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Piece[] GetPiecesInColumn(int index)
    {
        var list = new List<Piece>();

        if (hugePieceAnchor != invalidCoords && (hugePieceAnchor.x + offset.x == index || hugePieceAnchor.x + offset.x == index - 1))
        {
            for (int i = 0; i < VirtualHeight; i++)
            {
                if (_virtualBoard[hugePieceAnchor.x + offset.x, i] != null)
                {
                    list.Add(_virtualBoard[hugePieceAnchor.x + offset.x, i]);
                }
                if (_virtualBoard[hugePieceAnchor.x + offset.x + 1, i] != null)
                {
                    list.Add(_virtualBoard[hugePieceAnchor.x + offset.x + 1, i]);
                }
            }
            return list.ToArray();
        }

        for (int i = 0; i < VirtualHeight; i++)
        {
            if (_virtualBoard[index, i] != null)
            {
                list.Add(_virtualBoard[index, i]);
            }
        }
        return list.ToArray();
    }

    public bool CheckIfCoordsAreOnBoard(Vector2Int coords, bool isVirtualBoard = true)
    {
        var boardCoords = (isVirtualBoard ? coords - offset : coords);
        return boardCoords.x >= 0 && boardCoords.x < WIDTH && boardCoords.y >= 0 && boardCoords.y < HEIGHT;
    }

    public void PerformMove(Piece[] movingPieces)
    {
        combo = 0;

        inputManager.OnPerformMoveStarted();

        var oldAnchor = hugePieceAnchor;
        var oldData = (PieceData[,])data.Clone();
        foreach (var piece in movingPieces)
        {
            if (piece.Type == PieceType.HUGE && CheckIfCoordsAreOnBoard(piece.coords) && CheckIfCoordsAreOnBoard(piece.newCoords))
            {
                hugePieceAnchor = oldAnchor + (piece.newCoords - piece.coords);
            }
            var coordsVirtualBoard = piece.newCoords;
            var coords = coordsVirtualBoard - offset;
            if (CheckIfCoordsAreOnBoard(coords, false))
            {
                data[coords.x, coords.y] = new PieceData(piece.Color, piece.Type, piece.EyeColor);
            }
        }

        // Clear matches
        var matches = BoardUtils.CheckMatches(this);
        if (matches.Count > 0)
        {
            level.OnFillPieceStarted(data, hugePieceAnchor);
            FillPieces(matches);
        }
        else
        {
            data = oldData;
            hugePieceAnchor = oldAnchor;
            RollbackMove(movingPieces, () =>
            {
                inputManager.OnPerformMoveCompleted();
            });
        }
    }

    private void FillPieces(List<Cluster> matches)
    {
        combo++;
        //OnFillPiecesStart?.Invoke(matches, combo);
        StartCoroutine(FillPiecesRoutine(matches));
    }

    private IEnumerator FillPiecesRoutine(List<Cluster> matches)
    {
        UpdateVirtualBoard(data, hugePieceAnchor);
        yield return new WaitForSeconds(DELAY_FILL_PIECES);

        var score = level.ScoreKeeper.Evaluate(matches, combo);
        level.ScoreKeeper.AddScore(score);

        var explosivePieceCoords = new List<(Vector2Int, PieceColor)>();
        var effectChain = new EffectChain();
        var listExplodeCoords = new List<Vector2Int>();

        var centerPos = Vector2.zero;   // Center pos of all cluster
        foreach (var cluster in matches)
        {
            centerPos += cluster.CenterPoint;
        }
        centerPos /= matches.Count;

        if (combo > 1)
        {
            effectChain.AddEffect(new ComboEffect((ComboType)Math.Min(combo, 7), centerPos));
        }

        if (matches.Count > 1)   // Have combo
        {
            var v = centerPos;
            if (combo > 1)
            {
                v.y += SQUARE_SIZE;
            }
            effectChain.AddEffect(new ComboEffect(ComboType.Combo, v));
        }

        // Clear matches
        foreach (var cluster in matches)
        {
            if (cluster.Size >= 5 && !cluster.ContainsPieceOfType(PieceType.HUGE))
            {
                // Create explosive piece
                var coords = cluster.GetCoords(cluster.Size / 2);
                var pieceData = GetPieceData(coords);
                explosivePieceCoords.Add((coords, pieceData.pieceColor));
                effectChain.AddEffect(new ComboEffect(ComboType.Super, CalcPositionFromCoords(coords + offset)));
            }
            foreach (var match in cluster.listCoords)
            {
                var pieceData = GetPieceData(match);
                if (pieceData.type == PieceType.HUGE)
                {
                    ClearHugePiece(effectChain);
                }
                else if (pieceData.type == PieceType.EXPLOSIVE)
                {
                    listExplodeCoords.Add(match);
                }
                else if (pieceData.type == PieceType.FREEZING)
                {
                    SetPiece(match, pieceData.pieceColor, PieceType.NORMAL, pieceData.eyeColor);
                }
                else if (pieceData.type == PieceType.LOCKED)
                {
                    effectChain.AddEffect(new UnlockEffect(CalcPositionFromCoords(match + offset)));
                    ClearPiece(match, effectChain);
                }
                else
                {
                    ClearPiece(match, effectChain);
                }
            }
        }

        effectChain.data = (PieceData[,])data.Clone();
        effectChain.hugePieceAnchor = hugePieceAnchor;

        if (listExplodeCoords.Count > 0)
        {
            effectChain.next = new EffectChain();
            ExplodeAtCoords(listExplodeCoords, effectChain.next);
        }

        // Create explosive piece
        foreach (var (coords, color) in explosivePieceCoords)
        {
            SetPiece(coords, color, PieceType.EXPLOSIVE, EyeColor.Green);
        }

        // Display effect
        SFXManager.Instance.PlayPopSFX(combo);
        do
        {
            UpdateVirtualBoard(effectChain.data, effectChain.hugePieceAnchor);

            foreach (var effect in effectChain.effects)
            {
                effect.Play();
            }
            if (effectChain.ContainsEffectOfType(typeof(UnlockEffect)))
            {
                SFXManager.Instance.PlayBreakLockSFX();
            }
            effectChain = effectChain.next;
            if (effectChain != null)
            {
                yield return new WaitForSeconds(DELAY_CHAIN_EFFECT);
            }
        }
        while (effectChain != null);

        // Check if can create huge piece
        bool flag = false;
        float random = Random.Range(0f, 1f);
        if (random < level.hugePieceRatio && hugePieceAnchor == invalidCoords)
        {
            int numEmptyPiece = 0;
            for (int i = 0; i < WIDTH; i++)
            {
                int num = CountEmptyPiece(i);
                if (num >= 2 && numEmptyPiece == num)
                {
                    flag = true;
                    var anchor = new Vector2Int(i - 1, HEIGHT - 2);
                    var pieceData = RandomPiece(PieceType.HUGE);
                    hugePieceAnchor = anchor;
                    break;
                }
                numEmptyPiece = num;
            }
        }

        // Fill pieces in raw board
        var moves = new List<(Vector2Int, Vector2Int)>();

        int num2 = 0;
        if (!flag && hugePieceAnchor != invalidCoords)
        {
            int count1 = 0, count2 = 0;
            for (int y = 0; y < hugePieceAnchor.y; y++)
            {
                if (data[hugePieceAnchor.x, y].type == PieceType.EMPTY)
                {
                    count1++;
                }
                if (data[hugePieceAnchor.x + 1, y].type == PieceType.EMPTY)
                {
                    count2++;
                }
            }

            num2 = Mathf.Min(count1, count2);
        }

        for (int x = 0; x < WIDTH; x++)
        {
            if (!flag && hugePieceAnchor != invalidCoords && (x == hugePieceAnchor.x || x == hugePieceAnchor.x + 1))
            {
                int num3 = 0;
                int count3 = 0;
                for (int y = hugePieceAnchor.y; y >= 0; y--)
                {
                    if (GetPieceData(x, y).type == PieceType.EMPTY)
                    {
                        count3++;
                    }

                    if (count3 >= num2)
                    {
                        num3 = y;
                        break;
                    }
                }
                moves.AddRange(FillTopDown(num3, x));
                moves.AddRange(FillBottomUp(num3 - 1, x));
            }
            else
            {
                moves.AddRange(FillTopDown(0, x));
            }
        }

        hugePieceAnchor.y -= num2;

        if (flag)
        {
            // If create new huge piece then replace the normal piece in raw board by huge piece parts
            var anchor = hugePieceAnchor;
            var pieceData = RandomPiece(PieceType.HUGE);
            foreach (var offset in offsetOfBottomLeftAnchor)
            {
                var coords = anchor + offset;
                data[coords.x, coords.y] = pieceData;
            }
        }

        UpdateVirtualBoard(data, hugePieceAnchor);

        bool[] flags = new bool[moves.Count];
        for (int i = 0; i < moves.Count; i++)
        {
            var (start, target) = moves[i];
            var piece = GetPiecesAtCoords(target, false);
            var startPoint = CalcPositionFromCoords(start + offset);
            var targetPoint = CalcPositionFromCoords(target + offset);
            int num = i;
            piece.Move(startPoint, targetPoint, FILL_SPEED, () =>
            {
                flags[num] = true;
                var flag2 = check();
                if (flag2)
                {
                    StartCoroutine(refill());
                }
            });
        }

        IEnumerator refill()
        {
            var nmatches = BoardUtils.CheckMatches(this);
            if (nmatches.Count > 0)
            {
                FillPieces(nmatches);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                inputManager.OnPerformMoveCompleted();
                level.OnFillPieceCompleted(data, hugePieceAnchor);
            }
        }

        bool check()
        {
            foreach (var flag in flags)
            {
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }
    }

    private void ExplodeAtCoords(List<Vector2Int> listCoords, EffectChain chain)
    {
        var nextExplodedCoords = new List<Vector2Int>();
        foreach (var coords in listCoords)
        {
            var pieceData = GetPieceData(coords);
            if (pieceData.pieceColor == PieceColor.NONE)
                continue;
            foreach (var offset in offsetOfExplosion)
            {
                var ncoords = coords + offset;
                if (CheckIfCoordsAreOnBoard(ncoords, false))
                {
                    var nPieceData = GetPieceData(ncoords);
                    if (nPieceData.type == PieceType.HUGE)
                    {
                        ClearHugePiece(chain);
                    }
                    else if (nPieceData.type == PieceType.FREEZING)
                    {
                        SetPiece(ncoords, nPieceData.pieceColor, PieceType.NORMAL, nPieceData.eyeColor);
                    }
                    else if (nPieceData.type == PieceType.EXPLOSIVE)
                    {
                        nextExplodedCoords.Add(ncoords);
                    }
                    else if (nPieceData.type == PieceType.LOCKED)
                    {
                        chain.AddEffect(new UnlockEffect(CalcPositionFromCoords(ncoords + offset)));
                        ClearPiece(ncoords, chain);
                    }
                    else
                    {
                        ClearPiece(ncoords, chain);
                    }
                }
            }

            Debug.Log("Add explode effect at coords " + coords);
            ClearPiece(coords, chain);
            chain.AddEffect(new ExplodeEffect(this, coords, pieceData.pieceColor));
        }

        chain.data = (PieceData[,])data.Clone();
        chain.hugePieceAnchor = hugePieceAnchor;

        if (nextExplodedCoords.Count > 0)
        {
            if (chain.next == null)
            {
                chain.next = new EffectChain();
            }
            ExplodeAtCoords(nextExplodedCoords, chain.next);
        }
    }

    private List<(Vector2Int, Vector2Int)> FillTopDown(int startRow, int column)
    {
        var moves = new List<(Vector2Int, Vector2Int)>();
        int count = 0;
        for (int y = startRow; y < HEIGHT; y++)
        {
            var pieceData = GetPieceData(column, y);
            if (pieceData.type != PieceType.EMPTY)
            {
                data[column, y - count] = data[column, y];
                if (count > 0)
                {
                    moves.Add((new Vector2Int(column, y), new Vector2Int(column, y - count)));
                }
            }
            else
            {
                count++;
            }
        }

        for (int y = 0; y < count; y++)
        {
            data[column, HEIGHT - y - 1] = RandomPiece(PieceType.NORMAL);
            moves.Add((new Vector2Int(column, HEIGHT - 1 - y + count), new Vector2Int(column, HEIGHT - 1 - y)));
        }

        return moves;
    }

    private List<(Vector2Int, Vector2Int)> FillBottomUp(int startRow, int column)
    {
        var moves = new List<(Vector2Int, Vector2Int)>();
        int count = 0;
        for (int y = startRow; y >= 0; y--)
        {
            var pieceData = GetPieceData(column, y);
            if (pieceData.type != PieceType.EMPTY)
            {
                data[column, y + count] = data[column, y];
                if (count > 0)
                {
                    moves.Add((new Vector2Int(column, y), new Vector2Int(column, y + count)));
                }
            }
            else
            {
                count++;
            }
        }

        for (int y = 0; y < count; y++)
        {
            data[column, y] = RandomPiece(PieceType.NORMAL);
            moves.Add((new Vector2Int(column, y - count), new Vector2Int(column, y)));
        }

        return moves;
    }

    private void RollbackMove(Piece[] movingPieces, Action completedCallback = null)
    {
        SFXManager.Instance.PlayBadMoveSFX();
        StartCoroutine(RollbackRoutine(ROLLBACK_SPEED));

        IEnumerator RollbackRoutine(float moveSpeed)
        {
            if (movingPieces.Length > 0)
            {
                var newOffset = movingPieces[0].offset;
                var step = moveSpeed * Time.deltaTime;
                while (newOffset.magnitude > 0.02f)
                {
                    newOffset = Vector2.Lerp(newOffset, Vector2.zero, Mathf.Min(step, 1f));
                    foreach (var piece in movingPieces)
                    {
                        piece.SetOffset(newOffset);
                    }
                    yield return null;
                }
                foreach (var piece in movingPieces)
                {
                    piece.SetOffset(Vector2.zero);
                }
            }

            completedCallback?.Invoke();
        }
    }

    public PieceData GetPieceData(Vector2Int coords)
    {
        return GetPieceData(coords.x, coords.y);
    }

    public PieceData GetPieceData(int x, int y)
    {
        return data[x, y];
    }

    public Piece GetPiecesAtCoords(Vector2Int coords, bool isVirtualBoard = true)
    {
        if (!CheckIfCoordsAreOnBoard(coords, isVirtualBoard))
        {
            return null;
        }

        if (isVirtualBoard)
        {
            return _virtualBoard[coords.x, coords.y];
        }

        return _virtualBoard[coords.x + offset.x, coords.y + offset.y];
    }

    public void SetPiece(Vector2Int coords, PieceColor color, PieceType type, EyeColor eyeColor)
    {
        data[coords.x, coords.y] = new PieceData(color, type, eyeColor);
    }

    private void ClearPiece(Vector2Int coords, EffectChain chain, bool useEffect = true)
    {
        var pieceData = GetPieceData(coords);
        if (useEffect)
        {
            if (pieceData.pieceColor != PieceColor.NONE)
            {
                //chain.AddEffect(new EyeToBottleEffect(CalcPositionFromCoords(coords + offset), pieceData.eyeColor));
                chain.AddEffect(new NormalSplodeEffect(this, coords, pieceData.pieceColor));
            }
        }
        level.OnPieceCleared(pieceData, coords, useEffect ? chain : null);
        SetPiece(coords, PieceColor.NONE, PieceType.EMPTY, EyeColor.Green);
    }

    private void ClearHugePiece(EffectChain chain)
    {
        if (hugePieceAnchor == invalidCoords)
        {
            return;
        }

        var pieceData = GetPieceData(hugePieceAnchor);
        chain.AddEffect(new HugeSplodeEffect(this, hugePieceAnchor, pieceData.pieceColor));
        ClearPiece(hugePieceAnchor, chain, false);
        foreach (var offset in offsetOfBottomLeftAnchor)
        {
            var coords = hugePieceAnchor + offset;
            SetPiece(coords, PieceColor.NONE, PieceType.EMPTY, EyeColor.Green);
        }
        hugePieceAnchor = invalidCoords;
    }

    private int CountEmptyPiece(int col)
    {
        int count = 0;
        for (int i = 0; i < HEIGHT; i++)
        {
            if (data[col, i].type == PieceType.EMPTY)
                count++;
        }

        return count;
    }

    public void OnPiecesMoveOutOfRange(Piece[] pieces, OutOfRangeType type)
    {
        var lockedPieces = GetPiecesOfType(PieceType.LOCKED);
        if (lockedPieces.Length > 0)
        {
            foreach (var piece in lockedPieces)
            {
                ((LockedPiece)piece).Warning();
            }
            SFXManager.Instance.PlayLockClankSFX();
            return;
        }

        var freezingPieces = GetPiecesOfType(PieceType.FREEZING);
        if (freezingPieces.Length > 0)
        {
            Debug.Log("Warning freezing pieces");
            return;
        }

        var hugePieces = GetPiecesOfType(PieceType.HUGE);
        if (hugePieces.Length > 0)
        {
            var effect = new HugePieceTouchBorderEffect(type, hugePieceAnchor, this);
            effect.Play();
            return;
        }

        Piece[] GetPiecesOfType(PieceType pieceType)
        {
            var list = new List<Piece>();
            foreach (Piece piece in pieces)
            {
                if (piece.Type == pieceType)
                {
                    list.Add(piece);
                }
            }
            return list.ToArray();
        }
    }

    public void LockPiece()
    {
        Vector2Int coords;
        do
        {
            coords = RandomCoords();
            if (CheckIfCoordsAreOnBoard(coords, false))
            {
                var pieceData = GetPieceData(coords);
                if (pieceData.type == PieceType.NORMAL)
                {
                    LockPieceAtCoords(coords);
                    UpdateVirtualBoard(data, hugePieceAnchor);
                    var pos = CalcPositionFromCoords(coords + offset);
                    var lockEffect = new LockEffect(pos, GetPiecesAtCoords(coords, false));
                    lockEffect.Play();
                    coords = invalidCoords;
                }
            }
        }
        while (coords != invalidCoords);
    }

    public void LockPieceAtCoords(Vector2Int coords)
    {
        var pieceData = GetPieceData(coords);
        SetPiece(coords, pieceData.pieceColor, PieceType.LOCKED, pieceData.eyeColor);
    }

    public Vector2Int RandomCoords()
    {
        return new Vector2Int(Random.Range(0, WIDTH), Random.Range(0, HEIGHT));
    }

    public void CreateHugePiece()
    {
        if (hugePieceAnchor != invalidCoords) return;

        hugePieceAnchor = new Vector2Int(Random.Range(0, WIDTH - 1), Random.Range(0, HEIGHT - 1));
        PieceData pieceData = RandomPiece(PieceType.HUGE);
        foreach(var offset in offsetOfBottomLeftAnchor)
        {
            var coords = hugePieceAnchor + offset;
            SetPiece(coords, pieceData.pieceColor, pieceData.type, pieceData.eyeColor);
        }

        UpdateVirtualBoard(data, hugePieceAnchor);
    }

    public void CreateExplosivePiece()
    {
        Vector2Int coords;
        PieceData pieceData;
        do
        {
            coords = new Vector2Int(Random.Range(0, WIDTH), Random.Range(0, HEIGHT));
            pieceData = GetPieceData(coords);
            if (pieceData.type != PieceType.NORMAL)
            {
                coords = invalidCoords;
            }
        }
        while (coords == invalidCoords);
        SetPiece(coords, pieceData.pieceColor, PieceType.EXPLOSIVE, pieceData.eyeColor);
        UpdateVirtualBoard(data, hugePieceAnchor);
    }

    public void OnEditorValidate(GameObject context)
    {
        level = context.GetComponent<LevelManager>();
        pieceCreator = context.GetComponentInChildren<PieceFactory>();
    }
}
