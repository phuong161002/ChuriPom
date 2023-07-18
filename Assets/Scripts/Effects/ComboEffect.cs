using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboEffect : IEffect
{
    private ComboType comboType;
    private Vector2 position;

    public ComboEffect(ComboType comboType, Vector2 position)
    {
        this.comboType = comboType;
        this.position = position;
    }

    public void Play()
    {
        VFXManager.Instance.PlayComboEffect(position, comboType);
    }
}
