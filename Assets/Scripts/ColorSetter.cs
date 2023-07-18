using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private ParticleSystem[] particleSystems;

    public void SetColor(Color color)
    {
        foreach (var renderer in spriteRenderers)
        {
            renderer.color = color;
        }
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.startColor = color;
        }
    }
}
