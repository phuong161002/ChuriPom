using System;
using UnityEngine;

public struct PieceData
{
    public PieceColor pieceColor;
    public PieceType type;
    public EyeColor eyeColor;

    public PieceData(PieceColor pieceColor, PieceType type, EyeColor eyeColor)
    {
        this.pieceColor = pieceColor;
        this.type = type;
        this.eyeColor = eyeColor;
    }
}

[System.Serializable]
public struct ColorData
{
    public PieceColor pieceColor;
    public int quantity;
}

[Serializable]
public struct PieceColorPrefab
{
    public PieceColor pieceColor;
    public Color value;
}

[Serializable]
public struct EyeColorPrefab
{
    public EyeColor eyeColor;
    public Sprite sprite;
}

[Serializable]
public struct ComboPrefab
{
    public ComboType comboType;
    public Sprite sprite;
}