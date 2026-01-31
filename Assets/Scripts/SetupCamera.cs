using UnityEngine;

public class SetupCamera : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Camera>().orthographic = true;
        gameObject.GetComponent<Camera>().orthographicSize = (float)Screen.height / 2;
    }

}
