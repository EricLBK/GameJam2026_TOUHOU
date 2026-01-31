using System;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI charName;

    [Header("Dialogue Data")]
    public DialogueText dialogue;

    [Header("Typing")]
    [Min(0f)] public float textSpeed = 0.02f;

    public bool isFinished = true;
    public int index = 0;

    [Serializable]
    public class DialogueText
    {
        [Header("Content (same length)")]
        public string[] lines;
        public string[] names;

        public int Count
        {
            get
            {
                int a = lines == null ? 0 : lines.Length;
                int b = names == null ? 0 : names.Length;
                return Mathf.Min(a, b);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (dialogue == null) dialogue = new DialogueText();

        // Keep arrays same length in-editor
        if (dialogue.lines != null && dialogue.names != null && dialogue.lines.Length != dialogue.names.Length)
        {
            Debug.LogWarning("Dialogue lines and names must be same length.");

        }

        int count = dialogue.Count;
        if (count == 0) index = 0;
        else index = Mathf.Clamp(index, 0, count - 1);
    }
#endif

    public void ShowCurrent()
    {
        if (dialogue == null || dialogue.Count == 0)
        {
            if (dialogueText) dialogueText.text = "";
            if (charName) charName.text = "";
            isFinished = true;
            return;
        }

        index = Mathf.Clamp(index, 0, dialogue.Count - 1);

        if (dialogueText) dialogueText.text = dialogue.lines[index] ?? "";
        if (charName) charName.text = dialogue.names[index] ?? "";
        isFinished = true;
    }

    public void Next()
    {
        if (dialogue == null) return;
        int count = dialogue.Count;
        if (count == 0) return;

        index = Mathf.Min(index + 1, count - 1);
        ShowCurrent();
    }

    public void Prev()
    {
        if (dialogue == null) return;
        int count = dialogue.Count;
        if (count == 0) return;

        index = Mathf.Max(index - 1, 0);
        ShowCurrent();
    }
}
