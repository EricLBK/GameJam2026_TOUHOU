using System;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI charName;

    [Header("Dialogue Data")]
    public DialogueLine[] lines;

    [Header("Typing")]
    [Min(0f)] public float textSpeed = 0.02f;

    public bool isFinished = true;
    public int index = 0;

    public enum Slot { Left, Right }

    [Serializable]
    public class DialogueLine
    {
        public Slot speaker;
        public string name;

        [TextArea]
        public string text;

        public string leftPortraitKey;
        public string rightPortraitKey;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        int count = lines == null ? 0 : lines.Length;
        if (count == 0) index = 0;
        else index = Mathf.Clamp(index, 0, count - 1);
    }
#endif

    public void ShowCurrent()
    {
        if (lines == null || lines.Length == 0)
        {
            if (dialogueText) dialogueText.text = "";
            if (charName) charName.text = "";
            isFinished = true;
            return;
        }

        index = Mathf.Clamp(index, 0, lines.Length - 1);

        var line = lines[index];
        if (dialogueText) dialogueText.text = line.text ?? "";
        if (charName) charName.text = line.name ?? "";

        // isFinished should mean "dialogue ended", not "line displayed"
        isFinished = (index >= lines.Length - 1);
    }

    public void Next()
    {
        if (lines == null || lines.Length == 0) return;

        if (index < lines.Length - 1)
        {
            index++;
            ShowCurrent();
        }
        else
        {
            isFinished = true;
        }
    }

    public void Prev()
    {
        if (lines == null || lines.Length == 0) return;

        if (index > 0)
        {
            index--;
            ShowCurrent();
        }
    }
}
