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

    [Tooltip("Playfield height as % of screen height")]
    [Range(0f, 1f)]
    [SerializeField] private float playHeightPct = 0.93f;

    [Tooltip("Playfield width cap as % of screen width")]
    [Range(0f, 1f)]
    [SerializeField] private float playMaxWidthPct = 0.45f;

    [Header("Right Panel")]
    [Tooltip("Right panel target width as % of screen width")]
    [Range(0f, 1f)]
    [SerializeField] private float rightPanelWidthPct = 0.26f;

    [Header("Spacing (in pixels at current resolution)")]
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

        // 1) Playfield target size (aspect preserved)
        float targetPlayH = playHeightPct * H;
        float playW_fromH = targetPlayH * aspect;

        float maxPlayW = playMaxWidthPct * W;

        float playW = playW_fromH;
        float playH = targetPlayH;

        if (playW > maxPlayW)
        {
            playW = maxPlayW;
            playH = playW / aspect;
        }

        // 2) Right panel desired width
        float desiredRightPanelW = rightPanelWidthPct * W;

        // 3) Allocate remaining width to art first. If not enough, shrink right panel last.
        float fixedGaps = gapAfterLeftArt + gapAfterPlayfield + gapAfterRightPanel;
        float remainingForArt = W - (playW + desiredRightPanelW + fixedGaps);

        float artW;
        float panelW;

        if (remainingForArt >= 0f)
        {
            // Plenty of room: keep panel at target, split art evenly
            artW = remainingForArt * 0.5f;
            panelW = desiredRightPanelW;
        }
        else
        {
            // Tight: art shrinks to 0 first, then panel shrinks
            artW = 0f;
            panelW = Mathf.Max(0f, W - (playW + fixedGaps));
        }

        // ---- Apply transforms ----
        SetupFullHeightLeftAnchored(leftArt);
        SetupFullHeightLeftAnchored(rightPanel);
        SetupFullHeightLeftAnchored(rightArt);
        SetupCenteredLeftAnchored(playfield);

        leftArt.sizeDelta = new Vector2(artW, 0f);
        rightArt.sizeDelta = new Vector2(artW, 0f);
        rightPanel.sizeDelta = new Vector2(panelW, 0f);
        playfield.sizeDelta = new Vector2(playW, playH);

        // Lay out left -> right in one row
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
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.offsetMin = new Vector2(rt.offsetMin.x, 0f);
        rt.offsetMax = new Vector2(rt.offsetMax.x, 0f);
    }

    static void SetupCenteredLeftAnchored(RectTransform rt)
    {
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(0f, 0.5f);
    }
}
