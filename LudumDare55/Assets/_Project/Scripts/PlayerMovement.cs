using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour{
    public float moveSpeed = 10f;
    public float jumpForce = 12f;
    public float airAcceleration = 20f;
    public float airDeceleration = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float groundCheckRadius = 0.1f;
    private bool isJumping;
    private bool canJump;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            float targetVelocity = moveInput * moveSpeed;
            float acceleration = isGrounded ? airAcceleration : airAcceleration * 0.5f;
            float deceleration = isGrounded ? airDeceleration : airDeceleration * 0.5f;

            if (moveInput * rb.velocity.x < 0)
            {
                // Decelerate when changing direction
                rb.velocity = new Vector2(rb.velocity.x + moveInput * deceleration * Time.deltaTime, rb.velocity.y);
            }
            else if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(targetVelocity))
            {
                // Accelerate up to target velocity
                rb.velocity = new Vector2(rb.velocity.x + moveInput * acceleration * Time.deltaTime, rb.velocity.y);
            }
        }
        else
        {
            // Decelerate to stop when no input
            float deceleration = isGrounded ? airDeceleration : airDeceleration * 0.5f;
            rb.velocity = new Vector2(rb.velocity.x - Mathf.Sign(rb.velocity.x) * deceleration * Time.deltaTime, rb.velocity.y);
        }

        if (isGrounded)
        {
            canJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
    }
}