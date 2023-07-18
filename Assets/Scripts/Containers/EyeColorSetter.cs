using System.Collections.Generic;
using UnityEngine;

public class EyeColorSetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] eyeSpriteRenderers;

    public void SetEyeColor(Sprite eyeSprite)
    {
        foreach(var renderer in eyeSpriteRenderers)
        {
            renderer.sprite = eyeSprite;
        }
    }
}
