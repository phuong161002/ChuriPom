using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move
{
    public int coord;
    public RollType type;
    public int offset;
}

public static class BoardUtils 
{
    public static List<Cluster> CheckMatches(Board board)
    {
        var matches = new List<Cluster>();
        var data = board.data;
        var visited = new bool[data.GetLength(0), data.GetLength(1)];

        for (int y = 0; y < Board.HEIGHT; y++)
        {
            for (int x = 0; x < Board.WIDTH; x++)
            {
                if (!visited[x, y])
                {
                    var cluster = new Cluster(board);
                    var pieceData = data[x, y];
                    // DFS
                    var stack = new Stack<Vector2Int>();
                    stack.Push(new Vector2Int(x, y));
                    while (stack.Count > 0)
                    {
                        var coords = stack.Pop();
                        if (!visited[coords.x, coords.y])
                        {
                            visited[coords.x, coords.y] = true;
                            cluster.AddCoords(coords);
                            for (int i = 0; i < 4; i++)
                            {
                                var direction = Board.directions[i];
                                var neighbor = coords + direction;
                                if (board.CheckIfCoordsAreOnBoard(neighbor, false) && !visited[neighbor.x, neighbor.y])
                                {
                                    var nPieceData = data[neighbor.x, neighbor.y];
                                    if (nPieceData.pieceColor == pieceData.pieceColor)
                                    {
                                        stack.Push(new Vector2Int(neighbor.x, neighbor.y));
                                    }
                                }
                            }
                        }
                    }

                    bool containsHugePiece = cluster.ContainsPieceOfType(PieceType.HUGE);

                    int minMatches = containsHugePiece ? 6 : 3;
                    if (cluster.Size >= minMatches)
                    {
                        matches.Add(cluster);
                    }
                }
            }
        }
        return matches;
    }

