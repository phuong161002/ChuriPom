using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWarningEffect : IEffect
{
    private LockedPiece lockedPiece;

    public LockWarningEffect(LockedPiece lockedPiece)
    {
        this.lockedPiece = lockedPiece;
    }

    public void Play()
    {
        lockedPiece.Warning();
    }
}
