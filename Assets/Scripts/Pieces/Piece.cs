using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class Piece : MonoBehaviour
{
    public Vector2Int coords;
    public Vector2Int newCoords;
    public Vector2 postition;
    public Vector2 offset = Vector2.zero;
    private Board board;

    public PieceType Type;
    public PieceColor Color;
    public EyeColor EyeColor;

    public void Setup(Vector2Int coords, Board board)
    {
        this.coords = coords;
        this.board = board;
        transform.position = board.CalcPositionFromCoords(coords);
        postition = board.CalcPositionFromCoords(coords);
    }

    internal void SetOffset(Vector2 offset)
    {
        if (this.offset == offset) return;
        this.offset = offset;
        var offsetUnit = Vector2Int.RoundToInt(offset / Board.SQUARE_SIZE);
        newCoords = coords + offsetUnit;
        newCoords.x = (newCoords.x + board.VirtualWidth * 100) % board.VirtualWidth;
        newCoords.y = (newCoords.y + board.VirtualHeight * 100) % board.VirtualHeight;
        transform.position = board.CalcPositionFromCoords(newCoords)
            + new Vector3(offset.x - offsetUnit.x * Board.SQUARE_SIZE, offset.y - offsetUnit.y * Board.SQUARE_SIZE);
    }

    public void Move(Vector2 start, Vector2 end, float speed, Action callback = null)
    {
        StartCoroutine(MoveRoutine());

        IEnumerator MoveRoutine()
        {
            transform.position = start;

            while (transform.position != (Vector3)end)
            {
                speed += 20f * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, end, speed * Time.deltaTime);
                yield return null;
            }

            if (end.y < start.y)
            {
                float elasped = 0f;
                float duration = VFXManager.Instance.duration;
                float magnitude = VFXManager.Instance.magnitude;

                while (elasped <= duration)
                {
                    float y = VFXManager.Instance.curve.Evaluate(elasped / duration);
                    transform.position = end + new Vector2(0, y * magnitude);
                    elasped += Time.deltaTime;
                    yield return null;
                }

                transform.position = end;
            }

            callback?.Invoke();
        }
    }
}
