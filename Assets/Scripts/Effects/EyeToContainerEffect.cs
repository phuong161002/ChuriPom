using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeToContainerEffect : IEffect
{
    private Vector2 position;
    private EyeColor eyeColor;
    private EyeContainer container;

    public EyeToContainerEffect(Vector2 position, EyeColor eyeColor, EyeContainer container)
    {
        this.position = position;
        this.eyeColor = eyeColor;
        this.container = container;
    }

    public void Play()
    {
        VFXManager.Instance.PlayEyeToContainerEffect(position, eyeColor, container);
    }
}
