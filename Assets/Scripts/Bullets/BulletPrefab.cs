using UnityEngine;

namespace Bullets
{
    public class BulletPrefab
    {
        public GameObject prefab;
        public float radius = 10;

        public void ApplyPropertiesTo(GameObject go)
        {
            var oldSr = prefab.GetComponent<SpriteRenderer>();
            var newSr = go.GetComponent<SpriteRenderer>();
            newSr.sprite = oldSr.sprite;
            var spriteSize = newSr.sprite.bounds.size;
            var newScale = new Vector3(radius * 2 / spriteSize.x, radius * 2 / spriteSize.y, 1f);
            go.transform.localScale = newScale;
        }
    }

    public class BulletPrefabs
    {
        public BulletPrefab blue;
        public BulletPrefab pink;
        public BulletPrefab purple;
        public BulletPrefab purple2;
        public BulletPrefab red;
        public BulletPrefab spell1;
        public BulletPrefab spell2;

        public static BulletPrefabs Load()
        {
            return new BulletPrefabs
            {
                blue = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Blue"),
                },
                red = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Red"),
                },
                purple = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Purple"),
                },
                purple2 = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Purple2"),
                },
                pink = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Pink"),
                },
                spell1 = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Reimu"),
                },
                spell2 = new BulletPrefab
                {
                    prefab = Resources.Load<GameObject>("Prefab/Bullets/Reimu2"),
                } 
            };
        }
    }
}
