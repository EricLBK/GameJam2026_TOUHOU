using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float normalSpeed = 400.0f;
        [SerializeField] private float focusSpeed = 160.0f;
        [SerializeField] public float xBound = 600.0f;
        [SerializeField] public float yBound = 720.0f;

        private float _screenHeight;
        private float _screenWidth;

        private float _FOPheight;
        private float _FOPwidth;
        
        private float FOPaspectRatio = 730f/850f; // the ratio of 730/850 (H/W) needs to be maintained for the field of play


        [SerializeField] private GameObject hitBoxSprite;
        private bool _isHitBoxSpriteNotNull;

        private void Start()
        {
            _isHitBoxSpriteNotNull = hitBoxSprite != null;
            _screenHeight = Screen.height/2;
            _screenWidth = Screen.width/2;

            _FOPheight = _screenHeight*0.93f*2;
            
            _FOPwidth = 1f/FOPaspectRatio*_FOPheight;
            Debug.Log(_FOPheight);
            Debug.Log(_FOPheight);
            
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
            var clampedX = Mathf.Clamp(newPos.x, -_FOPwidth, _FOPwidth);
            var clampedY = Mathf.Clamp(newPos.y, -_FOPheight, _FOPheight);
            var clampedPos = new Vector3(clampedX,clampedY,newPos.z);



            transform.position = clampedPos;
        }
    }
}