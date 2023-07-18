using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class Bottle : EyeContainer
{
    [SerializeField] private TextMeshProUGUI TextPercent;
    [SerializeField] private Image fillImage;

    public int maxVolumn = 100;
    private int volumn;

    private const float duration = 0.25f;
    private const float deltaShakeY = 0.05f;

    private bool canShake;

    public float Percent
    {
        get => (volumn / (float)maxVolumn) * 100f;
    }

    public int Volumn
    {
        get => volumn;
        set {
            volumn = value;
            TextPercent.text = Percent.ToString("N0") + "%";
        }
    }

    void Start()
    {
        canShake = true;
        fillImage.fillAmount = 0;
        TextPercent.alpha = 0f;
    }

    public void ReloadPercentNumber()
    {
        if (!IsFull())
        {
            transform.DOComplete();
            DOTween.Sequence().SetTarget(transform).Append(transform.DOLocalMoveY(transform.position.y - deltaShakeY, duration))
                .Append(transform.DOLocalMoveY(transform.position.y, duration)).Play();
            TextPercent.DOFade(0f, 2f).From(1).SetTarget(transform);
        }
        fillImage.fillAmount = Percent;
    }

    public void AddPercent(int percentPoint)
    {
        if (percentPoint > 0)
        {
            Volumn = Mathf.Clamp(Volumn + percentPoint, 0, maxVolumn);
            ReloadPercentNumber();
        }
    }
   
    public void SetMaxVolumn(int volumn)
    {
        maxVolumn = volumn;
    }

    public override void OnEyeReceived(Eye eye)
    {
        AddPercent(eye.Value);
        SFXManager.Instance.PlayBottledSFX();
        base.OnEyeReceived(eye);
    }

    public bool IsFull()
    {
        return volumn >= maxVolumn;
    }
}
