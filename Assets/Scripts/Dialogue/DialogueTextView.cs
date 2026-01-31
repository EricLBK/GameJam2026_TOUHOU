using TMPro;
using UnityEngine;

public class DialogueTextView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bodyText;

    public void SetName(string name)
    {
        if (nameText) nameText.text = name ?? "";
    }

    public void SetBody(string body)
    {
        if (bodyText) bodyText.text = body ?? "";
    }

    public string GetBody()
    {
        return bodyText ? bodyText.text : "";
    }

    public void Clear()
    {
        if (nameText) nameText.text = "";
        if (bodyText) bodyText.text = "";
    }
}
