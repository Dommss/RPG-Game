using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashTime;

    private float xInput;

    private int facingDir = 1;
    private bool facingRight = true;


    [Header("Collision Information")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update() {

        Movement();
        CheckInput();
        CollisionChecks();

        dashTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            dashTime = dashDuration;
        }

        FlipController();
        AnimatorControllers();
    }

    private void CollisionChecks() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput() {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }

    private void Movement() {

        if (dashTime > 0) {
            // Dashing
            rb.velocity = new Vector2(xInput * dashSpeed, 0);
        } else {
            // Regular movement
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump() {
        if (isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }


    private void AnimatorControllers() {
        bool isMoving = rb.velocity.x != 0;

        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
    }

    private void Flip() {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipController() {
        if (rb.velocity.x > 0 && !facingRight)
            Flip();
        else if (rb.velocity.x < 00 && facingRight)
            Flip();
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
