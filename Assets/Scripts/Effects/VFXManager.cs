using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VFXManager : SingletonMonobehavior<VFXManager>
{
    [SerializeField] private GameObject splodeVFXPrefab;
    [SerializeField] private GameObject lockPrefab;
    [SerializeField] private GameObject eyeHint;
    [SerializeField] private GameObject mindBenderVFXPrefab;
    [SerializeField] private GameObject releaseLockVFXPrefab;
    [SerializeField] private GameObject comboVFXPrefab;
    [SerializeField] private GameObject hintVFXPrefab;
    [SerializeField] private GameObject hintHugeVFXPrefab;
    [SerializeField] private GameObject explodeVFXPrefab;

    [SerializeField] private PieceColorPrefab[] pieceColors;
    [SerializeField] private ComboPrefab[] comboPrefabs;

    private Dictionary<ComboType, Sprite> comboSpriteDict;

    public AnimationCurve curve;
    public float duration = 0.2f;
    public float magnitude = 0.3f;

    private void Start()
    {
        
        comboSpriteDict = new Dictionary<ComboType, Sprite>();
        foreach (var combo in comboPrefabs)
        {
            comboSpriteDict[combo.comboType] = combo.sprite;
        }
    }

    

    public void PlaySplodeVFX(PieceColor color, Vector2 position, float scale = 1f)
    {
        //if (color == PieceColor.NONE) return;
        var gO = PoolManager.Instance.ReuseObject(splodeVFXPrefab, position, Quaternion.identity);
        gO.transform.localScale = new Vector3(scale, scale, 1);
        gO.GetComponent<ColorSetter>().SetColor(GameAssets.GetColorByPieceColor(color));
        gO.SetActive(true);
        DisableAfter(3f, gO);
    }

    public void PlayExplodeVFX(Vector2 position)
    {
        var gO = PoolManager.Instance.ReuseObject(explodeVFXPrefab, position, Quaternion.identity);
        gO.SetActive(true);
        DisableAfter(1f, gO);
    }

    public void PlayComboEffect(Vector2 position, ComboType type)
    {
        var gO = PoolManager.Instance.ReuseObject(comboVFXPrefab, position, Quaternion.Euler(0f, 0f, 180f));
        gO.GetComponentInChildren<SpriteRenderer>().sprite = comboSpriteDict[type];
        gO.SetActive(true);
        gO.transform.localScale = Vector3.zero;
        gO.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
        gO.transform.DORotate(Vector3.forward, 0.2f, RotateMode.FastBeyond360);
        DisableAfter(1f, gO);
    }

    public void ShakeCamera()
    {
        CameraShaker.Instance.Shake(0.2f, 0.05f);
    }

    public void PlayMindBenderEffect(Vector2 position, RollType type)
    {
        var quaternion = Quaternion.identity;
        if (type == RollType.VERTICAL)
        {
            quaternion = Quaternion.Euler(0f, 0f, 90f);
        }
        var gO = PoolManager.Instance.ReuseObject(mindBenderVFXPrefab, position, quaternion);
        gO.SetActive(true);

        DisableAfter(0.5f, gO);
    }

    public void PlayEyeToContainerEffect(Vector2 position, EyeColor eyeColor, EyeContainer container)
    {
        var gO = PoolManager.Instance.ReuseObject(GameAssets.EyeToContainer, position, Quaternion.identity);
        gO.SetActive(true);
        var colorSetter = gO.GetComponent<EyeColorSetter>();
        var sprite = GameAssets.GetEyeSprite(eyeColor);
        colorSetter.SetEyeColor(sprite);
        var eye = gO.GetComponent<Eye>();
        eye.Setup(position);
        eye.MoveTo(container);
        container.AddEye(eye);
    }

    public void PlayReleaseLockEffect(Vector2 position)
    {
        var gO = PoolManager.Instance.ReuseObject(releaseLockVFXPrefab, position, Quaternion.identity);
        gO.SetActive(true);

        DisableAfter(2f, gO);
    }

    private void DisableAfter(float seconds, GameObject gameObject)
    {
        StartCoroutine(disable());
        IEnumerator disable()
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }
    }

    public void PlayLockPieceEffect(Vector2 position, float duration, Piece piece)
    {
        if (piece is LockedPiece lockedPiece)
        {
            lockedPiece.HideLock();
            var gO = PoolManager.Instance.ReuseObject(lockPrefab, position, Quaternion.identity);
            gO.SetActive(true);

            gO.transform.DOScale(1, duration).onComplete += () =>
            {
                lockedPiece.ShowLock();
                gO.SetActive(false);
            };
        }
    }
}
