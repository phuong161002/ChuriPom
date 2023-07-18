using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LockBar : EyeContainer
{
    public int Size
    {
        get
        {
            return size;
        }
        set
        {
            size = value;
            float percent = _countLock / (float)Size * slideBar.maxValue;
            SetValue(percent, 0.2f);
        }
    }
    public int CountLock
    {
        get => _countLock;
        set
        {
            _countLock = value;
            float percent = _countLock / (float)Size * slideBar.maxValue;
            SetValue(percent, 0.2f);
        }
    }
    [SerializeField] private int size;
    private int _countLock;

    [SerializeField] private Slider slideBar;

    private void Start()
    {
        slideBar.value = 0f;
    }

    public void SetValue(float target, float duration)
    {
        slideBar.DOValue(target, duration);
    }

    public void ResetLockBar()
    {
        //action to lock object
        CountLock = 0;
        slideBar.DOValue(0f, 0.5f);
    }

    public override void OnEyeReceived(Eye eye)
    {
        CountLock += eye.Value;
        base.OnEyeReceived(eye);
    }

    public bool IsFull()
    {
        return CountLock >= Size;
    }
}
