using UnityEngine;

public class SpriteInit : MonoBehaviour
{
    [SerializeField] private Texture2D bulletTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (bulletTexture == null)
            return;
        var curSprite = Sprite.Create(bulletTexture, new Rect(0, 0, bulletTexture.width, bulletTexture.height),
            new Vector2(0.5f, 0.5f), 1.0f);
        gameObject.GetComponent<SpriteRenderer>().sprite = curSprite;
        transform.localScale = new Vector3(0.25f, 0.25f, 1f);
    }

}
