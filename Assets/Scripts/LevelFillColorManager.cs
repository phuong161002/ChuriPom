using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelFillColorManager : LevelManager
{
    private Dictionary<PieceColor, Egg> eggDict;
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private Transform eggsTransform;
    [SerializeField] private float width = 3f;

    private Dictionary<PieceColor, int> colorCounter;

    public Egg GetEgg(PieceColor pieceColor)
    {
        if (eggDict.TryGetValue(pieceColor, out Egg egg))
        {
            return egg;
        }
        return null;
    }

    public override void SetupLevel(LevelData levelData)
    {
        base.SetupLevel(levelData);
        eggDict = new Dictionary<PieceColor, Egg>();
        colorCounter = new Dictionary<PieceColor, int>();
        foreach (var colorData in levelData.colorData)
        {
            var gO = Instantiate(eggPrefab, eggsTransform);
            var egg = gO.GetComponent<Egg>();
            egg.Setup(colorData.pieceColor, colorData.quantity);
            eggDict.Add(colorData.pieceColor, egg);
            colorCounter.Add(colorData.pieceColor, 0);
        }

        var eggPosition = (Vector2)eggsTransform.position - new Vector2(width / 2, 0);
        var delta = width / (eggDict.Count - 1);
        foreach (var egg in eggDict.Values)
        {
            egg.transform.position = eggPosition;
            eggPosition += new Vector2(delta, 0);
        }
        typeLevel = LevelType.FillColor;
    }

    public override void OnPieceCleared(PieceData pieceData, Vector2Int coords, EffectChain effectChain)
    {
        //base.OnPieceCleared(pieceData, coords, effectChain);
        if (effectChain == null)
        {
            return;
        }

        var egg = GetEgg(pieceData.pieceColor);
        if (egg != null && !egg.IsFull())
        {
            colorCounter[pieceData.pieceColor]++;
            effectChain.AddEffect(new EyeToContainerEffect(board.CalcPositionFromCoords(coords + board.offset), pieceData.eyeColor, egg));
        }
    }

    public override void OnFillPieceCompleted(PieceData[,] data, Vector2Int hugePieceAnchor)
    {
        if (CheckIfFillAllColors())
        {
            inputManager.Disable();
            GameWin();
            return;
        }

        if (BoardUtils.GetAvailableMove(board.data, board.hugePieceAnchor) == null)
        {
            GameLose();
        }
        inputManager.OnPerformMoveCompleted();
    }

    private bool CheckIfFillAllColors()
    {
        foreach (var egg in eggDict.Values)
        {
            if (colorCounter[egg.Color] < egg.MaxVolumn)
            {
                return false;
            }
        }

        return true;
    }


    public override void OnClickHintButton()
    {
    }
}