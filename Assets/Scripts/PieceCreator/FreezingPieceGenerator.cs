using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingPieceGenerator : PieceGenerator
{
    [SerializeField] private PieceColorPrefab[] colorPrefabs;
    [SerializeField] private GameObject freezingPiecePrefab;
    private Dictionary<PieceColor, Color> pieceColorDict;

    private void Awake()
    {
        pieceColorDict = new Dictionary<PieceColor, Color>();
        foreach (var colorPrefab in colorPrefabs)
        {
            pieceColorDict.Add(colorPrefab.pieceColor, colorPrefab.value);
        }
    }

    public override Piece GeneratePiece(PieceColor color, EyeColor eyeColor)
    {
        var gO = PoolManager.Instance.ReuseObject(freezingPiecePrefab, Vector3.zero, Quaternion.identity);
        gO.SetActive(true);
        gO.GetComponent<ColorSetter>().SetColor(pieceColorDict[color]);
        gO.GetComponentInChildren<EyeColorSetter>().SetEyeColor(GameAssets.GetEyeSprite(eyeColor));
        var piece = gO.GetComponent<FreezingPiece>();
        piece.Type = PieceType.FREEZING;
        piece.Color = color;
        piece.EyeColor = eyeColor;
        return piece;
    }
}
