using UnityEngine;

public class FieldOfPlayBounds : MonoBehaviour
{
    public static FieldOfPlayBounds Instance { get; private set; }

    // This Rect will hold x, y (bottom-left corner) and width, height
    public Rect Bounds { get; private set; }

    private readonly float _targetAspectRatio = 730f / 850f;

    private void Awake()
    {
        // Singleton Pattern setup
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        CalculateBoundaries();
    }

    private void CalculateBoundaries()
    {
        // Note: Keeping your original logic of using Screen.width/2 as the base unit
        float halfScreenHeight = Screen.height / 2.0f;
        float halfScreenWidth = Screen.width / 2.0f;
        
        float currentScreenRatio = halfScreenHeight / halfScreenWidth;

        float fopHalfWidth;
        float fopHalfHeight;

        if (currentScreenRatio > _targetAspectRatio)
        {
            // Screen is too tall: Width is the constraint
            fopHalfWidth = halfScreenWidth * 0.45f;
            fopHalfHeight = fopHalfWidth / _targetAspectRatio;
        }
        else
        {
            // Screen is too wide: Height is the constraint
            fopHalfHeight = halfScreenHeight * 0.93f;
            fopHalfWidth = fopHalfHeight * _targetAspectRatio;
        }

        // Construct a Rect centered at (0,0). 
        // Arguments: x (left), y (bottom), width, height
        Bounds = new Rect(
            -fopHalfWidth,      // xMin
            -fopHalfHeight,     // yMin
            fopHalfWidth * 2,   // Total Width
            fopHalfHeight * 2   // Total Height
        );
    }
}