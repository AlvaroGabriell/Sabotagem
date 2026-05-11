using UnityEngine;

public class JumpController
{
    // -- Componentes & Refs  ----------------------
    public readonly PlayerController owner;

    private readonly float rabbitJumpMultiplier;
    private readonly float coyoteTime;
    private readonly float jumpBufferTime;

    // -- Estado interno ---------------------------
    public enum JumpState { Grounded, Rising, Falling }
    public JumpState State {get; private set;} = JumpState.Grounded;

    private float coyoteCounter;
    private float bufferCounter;
    private bool isRabbitJump = false;

    // -- Propriedade pública ----------------------
    public bool IsJumping => State == JumpState.Rising;

    public JumpController(
        PlayerController owner,
        float rabbitJumpMultiplier = 1.4f,
        float coyoteTime           = 0.2f,
        float jumpBufferTime       = 0.2f
    )
    {
        this.owner                = owner;
        this.rabbitJumpMultiplier = rabbitJumpMultiplier;
        this.coyoteTime           = coyoteTime;
        this.jumpBufferTime       = jumpBufferTime;
    }

    // -- API pública ------------------------------

    /// <summary>
    /// Chame em Update, antes de TryJump.
    /// </summary>
    public void Tick(float deltaTime, bool grounded)
    {
        TickCounters(deltaTime);
        UpdateState(grounded);
    }

    ///<summary>
    /// Tenta executar o pulo. Chame em Update, após Tick.
    /// </summary>
    public void TryJump()
    {
        // Quando está no chão, o coyoteCounter é sempre resetado para coyoteTime
        // E quando o botão de pulo é pressionado, o bufferCounter é resetado para jumpBufferTime
        // Então, quando o jogador aperta o botão de pulo estando no chão, ambos os valores
        // estão no máximo fazendo o pulo acontecer imediatamente. E se o jogador apertar o botão
        // um pouco antes de chegar no chão, o buffer garante que o pulo aconteça assim que ele tocar o chão.
        bool canJump  = coyoteCounter > 0f;
        bool buffered = bufferCounter > 0f;

        if (!canJump || !buffered) return;
        ExecuteJump();
    }

    /// <summary>
    /// Chamado pelo Input System quando o botão é pressionado.
    /// </summary>
    public void OnJumpPressed()
    {
        bufferCounter = jumpBufferTime;
    }

    /// <summary>
    /// Chamado pelo SkillHelper quando a habilidade do coelho é ativada.
    /// </summary>
    public void OnRabbitJumpPressed()
    {
        bufferCounter = jumpBufferTime;
        isRabbitJump = true;
    }

    // -- Privados ---------------------------------

    private void UpdateState(bool grounded)
    {
        switch (State)
        {
            case JumpState.Grounded:
                if (!grounded)
                    State = JumpState.Falling; // Se estava no chão e saiu, é porque provavelmente caiu de uma plataforma
                break;

            case JumpState.Rising:
                if (owner.Rb.linearVelocity.y <= 0f)
                    State = JumpState.Falling;
                break;

            case JumpState.Falling:
                if (grounded)
                {
                    State = JumpState.Grounded;
                }
                break;
        }
    }

    private void TickCounters(float deltaTime)
    {
        // Coyote time: conta apenas enquanto estiver caindo (saiu do chão sem pular)
        if(State == JumpState.Grounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter = Mathf.Max(0f, coyoteCounter - deltaTime);
        }

        // Jump buffer: decrementa sempre
        bufferCounter = Mathf.Max(0f, bufferCounter - deltaTime);
    }

    private void ExecuteJump()
    {
        State         = JumpState.Rising;
        coyoteCounter = 0f;
        bufferCounter = 0f;

        float multiplier = isRabbitJump ? rabbitJumpMultiplier : 1f;
        isRabbitJump = false;

        float force = owner.Attributes.Get(AttributeType.jumpForce).FinalValue * multiplier;

        // Zera a velocidade vertical antes do impulso pra garantir altura consistente
        owner.Rb.linearVelocity = new Vector3(owner.Rb.linearVelocity.x, 0f, owner.Rb.linearVelocity.z);
        owner.Rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }
}
