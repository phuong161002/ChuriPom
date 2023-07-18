using System.Collections.Generic;
using UnityEngine;

public class GameAssets : SingletonMonobehavior<GameAssets>
{
    [SerializeField] EyeColorPrefab[] eyeColors;
    [SerializeField] private PieceColorPrefab[] pieceColors;
    [SerializeField] AudioClip homeMusic;
    [SerializeField] AudioClip inGameMusic;

    private Dictionary<EyeColor, Sprite> eyeSpriteDict;
    private Dictionary<PieceColor, Color> pieceColorDict;

    [SerializeField] GameObject eyeToContainerPrefab;

    public static GameObject EyeToContainer => Instance.eyeToContainerPrefab;
    public static AudioClip HomeMusic => Instance.homeMusic;
    public static AudioClip InGameMusic => Instance.inGameMusic;

    protected override void Awake()
    {
        base.Awake();
        eyeSpriteDict = new Dictionary<EyeColor, Sprite>();
        foreach(var prefab in eyeColors)
        {
            eyeSpriteDict.Add(prefab.eyeColor, prefab.sprite);
        }

        pieceColorDict = new Dictionary<PieceColor, Color>();
        foreach (var color in pieceColors)
        {
            pieceColorDict[color.pieceColor] = color.value;
        }
    }

    public static Sprite GetEyeSprite(EyeColor eyeColor)
    {
        return Instance.eyeSpriteDict[eyeColor];
    }

    public static Color GetColorByPieceColor(PieceColor pieceColor)
    {
        return Instance.pieceColorDict[pieceColor];
    }
}
