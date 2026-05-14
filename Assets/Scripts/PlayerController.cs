using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : LivingEntity
{
    // --  Components & References ------------------------------
    public CharacterSelector CharacterSelector {get; private set;}
    public JumpController JumpController {get; private set;}
    private PlayerInput playerInput;

    // -- Ground Check ------------------------------------------
    private Transform groundCheck;
    private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    // -- Movement ----------------------------------------------
    public Vector2 MoveInput {get; private set;}
    public bool IsJumping => JumpController.IsJumping;
    
    // ----------------------------------------------------------

    override protected void Awake()
    {
        base.Awake();
        
        groundCheck = gameObject.transform.Find("GroundCheck");
        CharacterSelector = new(this);
        JumpController = new(this);
        playerInput = GetComponent<PlayerInput>();
        //animator = GetComponent<Animator>();

        playerInput.neverAutoSwitchControlSchemes = true;
    }
    
    void Update()
    {
        UpdateValues();

        // Atualiza os valores do JumpController (coyote time, jump buffer, etc) e tenta pular se possível.
        JumpController.Tick(Time.deltaTime, IsGrounded());
        JumpController.TryJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void UpdateValues()
    {
        //isMoving = MoveInput.sqrMagnitude > 0f;

        // ---------- Animator ----------
        //animator.SetBool("isMoving", isMoving);
        //animator.SetFloat("moveSpeed", attributes.moveSpeed.FinalValue / 3);
    }

    //Calcula e executa o movimento do jogador.
    public void HandleMovement()
    {
        float speed = Attributes.Get(AttributeType.moveSpeed).FinalValue;
        Rb.linearVelocity = new Vector3(MoveInput.x * speed, Rb.linearVelocity.y, MoveInput.y * speed);
    }

    // Verifica se o jogador está no chão usando um Raycast.
    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }


    // -- Input System Callbacks --------------------------------

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) JumpController.OnJumpPressed();
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.started) SkillHelper.HandleSkill(this);
    }

    // -- Debugging ---------------------------------------------

    void OnDrawGizmos()
    {
        if (groundCheck == null)
        {
            Transform t = transform.Find("GroundCheck");
            if (t == null) return;
            groundCheck = t;
        }
        
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
