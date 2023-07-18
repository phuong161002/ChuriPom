using System;
using System.Collections.Generic;
using UnityEngine;

public class EyeContainer : MonoBehaviour
{
    public Vector2 EndPosition => endTransform.position;

    [SerializeField] private Transform endTransform;

    protected List<Eye> comingEyes = new List<Eye>();
    public event Action<EyeContainer> OnReceivedAllEyes;

    public void AddEye(Eye eye)
    {
        comingEyes.Add(eye);
    }

    public virtual void OnEyeReceived(Eye eye)
    {
        comingEyes.Remove(eye);
        if(comingEyes.Count <= 0)
        {
            OnReceivedAllEyes?.Invoke(this);
        }
    }

    public virtual void OnGameWin()
    {
        
    }

    public virtual void OnGameLose()
    {
        
    }

    public bool HasReceivedAllEyes()
    {
        return comingEyes.Count <= 0;
    }

    private void OnDestroy()
    {
        foreach (var eye in comingEyes)
        {
            eye.gameObject.SetActive(false);
        }
    }
}
