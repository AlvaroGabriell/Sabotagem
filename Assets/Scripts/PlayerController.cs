using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]

/// <summary>
/// Script principal do player.
/// Todos os componentes do player são instanciados através desse script, portanto outros scripts
/// que quiserem referenciar tais componentes podem acessá-los por esse script.
/// </summary>
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
    private float stepInterval = 0.5f;
    private float stepTimer;
    
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
                GetCurrentSurface(out SurfaceData surfaceData);
                PlayFootstep(surfaceData.surfaceType);
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

    /// <summary>
    /// Retorna se o player está pisando em alguma superfície e o tipo da superfície que ele está pisando.
    /// </summary>
    /// <param name="surfaceData">O tipo da superfície.</param>
    /// <returns>Retorna <c>true</c> se o player está pisando em uma superfície e <c>false</c> se não estiver pisando em superfície nenhuma.</returns>
    public bool GetCurrentSurface(out SurfaceData surfaceData)
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if(IsGrounded() && Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.35f))
        {
            if(hit.collider.TryGetComponent(out surfaceData)) return true;
        }

        surfaceData = default;
        return false;
    }

    // -- Audio Helper ------------------------------------------
    private void PlayFootstep(SurfaceType type)
    {
        AudioManager.Instance.PlayOneShot($"{type.ToString().ToLower()}Steps", transform.position);
    }

    public void PlaySfxFromPlayer(string key)
    {
        AudioManager.Instance.PlayOneShot(key, transform.position);
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
        if (context.performed) CharacterSelector.Talk();
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
        Debug.DrawRay(origin, Vector3.down * 0.35f, Color.red);
    }
}
