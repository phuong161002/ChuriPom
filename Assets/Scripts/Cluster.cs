using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cluster
{
    public int Size { get => listCoords.Count; }
    public readonly List<Vector2Int> listCoords;
    public readonly List<PieceData> listPiece;
    private Board board;

    public Vector2 CenterPoint
    {
        get
        {
            if (listCoords.Count == 0)
                return Vector2.zero;
            Vector2 sum = Vector2.zero;
            foreach (var coords in listCoords)
            {
                Vector2 pos = board.CalcPositionFromCoords(coords + board.offset);
                sum += pos;
            }
            return sum / listCoords.Count;
        }
    }

    public Cluster(Board board)
    {
        listCoords = new List<Vector2Int>();
        listPiece = new List<PieceData>();
        this.board = board;
    }

    public void AddCoords(Vector2Int coords)
    {
        listCoords.Add(coords);
        var pieceData = board.GetPieceData(coords);
        listPiece.Add(pieceData);
    }

    public bool ContainsPieceOfType(PieceType pieceType)
    {
        return listCoords.Any(coords =>
        {
            var pieceData = board.GetPieceData(coords);
            return pieceData.type == pieceType;
        });
    }

    public Vector2Int GetCoords(int index)
    {
        if (index < 0 || index >= Size)
        {
            return new Vector2Int(-1, -1);
        }
        return listCoords[index];
    }

    public bool ContainsRow(int row)
    {
        foreach (var coords in listCoords)
        {
            if (coords.y == row)
                return true;
        }
        return false;
    }

    public bool ContainsColumn(int col)
    {
        foreach (var coords in listCoords)
        {
            if (coords.x == col)
                return true;
        }
        return false;
    }
}
