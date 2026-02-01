using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDimView : MonoBehaviour
{
    [SerializeField] private Image dimImage;
    [SerializeField] private float fadeDuration = 0.3f;
    [Range(0f, 1f)] public float targetAlpha = 0.6f;

    private Coroutine fadeCo;

    public void FadeIn()
    {
        FadeTo(targetAlpha);
    }

    public void FadeOut()
    {
        FadeTo(0f);
    }

    private void FadeTo(float alpha)
    {
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeRoutine(alpha));
    }

    private IEnumerator FadeRoutine(float target)
    {
        float start = dimImage.color.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, t / fadeDuration);
            dimImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        dimImage.color = new Color(0, 0, 0, target);
        fadeCo = null;
    }
}
