using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockEffect : IEffect
{
    private Vector2 position;

    public UnlockEffect(Vector2 position)
    {
        this.position = position;
    }


    public void Play()
    {
        VFXManager.Instance.ShakeCamera();
        VFXManager.Instance.PlayReleaseLockEffect(position);
    }
}
