using System;
using UnityEngine;

public enum DialogueSlot
{
    Left,
    Right
}

[Serializable]
public class DialogueLine
{
    public DialogueSlot speaker;

    public string name;

    [TextArea(2, 6)]
    public string text;

    // Optional portrait swaps on this line (leave empty = no change)
    public string leftPortraitKey;
    public string rightPortraitKey;
}
