using System;
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

    private Dictionary<string, Sprite> map;

    private void Awake()
    {
        map = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        if (sprites == null) return;

        foreach (var ns in sprites)
        {
            if (!string.IsNullOrWhiteSpace(ns.key) && ns.sprite)
                map[ns.key] = ns.sprite;
        }
    }

    public void Apply(DialogueLine line)
    {
        if (line == null) return;

        // Optional swaps
        if (!string.IsNullOrWhiteSpace(line.leftPortraitKey))  TrySet(leftPortrait, line.leftPortraitKey);
        if (!string.IsNullOrWhiteSpace(line.rightPortraitKey)) TrySet(rightPortrait, line.rightPortraitKey);

        // Highlight speaker
        bool leftSpeaking = line.speaker == DialogueSlot.Left;
        SetDim(leftPortrait, leftSpeaking ? 0f : dimAmount);
        SetDim(rightPortrait, leftSpeaking ? dimAmount : 0f);
    }

    private void TrySet(Image img, string key)
    {
        if (!img) return;
        if (map != null && map.TryGetValue(key, out var sp) && sp) img.sprite = sp;
    }

    private void SetDim(Image img, float dim)
    {
        if (!img) return;
        float v = 1f - dim;
        img.color = new Color(v, v, v, 1f);
    }
}
