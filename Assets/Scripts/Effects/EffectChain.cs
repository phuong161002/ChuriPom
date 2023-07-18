using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChain
{
    public readonly List<IEffect> effects;
    public EffectChain next;
    public PieceData[,] data;
    public Vector2Int hugePieceAnchor;

    public EffectChain()
    {
        effects = new List<IEffect>();
    }

    public void AddEffect(IEffect effect)
    {
        effects.Add(effect);
    }

    public void Play()
    {
        foreach (var effect in effects)
        {
            effect.Play();
        }

        if (next != null)
        {
            next.Play();
        }
    }

    public bool ContainsEffectOfType(Type type)
    {
        foreach (var effect in effects)
        {
            if (effect.GetType().Equals(type))
            {
                return true;
            }
        }
        return false;
    }
}
