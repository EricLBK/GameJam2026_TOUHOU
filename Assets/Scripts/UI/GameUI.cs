using UnityEngine;

[ExecuteAlways]
public class BulletHellUILayout_UIOnly : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform leftArt;
    [SerializeField] private RectTransform playfield;
    [SerializeField] private RectTransform rightPanel;
    [SerializeField] private RectTransform rightArt;

    [Header("Playfield (aspect locked)")]
    [SerializeField] private float playAspectW = 730f;
    [SerializeField] private float playAspectH = 850f;

    [Header("Right Panel")]
    [Tooltip("Right panel target width as % of screen width (when space allows)")]
    [Range(0f, 1f)]
    [SerializeField] private float rightPanelWidthPct = 0.26f;

    [Header("Spacing (pixels)")]
    [SerializeField] private float gapAfterLeftArt = 0f;
    [SerializeField] private float gapAfterPlayfield = 0f;
    [SerializeField] private float gapAfterRightPanel = 0f;

    void Reset()
    {
        canvasRect = GetComponent<RectTransform>();
    }

    void OnEnable() => Layout();
    void OnRectTransformDimensionsChange() => Layout();

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying) Layout();
    }
#endif

    void Layout()
    {
        if (!canvasRect || !leftArt || !playfield || !rightPanel || !rightArt) return;

        float W = canvasRect.rect.width;
        float H = canvasRect.rect.height;
        if (W <= 0f || H <= 0f) return;

        float aspect = playAspectW / playAspectH;

        // 1) Playfield fills full height, width derived from aspect
        float playH = H;
        float playW = playH * aspect;

        // 2) Desired right panel width
        float desiredPanelW = rightPanelWidthPct * W;

        // 3) Remaining width goes to arts first (equal widths), then panel shrinks if needed
        float fixedGaps = gapAfterLeftArt + gapAfterPlayfield + gapAfterRightPanel;
        float remainingForArt = W - (playW + desiredPanelW + fixedGaps);

        float artW;
        float panelW;

        if (remainingForArt >= 0f)
        {
            // Enough space: keep panel at target width, split art equally
            artW = remainingForArt * 0.5f;
            panelW = desiredPanelW;
        }
        else
        {
            // Not enough space: art shrinks to 0 first...
            artW = 0f;

            // ...then panel shrinks to whatever is left
            panelW = Mathf.Max(0f, W - (playW + fixedGaps));
        }

        // ---- Apply transforms ----
        SetupFullHeightLeftAnchored(leftArt);
        SetupFullHeightLeftAnchored(playfield);
        SetupFullHeightLeftAnchored(rightPanel);
        SetupFullHeightLeftAnchored(rightArt);

        leftArt.sizeDelta = new Vector2(artW, 0f);
        playfield.sizeDelta = new Vector2(playW, 0f);     // height is driven by anchors (full height)
        rightPanel.sizeDelta = new Vector2(panelW, 0f);
        rightArt.sizeDelta = new Vector2(artW, 0f);

        // Layout left -> right
        float x = 0f;

        leftArt.anchoredPosition = new Vector2(x, 0f);
        x += artW + gapAfterLeftArt;

        playfield.anchoredPosition = new Vector2(x, 0f);
        x += playW + gapAfterPlayfield;

        rightPanel.anchoredPosition = new Vector2(x, 0f);
        x += panelW + gapAfterRightPanel;

        rightArt.anchoredPosition = new Vector2(x, 0f);
    }

    static void SetupFullHeightLeftAnchored(RectTransform rt)
    {
        // Left anchored, full height
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.offsetMin = new Vector2(rt.offsetMin.x, 0f);
        rt.offsetMax = new Vector2(rt.offsetMax.x, 0f);
    }
}
