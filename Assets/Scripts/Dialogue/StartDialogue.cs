using UnityEngine;

public class StartDialogue : MonoBehaviour

{
    public DialogueController hello;

    public DialogueAsset dialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hello.StartDialogue(dialogue);
        Debug.Log(dialogue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
