using UnityEngine;

public class SetupCamera : MonoBehaviour
{
    private float screenHeight = Screen.height/2;
    private float screenWidth = Screen.width/2;
    
    private void Start()
    {
        gameObject.GetComponent<Camera>().orthographic = true;
        gameObject.GetComponent<Camera>().orthographicSize = (float)Screen.height / 2;

        //dimensions
        float leftOfrightBanner = screenHeight*(0.875f)/2; //right side of right banner is screenHeight
        float rightOfleftBanner = -leftOfrightBanner; //same width but on the opposite side, left side of left banner is -screenHeight
    }

}
