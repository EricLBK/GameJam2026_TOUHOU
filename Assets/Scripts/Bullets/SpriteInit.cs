using UnityEngine;

public class SpriteInit : MonoBehaviour
{
    [SerializeField] private Texture2D bulletTexture;

    [SerializeField] private int radius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (bulletTexture == null)
            return;
        var curSprite = Sprite.Create(bulletTexture, new Rect(0, 0, bulletTexture.width, bulletTexture.height),
            new Vector2(0.5f, 0.5f), 1.0f);
            Debug.Log($"The height is:{bulletTexture.height}, The width is:{bulletTexture.width}");
        gameObject.GetComponent<SpriteRenderer>().sprite = curSprite;
        transform.localScale = new Vector3(0.25f, 0.25f, 1f);
    }
    private Sprite CreateResizedSprite(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        // 1. Resize the Texture data to match the target 1 PPU dimensions
        Texture2D resizedTexture = ResizeTexture(originalTexture, targetWidth, targetHeight);

        // 2. Create the Sprite with 1 PPU
        // Now that the texture is resized, 1 PPU will result in the exact World Size you want.
        Rect rect = new Rect(0, 0, targetWidth, targetHeight);
        Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot
        
        return Sprite.Create(resizedTexture, rect, pivot, 1.0f);
    }

    // Helper function to resize texture using the GPU (very fast)
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        // Create a temporary Render Texture with the target dimensions
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        
        // Ensure accurate scaling (Point for pixel art, Bilinear for smooth)
        source.filterMode = FilterMode.Bilinear; 
        rt.filterMode = FilterMode.Bilinear;
        
        // Activate the Render Texture and copy/scale the source into it
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        
        // Create the new Texture2D and read the active Render Texture into it
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();

        // Clean up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return nTex;
    }

}
