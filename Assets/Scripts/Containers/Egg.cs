using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Egg : EyeContainer
{
    public int Volumn { get => volumn; }
    public int MaxVolumn { get => maxVolumn; }

    public PieceColor Color { get; private set; }

    private int maxVolumn = 100;
    private int volumn;

    [SerializeField] Image[] eggImages;
    [SerializeField] Image slideImage;
    [SerializeField] Transform pieceTransform;
    [SerializeField] Transform pieceEndPoint;
    [SerializeField] ParticleSystem explodeEffect;
    [SerializeField] GameObject originEgg;
    [SerializeField] GameObject brokenEgg;

    private float percent => volumn / (float)maxVolumn;
    private bool hasBroken;

    private void OnEnable()
    {
        OnReceivedAllEyes += Egg_OnReceivedAllEyes;
    }

    private void OnDisable()
    {
        OnReceivedAllEyes -= Egg_OnReceivedAllEyes;
    }

    private void Egg_OnReceivedAllEyes(EyeContainer obj)
    {
        if (IsFull() && !hasBroken)
        {
            hasBroken = true;
            Break();
        }
    }

    public void Setup(PieceColor pieceColor, int maxVolumn)
    {
        hasBroken = false;
        this.Color = pieceColor;
        this.maxVolumn = maxVolumn;
        volumn = 0;
        var color = GameAssets.GetColorByPieceColor(Color);
        foreach (var image in eggImages)
        {
            image.color = color;
        }
        GetComponentInChildren<ColorSetter>().SetColor(color);
        slideImage.fillAmount = percent;
        pieceTransform.gameObject.SetActive(false);
        originEgg.SetActive(true);
        brokenEgg.SetActive(false);
    }

    public override void OnEyeReceived(Eye eye)
    {
        volumn = Mathf.Clamp(volumn + eye.Value, 0, maxVolumn);
        slideImage.fillAmount = percent;
        transform.DOComplete();
        transform.DOShakePosition(0.5f, 0.03f);
        base.OnEyeReceived(eye);
    }

    public void RemoveEye(int amount)
    {
        volumn = Mathf.Clamp(volumn - amount, 0, maxVolumn);
        slideImage.fillAmount = percent;
        transform.DOComplete();
        transform.DOShakePosition(0.5f, 0.03f);
    }

    public void Break(Action completedCallback = null)
    {
        StartCoroutine(BreakRoutine());

        IEnumerator BreakRoutine()
        {
            yield return ShakeRoutine(1f, 0.1f);
            explodeEffect.Play();
            brokenEgg.SetActive(true);
            originEgg.SetActive(false);
            pieceTransform.gameObject.SetActive(true);
            pieceTransform.DOMove(pieceEndPoint.position, 1f).From(transform.position).onComplete += () =>
            {
                pieceTransform.GetComponent<Animator>().Play("Piece");
            };
            completedCallback?.Invoke();
        }
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        var originPos = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            transform.position = originPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originPos;
    }

    public bool IsFull()
    {
        return volumn >= maxVolumn;
    }
}
