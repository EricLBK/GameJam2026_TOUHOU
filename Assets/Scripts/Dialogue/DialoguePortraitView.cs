using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePortraitView : MonoBehaviour
{
    [Serializable]
    public struct NamedSprite
    {
        public string key;
        public Sprite sprite;
    }

    [Header("UI")]
    [SerializeField] private Image leftPortrait;
    [SerializeField] private Image rightPortrait;

    [Header("Library")]
    [SerializeField] private NamedSprite[] sprites;

    [Header("Dim")]
    [Range(0f, 1f)] public float dimAmount = 0.5f;

    [Header("Dim Animation")]
    [SerializeField] private float dimTweenSeconds = 0.12f;
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Speaker Pop Animation")]
    [SerializeField] private float popScale = 1.06f;
    [SerializeField] private float popUpSeconds = 0.06f;
    [SerializeField] private float popDownSeconds = 0.10f;

    private Dictionary<string, Sprite> map;

    // --- dim state ---
    private float _leftDim = 0f;
    private float _rightDim = 0f;
    private Coroutine _leftDimCo;
    private Coroutine _rightDimCo;

    // --- pop state ---
    private Coroutine _leftPopCo;
    private Coroutine _rightPopCo;

    // Track last line speaker for "same speaker next line" pop
    private DialogueSlot? _lastSpeaker = null;

    private void Awake()
    {
        map = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

        if (sprites != null)
        {
            foreach (var ns in sprites)
            {
                if (!string.IsNullOrWhiteSpace(ns.key) && ns.sprite)
                    map[ns.key] = ns.sprite;
            }
        }

        // Initialize visuals
        SetDimImmediate(leftPortrait, 0f);
        SetDimImmediate(rightPortrait, 0f);

        if (leftPortrait) leftPortrait.rectTransform.localScale = Vector3.one;
        if (rightPortrait) rightPortrait.rectTransform.localScale = Vector3.one;
    }

    public void Apply(DialogueLine line)
    {
        if (line == null) return;

        // Optional swaps
        if (!string.IsNullOrWhiteSpace(line.leftPortraitKey))
            TrySet(leftPortrait, line.leftPortraitKey);

        if (!string.IsNullOrWhiteSpace(line.rightPortraitKey))
            TrySet(rightPortrait, line.rightPortraitKey);

        // Speaker highlighting
        bool leftSpeaking = line.speaker == DialogueSlot.Left;

        float targetLeftDim = leftSpeaking ? 0f : dimAmount;
        float targetRightDim = leftSpeaking ? dimAmount : 0f;

        AnimateDim(leftPortrait, targetLeftDim, ref _leftDimCo, isLeft: true);
        AnimateDim(rightPortrait, targetRightDim, ref _rightDimCo, isLeft: false);

        // Pop rule: pop when speaker changes OR continues (same speaker next line)
        bool shouldPop = !_lastSpeaker.HasValue || _lastSpeaker.Value == line.speaker;

        if (shouldPop)
        {
            if (leftSpeaking) Pop(leftPortrait ? leftPortrait.rectTransform : null, ref _leftPopCo);
            else Pop(rightPortrait ? rightPortrait.rectTransform : null, ref _rightPopCo);
        }

        _lastSpeaker = line.speaker;
    }

    private void TrySet(Image img, string key)
    {
        if (!img) return;
        if (map != null && map.TryGetValue(key, out var sp) && sp)
            img.sprite = sp;
    }

    // =========================
    // Dim animation (no lambdas)
    // =========================

    private void AnimateDim(Image img, float targetDim, ref Coroutine co, bool isLeft)
    {
        if (!img) return;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(DimRoutine(img, targetDim, isLeft));
    }

    private IEnumerator DimRoutine(Image img, float targetDim, bool isLeft)
    {
        float startDim = isLeft ? _leftDim : _rightDim;

        float seconds = Mathf.Max(0.0001f, dimTweenSeconds);
        float t = 0f;

        while (t < 1f)
        {
            t += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / seconds;
            float eased = EaseOutCubic(Mathf.Clamp01(t));
            float dim = Mathf.Lerp(startDim, targetDim, eased);

            if (isLeft) _leftDim = dim;
            else _rightDim = dim;

            SetDimImmediate(img, dim);
            yield return null;
        }

        if (isLeft) _leftDim = targetDim;
        else _rightDim = targetDim;

        SetDimImmediate(img, targetDim);
    }

    private void SetDimImmediate(Image img, float dim)
    {
        if (!img) return;
        dim = Mathf.Clamp01(dim);

        float v = 1f - dim;
        img.color = new Color(v, v, v, 1f);
    }

    // =========================
    // Pop animation
    // =========================

    private void Pop(RectTransform rt, ref Coroutine co)
    {
        if (!rt) return;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(PopRoutine(rt));
    }

    private IEnumerator PopRoutine(RectTransform rt)
    {
        Vector3 baseScale = Vector3.one;
        Vector3 peak = baseScale * popScale;

        yield return ScaleRoutine(rt, rt.localScale, peak, popUpSeconds);
        yield return ScaleRoutine(rt, rt.localScale, baseScale, popDownSeconds);
    }

    private IEnumerator ScaleRoutine(RectTransform rt, Vector3 from, Vector3 to, float duration)
    {
        float seconds = Mathf.Max(0.0001f, duration);
        float t = 0f;

        while (t < 1f)
        {
            t += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / seconds;
            float eased = EaseOutBack(Mathf.Clamp01(t));
            rt.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }

        rt.localScale = to;
    }

    // =========================
    // Easing
    // =========================

    private static float EaseOutCubic(float x)
    {
        float a = 1f - x;
        return 1f - a * a * a;
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
