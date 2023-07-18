using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceGenerator : MonoBehaviour
{
    public abstract Piece GeneratePiece(PieceColor color, EyeColor eyeColor);
}