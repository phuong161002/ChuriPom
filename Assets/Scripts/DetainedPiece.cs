using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetainedPiece : MonoBehaviour
{
    public void ReleasePiece()
    {
        GetComponent<Animator>().Play("Happy");
    }
}
