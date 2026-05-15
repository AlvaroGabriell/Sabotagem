using FMODUnity;
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

    // -- Audio -------------------------------------------------
    // TODO: Arrumar como o sfx é tocado
    [SerializeField] private EventReference jumpSfx, landSfx, raccoonSound;
    private float stepInterval = 0.5f;
    private float stepTimer;
    
    // ----------------------------------------------------------

    override protected void Awake()
    {
        base.Awake();
        
        groundCheck = gameObject.transform.Find("GroundCheck");
        CharacterSelector = new(this);
        JumpController = new(this, jumpSfx, landSfx);
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
        bool isMoving = MoveInput.x != 0f;

        // -- Animator ----------------------
        //animator.SetBool("isMoving", isMoving);
        //animator.SetFloat("moveSpeed", attributes.moveSpeed.FinalValue / 3);

        // -- Audio -------------------------
        if(isMoving && IsGrounded())
        {
            stepTimer -= Time.deltaTime;
            if(stepTimer <= 0)
            {
                PlayFootstep(GetCurrentSurface());
                stepTimer = stepInterval;
            }
        }
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

    private SurfaceType GetCurrentSurface()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1.5f))
        {
            if(hit.collider.TryGetComponent<SurfaceData>(out var surface))
            {
                return surface.surfaceType;
            }
        }

        return SurfaceType.CONCRETE;
    }

    // -- Audio Helper ------------------------------------------
    private void PlayFootstep(SurfaceType type)
    {
        string path = $"event:/sfx/steps/{type.ToString().ToLower()}";
        AudioManager.Instance.PlayOneShot(path, transform.position);
    }
    public void PlaySfx(EventReference reference)
    {
        AudioManager.Instance.PlayOneShot(reference, transform.position);
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

    public void OnTalk(InputAction.CallbackContext context)
    {
        if (context.performed) AudioManager.Instance.PlayOneShot(raccoonSound, transform.position);
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

        Vector3 origin = transform.position + Vector3.up * 0.2f;
        Debug.DrawRay(origin, Vector3.down * 1.5f, Color.red);
    }
}
