using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : LivingEntity
{
    //private Animator animator;

    private Vector2 movement;
    private bool isMoving = false;

    //override protected void Awake()
    //{
    //    base.Awake();
    //    rb = GetComponent<Rigidbody>();
    //    //animator = GetComponent<Animator>();
    //}

    void FixedUpdate()
    {
        HandleMovement();
        UpdateValues();
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.red);
    }

    void UpdateValues()
    {
        // ---------- Animator ----------
        isMoving = Mathf.Abs(movement.magnitude) > 0f;
        //animator.SetBool("isMoving", isMoving);
        //animator.SetFloat("moveSpeed", attributes.moveSpeed.FinalValue / 3);
    }

    //Calcula e executa o movimento do jogador.
    public void HandleMovement()
    {
        Rb.linearVelocity = new Vector3(movement.x * Attributes.Get(AttributeType.moveSpeed).FinalValue, Rb.linearVelocity.y, movement.y * Attributes.Get(AttributeType.moveSpeed).FinalValue) ;
    }

    //Captura o input de movimento
    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            Debug.Log("Jump!");
            Rb.AddForce(Vector3.up * Attributes.Get(AttributeType.jumpForce).FinalValue, ForceMode.Impulse);
        }
    }

    public bool IsGrounded()
    {
        float offset = 0.1f;
        float distance = 0.2f;

        Vector3 origin = transform.position + Vector3.up * offset;

        return Physics.Raycast(origin, Vector3.down, distance);
    }
}
