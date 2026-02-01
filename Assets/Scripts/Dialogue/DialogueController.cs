using System.Collections;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DialogueAsset dialogue;

    [Header("Views (optional)")]
    [SerializeField] private DialogueTextView textView;
    [SerializeField] private DialoguePortraitView portraitView;

    [Header("Typing")]
    [Min(0f)] [SerializeField] private float textSpeed = 0.02f;

    [Header("Input")]
    [SerializeField] private bool clickToAdvance = true;
    [SerializeField] private int mouseButton = 0; // 0 = left

    public int Index { get; private set; } = 0;
    public bool IsFinished { get; private set; } = false;

    private Coroutine typingCo;
    private string fullLineText = "";

    private void Start()
    {
        ShowLine(0);
    }

    private void Update()
    {
        if (!clickToAdvance) return;
        if (!Input.GetKeyDown(KeyCode.Z)) return;

        HandleAdvanceInput();
    }

    public void StartDialogue(DialogueAsset asset, int startIndex = 0)
    {
        dialogue = asset;
        ShowLine(startIndex);
    }

    public void Next()
    {
        if (dialogue == null || dialogue.Count == 0) return;
        if (IsFinished) return;

        ShowLine(Index + 1);
    }

    public void Prev()
    {
        if (dialogue == null || dialogue.Count == 0) return;
        ShowLine(Index - 1);
    }

    private void HandleAdvanceInput()
    {
        // If typing is in progress, skip to end of current line.
        if (typingCo != null)
        {
            FinishTypingInstant();
            return;
        }

        // Otherwise go to next line
        Next();
    }

    private void ShowLine(int newIndex)
    {
        if (dialogue == null || dialogue.Count == 0)
        {
            IsFinished = true;
            textView?.Clear();
            return;
        }

        Index = Mathf.Clamp(newIndex, 0, dialogue.Count - 1);
        IsFinished = (Index >= dialogue.Count - 1);

        DialogueLine line = dialogue.GetLine(Index);
        if (line == null)
        {
            IsFinished = true;
            textView?.Clear();
            return;
        }

        // Stop previous typing
        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = null;

        // Apply visuals for this line
        portraitView?.Apply(line);

        // Typewriter
        textView?.SetName(line.name);
        fullLineText = line.text ?? "";
        textView?.SetBody("");
        typingCo = StartCoroutine(TypeLine(fullLineText));
    }

    private IEnumerator TypeLine(string s)
    {
        if (textView == null)
        {
            typingCo = null;
            yield break;
        }

        for (int i = 0; i < s.Length; i++)
        {
            textView.SetBody(textView.GetBody() + s[i]);
            if (textSpeed > 0f) yield return new WaitForSeconds(textSpeed);
            else yield return null; // still yields 1 frame
        }

        typingCo = null;
    }

    private void FinishTypingInstant()
    {
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }
        textView?.SetBody(fullLineText);
    }
}
