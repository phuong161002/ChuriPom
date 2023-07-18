using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStarTimer : MonoBehaviour
{
    private float elapsed;
    private float totalTime;
    private float star1Time, star2Time, star3Time;


    [SerializeField] Text tTimeRemaining;
    [SerializeField] Image star1;
    [SerializeField] Image star2;
    [SerializeField] Image star3;

    int seconds;

    public float ElapsedSeconds => elapsed;
    float factor = 1f;

    public void Setup(float star1Time, float star2Time, float star3Time)
    {
        Debug.Log(star1Time);
        elapsed = 0f;
        totalTime = star1Time + star2Time + star3Time;
        this.star1Time = star1Time;
        this.star2Time = star2Time;
        this.star3Time = star3Time;
        star1.fillAmount = 1;
        star2.fillAmount = 1;
        star3.fillAmount = 1;
        factor = 1f;
        tTimeRemaining.text = SecondsToString((int)totalTime);
    }

    private void Update()
    {
        elapsed += Time.deltaTime * factor;
        float remainingTime = Mathf.Max(totalTime - elapsed, 0);
        if((int) remainingTime != seconds)
        {
            seconds = (int)remainingTime;
            tTimeRemaining.text = SecondsToString(seconds);
        }
        star1.fillAmount = Mathf.Clamp((remainingTime - star2Time - star3Time) / star1Time, 0, 1);
        star2.fillAmount = Mathf.Clamp((remainingTime - star3Time) / star2Time, 0, 1);
        star3.fillAmount = Mathf.Clamp((remainingTime) / star3Time, 0, 1);
    }

    private string SecondsToString(int seconds)
    {
        return $"{seconds / 60}:{seconds % 60}";
    }

    public void Stop()
    {
        factor = 0f;
    }
}
