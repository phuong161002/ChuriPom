using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedPieceGenerator : PieceGenerator
{
    [SerializeField] private PieceColorPrefab[] colorPrefabs;
    [SerializeField] private GameObject lockedPiecePrefab;
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
        var gO = PoolManager.Instance.ReuseObject(lockedPiecePrefab, Vector3.zero, Quaternion.identity);
        gO.SetActive(true);
        gO.GetComponent<ColorSetter>().SetColor(pieceColorDict[color]);
        gO.GetComponentInChildren<EyeColorSetter>().SetEyeColor(GameAssets.GetEyeSprite(eyeColor));
        var piece = gO.GetComponent<LockedPiece>();
        piece.Type = PieceType.LOCKED;
        piece.Color = color;
        piece.EyeColor = eyeColor;
        return piece;
    }
}
