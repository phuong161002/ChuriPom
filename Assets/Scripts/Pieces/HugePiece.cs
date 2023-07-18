using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugePiece : Piece
{
    public Vector2Int anchor;

    public void ActiveSpriteRenderer(bool active)
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = active;
        }
    }
}