    public static Move GetAvailableMove(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        int WIDTH = data.GetLength(0), HEIGHT = data.GetLength(1);
        bool haveHugePiece = hugePieceAnchor != Board.invalidCoords;
        int numX = !haveHugePiece ? WIDTH : WIDTH - 1;
        int numY = !haveHugePiece ? HEIGHT : HEIGHT - 1;

        for (int x = 0; x < numX; x++)
        {
            int col = !haveHugePiece || x <= hugePieceAnchor.x ? x : x + 1;
            var (min, max) = GetMoveRangeOfColumn(col);

            for (int offsetY = min; offsetY <= max; offsetY++)
            {
                if (offsetY == 0)
                    continue;

                var newData = MoveColumn(col, offsetY, data, hugePieceAnchor);
                if (CheckIfHaveMatches(newData, hugePieceAnchor))
                {
                    return new Move()
                    {
                        coord = col,
                        type = RollType.VERTICAL,
                        offset = offsetY,
                    };
                }
            }
        }

        for (int y = 0; y < numY; y++)
        {
            int row = !haveHugePiece || y <= hugePieceAnchor.y ? y : y + 1;
            var (min, max) = GetMoveRangeOfRow(row);

            for (int offsetX = min; offsetX <= max; offsetX++)
            {
                if (offsetX == 0)
                    continue;

                var newData = MoveRow(row, offsetX, data, hugePieceAnchor);
                if (CheckIfHaveMatches(newData, hugePieceAnchor))
                {
                    return new Move()
                    {
                        coord = row,
                        type = RollType.HORIZONTAL,
                        offset = offsetX,
                    };
                }
            }
        }

        (int, int) GetMoveRangeOfRow(int row)
        {
            if (haveHugePiece && (row == hugePieceAnchor.y || row == hugePieceAnchor.y + 1))
            {
                if (GetLockedItemIndexInRow(hugePieceAnchor.y) != -1 || GetLockedItemIndexInRow(hugePieceAnchor.y + 1) != -1)
                {
                    return (0, 0);
                }

                return (-hugePieceAnchor.x, WIDTH - hugePieceAnchor.x - 2);
            }

            if (GetLockedItemIndexInRow(row) != -1)
            {
                return (0, 0);
            }

            return (1, 5);
        }

        (int, int) GetMoveRangeOfColumn(int column)
        {
            if (haveHugePiece && (column == hugePieceAnchor.x || column == hugePieceAnchor.x + 1))
            {
                if (GetLockedItemIndexInColumn(hugePieceAnchor.x) != -1 || GetLockedItemIndexInColumn(hugePieceAnchor.x + 1) != -1)
                {
                    return (0, 0);
                }

                return (-hugePieceAnchor.y, HEIGHT - hugePieceAnchor.y - 2);
            }

            if (GetLockedItemIndexInColumn(column) != -1)
            {
                return (0, 0);
            }

            return (1, 5);
        }

        int GetLockedItemIndexInRow(int row)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (data[x, row].type == PieceType.LOCKED || data[x, row].type == PieceType.FREEZING)
                {
                    return x;
                }
            }
            return -1;
        }

        int GetLockedItemIndexInColumn(int col)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (data[col, y].type == PieceType.LOCKED || data[col, y].type == PieceType.FREEZING)
                {
                    return y;
                }
            }
            return -1;
        }


        return null;
    }

    public static PieceData[,] MoveColumn(int col, int offset, PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        var newData = (PieceData[,])data.Clone();
        var HEIGHT = data.GetLength(1);

        if (hugePieceAnchor != Board.invalidCoords && (col == hugePieceAnchor.x || col == hugePieceAnchor.x + 1))
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                newData[hugePieceAnchor.x, (y + offset + HEIGHT) % HEIGHT] = data[hugePieceAnchor.x, y];
                newData[hugePieceAnchor.x + 1, (y + offset + HEIGHT) % HEIGHT] = data[hugePieceAnchor.x + 1, y];
            }
            return newData;
        }
        for (int y = 0; y < HEIGHT; y++)
        {
            newData[col, (y + offset + HEIGHT) % HEIGHT] = data[col, y];
        }

        return newData;
    }

    public static PieceData[,] MoveRow(int row, int offset, PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        var newData = (PieceData[,])data.Clone();
        var WIDTH = data.GetLength(0);
        if (hugePieceAnchor != Board.invalidCoords && (row == hugePieceAnchor.y || row == hugePieceAnchor.y + 1))
        {
            for (int x = 0; x < WIDTH; x++)
            {
                newData[(x + offset + WIDTH) % WIDTH, hugePieceAnchor.y] = data[x, hugePieceAnchor.y];
                newData[(x + offset + WIDTH) % WIDTH, hugePieceAnchor.y + 1] = data[x, hugePieceAnchor.y + 1];
            }
            return newData;
        }
        for (int x = 0; x < WIDTH; x++)
        {
            newData[(x + offset + WIDTH) % WIDTH, row] = data[x, row];
        }

        return newData;
    }

    private static bool CheckIfHaveMatches(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        var WIDTH = data.GetLength(0);
        var HEIGHT = data.GetLength(1);
        var visited = new bool[WIDTH, HEIGHT];
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (!visited[x, y])
                {
                    var listCoords = new List<Vector2Int>();
                    var color = data[x, y].pieceColor;
                    var type = data[x, y].type;
                    // DFS
                    var stack = new Stack<Vector2Int>();
                    stack.Push(new Vector2Int(x, y));
                    while (stack.Count > 0)
                    {
                        var coords = stack.Pop();
                        if (!visited[coords.x, coords.y])
                        {
                            visited[coords.x, coords.y] = true;
                            listCoords.Add(coords);
                            for (int i = 0; i < 4; i++)
                            {
                                var direction = Board.directions[i];
                                var neighbor = coords + direction;
                                if (neighbor.x >= 0 && neighbor.x < WIDTH && neighbor.y >= 0 && neighbor.y < HEIGHT
                                    && !visited[neighbor.x, neighbor.y])
                                {
                                    if (color == data[neighbor.x, neighbor.y].pieceColor)
                                    {
                                        stack.Push(new Vector2Int(neighbor.x, neighbor.y));
                                    }
                                }
                            }
                        }
                    }

                    bool containsHugePiece = listCoords.Any(c => data[c.x, c.y].type == PieceType.HUGE);

                    int minMatches = containsHugePiece ? 6 : 3;
                    if (listCoords.Count >= minMatches)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
