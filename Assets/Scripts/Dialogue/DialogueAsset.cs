using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Asset", fileName = "DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    public DialogueLine[] lines;

    public int Count => lines == null ? 0 : lines.Length;

    public DialogueLine GetLine(int index)
    {
        if (lines == null || lines.Length == 0) return null;
        index = Mathf.Clamp(index, 0, lines.Length - 1);
        return lines[index];
    }
}
