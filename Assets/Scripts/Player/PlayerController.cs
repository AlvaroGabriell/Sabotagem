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
    [SerializeField] private CharacterEntry[] characters;
    [SerializeField] private CharacterWheel characterWheel;

    // -- Ground Check ------------------------------------------
    private Transform groundCheck;
    private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;
    private readonly Collider[] groundHits = new Collider[8];

    // -- Respawn -----------------------------------------------
    public Vector3 LastSafePos {get; private set;}
    [SerializeField] private LayerMask dangerLayer;
    // private readonly Collider[] hazardHits = new Collider[16];

    // -- Movement ----------------------------------------------
    public Vector2 MoveInput {get; private set;}
    public bool IsJumping => JumpController.IsJumping;

    // -- Audio -------------------------------------------------
    private float stepInterval = 0.5f;
    private float stepTimer;

    // -- Debugging ---------------------------------------------
    [SerializeField] private bool showAllDebugInfo = false;
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool drawGizmos = false;
    
    // ----------------------------------------------------------

    override protected void Awake()
    {
        base.Awake();

        Attributes.Get(AttributeType.moveSpeed).SetBaseValue(3f);
        
        groundCheck = gameObject.transform.Find("GroundCheck");
        CharacterSelector = new(this, characters);
        JumpController = new(this);
        playerInput = GetComponent<PlayerInput>();
        //animator = GetComponent<Animator>();

        playerInput.neverAutoSwitchControlSchemes = true;
    }
    
    void Update()
    {
        UpdateValues();

        // Atualiza os valores do JumpController (coyote time, jump buffer, etc) e tenta pular se possível.
        JumpController.Tick(Time.deltaTime, IsOnJumpableSurface());
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
                GetCurrentSurface(out SurfaceInfo surfaceInfo);
                PlayFootstep(surfaceInfo);
                stepTimer = stepInterval;
            }
        }
    }

    //Calcula e executa o movimento do jogador.
    public void HandleMovement()
    {
        if(characterWheel.IsOpen) return;

        float speed = Attributes.Get(AttributeType.moveSpeed).FinalValue;
        Rb.linearVelocity = new Vector3(MoveInput.x * speed, Rb.linearVelocity.y, MoveInput.y * speed);
    }

    /// <summary>
    /// Retorna se o player está pisando em alguma superfície que implemente a interface IJumpable.
    /// Pode retornar false mesmo se o player estiver pisando em uma superfície, caso essa superfície não implemente a interface IJumpable.
    /// </summary>
    /// <returns>Retorna <c>true</c> se o player está pisando em uma superfície IJumpable e <c>false</c> se não estiver pisando em nenhuma superfície IJumpable.</returns>
    public bool IsOnJumpableSurface()
    {
        int count = Physics.OverlapSphereNonAlloc(groundCheck.position, groundCheckRadius, groundHits, ~0, QueryTriggerInteraction.Ignore);

        for(int i = 0; i < count; i++)
        {
            if(groundHits[i].TryGetComponent(out IJumpableSurface _)) return true;
            
        }

        return false;
    }

    /// <summary>
    /// Retorna se o player está pisando em alguma superfície que esteja na groundLayer.
    /// </summary>
    /// <returns>Retorna <c>true</c> se o player está pisando em uma superfície na groundLayer e <c>false</c> se não estiver pisando em nenhuma superfície na groundLayer.</returns>
    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    public bool HasEnoughGroundForRespawn()
    {
        float radius = 0.3f;
        float rayLenght = 0.2f;

        Vector3 center = transform.position;

        Vector3[] points =
        {
            center,
            center + Vector3.forward * radius,
            center + Vector3.back * radius,
            center + Vector3.left * radius,
            center + Vector3.right * radius
        };

        foreach(var point in points)
        {
            if(!Physics.Raycast(point + Vector3.up * 0.1f, Vector3.down, rayLenght, groundLayer)) return false;
        }

        return true;
    }

    public bool IsSafeFromDanger()
    {
        return !Physics.CheckSphere(Utils.GetVisualCenter(gameObject), 3f, dangerLayer, QueryTriggerInteraction.Collide);
    }

    /// <summary>
    /// Retorna se o player está pisando em alguma superfície na groundLayer e o tipo da superfície que ele está pisando.
    /// </summary>
    /// <param name="surfaceInfo">O tipo da superfície.</param>
    /// <returns>Retorna <c>true</c> se o player está pisando em uma superfície na groundLayer e <c>false</c> se não estiver pisando em  nenhuma superfície na groundLayer.</returns>
    public bool GetCurrentSurface(out SurfaceInfo surfaceInfo)
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if(IsGrounded() && Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.35f))
        {
            if(hit.collider.TryGetComponent(out SurfaceData surface))
            {
                surfaceInfo = surface.surfaceInfo;
                return true;
            }
        }

        surfaceInfo = SurfaceInfo.Default;
        return false;
    }

    public bool TryUpdateSafePos()
    {
        if (IsGrounded() && HasEnoughGroundForRespawn() && IsSafeFromDanger() && Vector3.Distance(transform.position, LastSafePos) > 0.5f)
        {
            LastSafePos = transform.position;
            return true;
        }
        return false;
    }

    // -- Audio Helper ------------------------------------------
    private void PlayFootstep(SurfaceInfo info)
    {
        AudioManager.Instance.PlayOneShot(info.footstepSound, transform.position);
    }

    public void PlaySfxFromPlayer(string eventPath)
    {
        AudioManager.Instance.PlayOneShot(eventPath, transform.position);
    }

    // -- Input System Callbacks --------------------------------

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && !characterWheel.IsOpen) JumpController.OnJumpPressed();
    }

    //public void OnCrouch(InputAction.CallbackContext context)
    //{
    //    
    //}

    public void OnSwitchAnimal(InputAction.CallbackContext context)
    {
        if (context.started) characterWheel.Toggle();
    }

    public void OnWheelNavigate(InputAction.CallbackContext context)
    {
        characterWheel.OnDirectionInput(context.ReadValue<Vector2>());
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.started) SkillHelper.HandleSkill(this);
    }

    public void OnTalk(InputAction.CallbackContext context)
    {
        if (context.started) SkillHelper.Talk(CharacterSelector.SelectedCharacter, this);
    }

    // -- Debugging ---------------------------------------------

    void OnDrawGizmos()
    {
        if(!(showAllDebugInfo || drawGizmos)) return;

        if (groundCheck == null)
        {
            Transform t = transform.Find("GroundCheck");
            if (t == null) return;
            groundCheck = t;
        }
        
        // Ground Check original
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Vector3 origin = transform.position + Vector3.up * 0.2f;
        Debug.DrawRay(origin, Vector3.down * 0.35f, Color.red);

        // Danger Safety Check
        Gizmos.color = IsSafeFromDanger() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(Utils.GetVisualCenter(gameObject), 3f);

        // HasEnoughGround debug
        float radius = 0.3f;
        float rayLength = 0.2f;

        Vector3 center = transform.position;

        Vector3[] points =
        {
            center,
            center + Vector3.forward * radius,
            center + Vector3.back * radius,
            center + Vector3.left * radius,
            center + Vector3.right * radius
        };

        foreach (var point in points)
        {
            Vector3 rayOrigin = point + Vector3.up * 0.1f;

            bool hit = Physics.Raycast(rayOrigin, Vector3.down, rayLength, groundLayer);

            Gizmos.color = hit ? Color.green : Color.red;

            Gizmos.DrawSphere(rayOrigin, 0.03f);
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * rayLength);
        }
    }

    void OnGUI()
    {
        if(!(showAllDebugInfo || showDebugInfo)) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label($"IsGrounded: {IsGrounded()}");
        GUILayout.Label($"Is on Jumpable Surface: {IsOnJumpableSurface()}");
        if(GetCurrentSurface(out SurfaceInfo surfaceInfo)) GUILayout.Label($"Current Surface: {surfaceInfo.surfaceType}");
        GUILayout.Label($"Last Safe Pos: {LastSafePos}");
        GUILayout.Label($"Move speed: {Rb.linearVelocity}");
        GUILayout.Label($"Is safe from danger: {IsSafeFromDanger()}");
        GUILayout.EndArea();
    }
}
