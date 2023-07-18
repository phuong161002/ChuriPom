using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class PieceFactory : MonoBehaviour
{
    [SerializeField] private PieceGenerator[] pieceGenerators;
    [SerializeField] private Sprite[] eyeSprites;

    private PieceGenerator GetGenerator(PieceType type)
    {
        switch (type)
        {
            case PieceType.EMPTY:
            case PieceType.NORMAL:
                return pieceGenerators[0];
            case PieceType.HUGE:
                return pieceGenerators[1];
            case PieceType.LOCKED:
                return pieceGenerators[2];
            case PieceType.EXPLOSIVE:
                return pieceGenerators[3];
            case PieceType.FREEZING:
                return pieceGenerators[4];
            default:
                return pieceGenerators[0];
        }
    }

    public Piece CreatePieceOfType(PieceType type, PieceColor pieceColor, EyeColor eyeColor)
    {
        if (type == PieceType.EMPTY)
            return null;

        var generator = GetGenerator(type);
        return generator.GeneratePiece(pieceColor, eyeColor);
    }
}
