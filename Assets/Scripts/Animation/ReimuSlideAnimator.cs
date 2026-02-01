using UnityEngine;

public class ReimuSlideAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Input")]
    [Tooltip("Deadzone to avoid jitter from controllers.")]
    [SerializeField] private float deadZone = 0.1f;

    private int _slideLeftHash;
    private int _slideRightHash;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        _slideLeftHash  = Animator.StringToHash("SlideLeft");
        _slideRightHash = Animator.StringToHash("SlideRight");
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        bool slideLeft  = horizontal < -deadZone;
        bool slideRight = horizontal >  deadZone;

        animator.SetBool(_slideLeftHash, slideLeft);
        animator.SetBool(_slideRightHash, slideRight);
    }
}
