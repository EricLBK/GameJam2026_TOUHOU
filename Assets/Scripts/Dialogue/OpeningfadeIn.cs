using NUnit.Framework;
using UnityEngine;

public class OpeningfadeIn : MonoBehaviour
{
    [SerializeField] private ScreenDimView screen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen.FadeIn();
        Debug.Log("it faded in");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
