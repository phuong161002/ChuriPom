using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : SingletonMonobehavior<CameraShaker>
{
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        var originPos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originPos;
    }
}
