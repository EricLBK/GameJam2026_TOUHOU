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

    public int Index { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }

    private Coroutine typingCo;
    private string fullLineText = "";

    // -----------------------
    // Public API
    // -----------------------

    public void StartDialogue(DialogueAsset asset, int startIndex = 0)
    {
        if (asset == null || asset.Count == 0) return;

        dialogue = asset;
        Index = Mathf.Clamp(startIndex, 0, dialogue.Count - 1);

        IsRunning = true;
        IsFinished = false;

        ShowLine(Index);
    }

    public void Next()
    {
        if (!IsRunning) return;
        if (dialogue == null || dialogue.Count == 0) return;

        // If currently on the LAST line → end dialogue
        if (Index >= dialogue.Count - 1)
        {
            EndDialogue();
            return;
        }

        ShowLine(Index + 1);
    }

    public void Prev()
    {
        if (!IsRunning) return;
        if (dialogue == null || dialogue.Count == 0) return;

        ShowLine(Index - 1);
    }

    public void EndDialogue()
    {
        IsRunning = false;
        IsFinished = true;

        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        textView?.Clear();
    }

    // -----------------------
    // Unity loop
    // -----------------------

    private void Update()
    {
        if (!IsRunning) return;
        if (!clickToAdvance) return;

        if (Input.GetKeyDown(KeyCode.Z))
            HandleAdvanceInput();
    }

    // -----------------------
    // Internal logic
    // -----------------------

    private void HandleAdvanceInput()
    {
        // If typing is in progress → finish instantly
        if (typingCo != null)
        {
            FinishTypingInstant();
            return;
        }

        // Otherwise advance
        Next();
    }

    private void ShowLine(int newIndex)
    {
        Index = Mathf.Clamp(newIndex, 0, dialogue.Count - 1);

        DialogueLine line = dialogue.GetLine(Index);
        if (line == null) return;

        // Stop previous typing
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        // Apply visuals
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
            if (textSpeed > 0f)
                yield return new WaitForSecondsRealtime(textSpeed);
            else
                yield return null;
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
