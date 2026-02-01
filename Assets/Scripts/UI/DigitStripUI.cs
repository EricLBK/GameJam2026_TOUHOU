using UnityEngine;
using UnityEngine.UI;

public class DigitStripUI : MonoBehaviour
{
    [Header("Slots (left to right)")]
    [SerializeField] private Image[] slots;

    [Header("Sprites")]
    [SerializeField] private Sprite[] digitSprites = new Sprite[10]; // 0..9
    [SerializeField] private Sprite xSprite; // optional, for 'x'

    [Header("Behavior")]
    [SerializeField] private bool rightAlign = true;
    [SerializeField] private bool padWithZeros = false;

    void Awake()
    {
        // Auto-fill slots from children if not assigned
        if (slots == null || slots.Length == 0)
        {
            slots = GetComponentsInChildren<Image>(includeInactive: true);
            // This includes the parent Image if you had one; usually you don't.
            // If you do, remove it:
            if (slots.Length > 0 && slots[0].transform == transform)
            {
                var trimmed = new Image[slots.Length - 1];
                for (int i = 1; i < slots.Length; i++) trimmed[i - 1] = slots[i];
                slots = trimmed;
            }
        }
    }

    public void SetNumber(long value, int minDigits = 1)
    {
        if (value < 0) value = 0;
        string s = value.ToString();
        if (s.Length < minDigits)
            s = s.PadLeft(minDigits, padWithZeros ? '0' : ' ');

        SetString(s);
    }

    public void SetMultiplier(int m)
    {
        SetString("x" + Mathf.Max(0, m).ToString());
    }

    public void SetString(string s)
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning($"{name}: DigitStripUI has no slots assigned/found.");
            return;
        }

        // Validate sprites
        if (digitSprites == null || digitSprites.Length < 10)
        {
            Debug.LogWarning($"{name}: digitSprites must have 10 entries (0-9).");
            return;
        }

        // clear
        for (int i = 0; i < slots.Length; i++)
            if (slots[i]) slots[i].gameObject.SetActive(false);

        if (string.IsNullOrEmpty(s)) return;

        int start = rightAlign ? Mathf.Max(0, slots.Length - s.Length) : 0;

        for (int i = 0; i < s.Length; i++)
        {
            int slotIndex = start + i;
            if (slotIndex < 0 || slotIndex >= slots.Length) continue;

            var img = slots[slotIndex];
            if (!img) continue;

            char c = s[i];

            if (c >= '0' && c <= '9')
            {
                img.sprite = digitSprites[c - '0'];
                img.gameObject.SetActive(true);
            }
            else if ((c == 'x' || c == 'X') && xSprite != null)
            {
                img.sprite = xSprite;
                img.gameObject.SetActive(true);
            }
        }
    }
}
