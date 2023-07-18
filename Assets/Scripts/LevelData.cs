using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData_SO", menuName = "Scriptable Objects/Level Data")]
public class LevelData : ScriptableObject
{
    public LevelType levelType;
    public int bottleVolumn;
    public int lockBarSize;
    public Vector2Int[] initialLockCoords;
    public ColorData[] colorData;
    public float hugePieceRatio;
    public float time;
}
