using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlink : MonoBehaviour
{
    [SerializeField] float blinkDelay = 0.3f;
    [SerializeField] SpriteRenderer eyeLidSpriteRenderer;
    [SerializeField][Range(0f, 1f)] float ratio = 0.1f;
    [SerializeField] float delay = 5f;
    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        eyeLidSpriteRenderer.enabled = false;
        blinkCoroutine = StartCoroutine(RandomBlinksRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }

    private void Blink()
    {
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        eyeLidSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(blinkDelay);
        eyeLidSpriteRenderer.enabled = false;
    }

    private IEnumerator RandomBlinksRoutine()
    {
        do
        {
            float num = Random.Range(0, 1f);
            if (num < ratio)
            {
                yield return BlinkRoutine();
            }
            yield return new WaitForSeconds(delay);
        } while (true);
    }
}
