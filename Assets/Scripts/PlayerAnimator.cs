using UnityEngine;

//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int IsFallingHash = Animator.StringToHash("isFalling");
    private static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int IsCrouchingHash = Animator.StringToHash("isCrouching");

    // --  Components & References ------------------------------
    public Animator Animator {get; private set;}
    private PlayerController owner;

    // ----------------------------------------------------------

    void Awake()
    {
        //Animator = GetComponent<Animator>();
        owner = GetComponent<PlayerController>();
    }

    void Update()
    {
        bool isMoving = owner.MoveInput.sqrMagnitude > 0f;
        bool isFalling = owner.JumpController.State == JumpController.JumpState.Falling;
        bool isJumping = owner.IsJumping;
        //bool isCrouching = owner.IsCrouching;

        //Animator.SetBool(IsMovingHash, isMoving);
        //Animator.SetBool(IsFallingHash, isFalling);
        //Animator.SetBool(IsJumpingHash, isJumping);
        //Animator.SetBool(IsCrouchingHash, isCrouching);
    }
}
