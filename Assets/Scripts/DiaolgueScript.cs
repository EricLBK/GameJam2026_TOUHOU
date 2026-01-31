using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI charName;

    [Header("Content (same length)")]
    public string[] lines;
    public string[] names;

    [Header("Typing")]
    public float textSpeed = 0.02f;

    // Used by other scripts to wait for dialogue completion / react to progress
    public bool isFinished = false;
    public int index = 0;

    void Start()
    {
        dialogueText.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return; //Either it is pressed down this frame or it is not

        // If current line is fully shown, go to next line; otherwise skip typing.
        if (dialogueText.text == lines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            dialogueText.text = lines[index];
        }
    }

    void StartDialogue()
    {
        index = 0;
        isFinished = false;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        charName.text = names[index];
        dialogueText.text = "";

        foreach (char letter in lines[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            isFinished = true;
            gameObject.SetActive(false); // hides the dialogue box object
        }
    }
}
