using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePieceGenerator : PieceGenerator
{
    [SerializeField] private PieceColorPrefab[] colorPrefabs;
    [SerializeField] private GameObject explosivePiecePrefab;
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
        Debug.Log("Create explosive piece ");
        var gO = PoolManager.Instance.ReuseObject(explosivePiecePrefab, Vector3.zero, Quaternion.identity);
        gO.SetActive(true);
        gO.GetComponent<ColorSetter>().SetColor(pieceColorDict[color]);
        gO.GetComponentInChildren<EyeColorSetter>().SetEyeColor(GameAssets.GetEyeSprite(eyeColor));
        var piece = gO.GetComponent<ExplosivePiece>();
        piece.Type = PieceType.EXPLOSIVE;
        piece.Color = color;
        piece.EyeColor = eyeColor;
        return piece;
    }
}
