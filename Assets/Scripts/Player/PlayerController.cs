using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float normalSpeed = 400.0f;
        [SerializeField] private float focusSpeed = 160.0f;
        [SerializeField] public float xBound = 600.0f;
        [SerializeField] public float yBound = 720.0f;

        [SerializeField] private GameObject hitBoxSprite;
        private bool _isHitBoxSpriteNotNull;

        private void Start()
        {
            _isHitBoxSpriteNotNull = hitBoxSprite != null;
        }

        private void Update()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            var inputDir = new Vector2(x, y);

            // Otherwise vertical + horizontal make you move ~1.41x faster
            if (inputDir.sqrMagnitude > 1)
            {
                inputDir.Normalize();
            }

            var isFocusing = Input.GetKey(KeyCode.LeftShift);
            var curSpeed = isFocusing ? focusSpeed : normalSpeed;

            if (_isHitBoxSpriteNotNull)
            {
                hitBoxSprite.SetActive(isFocusing);
            }

            Vector3 displacement = inputDir * (curSpeed * Time.deltaTime);
            var newPos = transform.position + displacement;

            //clamping character movement
            var clampedX = Mathf.Clamp(newPos.x, -xBound, xBound);
            var clampedY = Mathf.Clamp(newPos.y, -yBound, yBound);
            var clampedPos = new Vector3(clampedX,clampedY,newPos.z);



            transform.position = clampedPos;
        }
    }
}