using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockEffect : IEffect
{
    private Vector2 position;
    private Piece lockedPiece;

    public LockEffect(Vector2 position, Piece lockedPiece)
    {
        this.position = position;
        this.lockedPiece = lockedPiece;
    }

    public void Play()
    {
        SFXManager.Instance.PlayLockSFX();
        VFXManager.Instance.PlayLockPieceEffect(position, 0.3f, lockedPiece);
    }
}
