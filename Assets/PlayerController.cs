using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rollForce = 25f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 10f;
    private float coolDownTimer;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("Jump")]
    public float jumpCooldown = 0.3f;
    private float jumpCooldownTimer = 0f;
    private bool canJump => isGrounded && jumpCooldownTimer <= 0f;

    [Header("Roll Collider Shrink")]
    public Vector2 normalColliderSize;
    public Vector2 normalColliderOffset;
    public Vector2 rollColliderSize;
    public Vector2 rollColliderOffset;

    public Animator animator;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isRolling = false;
    private float rollTimer = 0f;

    public bool isGrounded => CheckGrounded();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        // Auto-capture normal collider values at start
        normalColliderSize = col.size;
        normalColliderOffset = col.offset;
    }

    void Update()
    {
        if (coolDownTimer > 0f)
            coolDownTimer -= Time.deltaTime;

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        float velY = rb.linearVelocity.y;
        animator.SetFloat("velocityY", velY);
        animator.SetBool("isGrounded", isGrounded);

        if (!isGrounded)
        {
            if (velY > 0.1f)
                animator.SetBool("isJumping", true);
            else if (velY < -0.1f)
                animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        // Roll timer
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                isRolling = false;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animator.SetBool("isRunning", false);

                // Restore collider to normal size when roll ends
                col.size = normalColliderSize;
                col.offset = normalColliderOffset;
            }
        }
    }

    private bool CheckGrounded()
    {
        float rayLength = groundCheckDistance + col.bounds.extents.y;
       

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            rayLength,
            groundLayer
        );
        return hit.collider != null;
    }

    public void StandStill()
    {
        if (!isRolling)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isRunning", false);
        }
    }

    public void MoveRight()
    {
        if (!isRolling)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("isRunning", true);
        }
    }

    public void MoveLeft()
    {
        if (!isRolling)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(-1f, 1f, 1f);
            animator.SetBool("isRunning", true);
        }
    }

    public void Rollleft()
    {
        if (coolDownTimer <= 0f && !isRolling)
        {
            coolDownTimer = rollCooldown;
            isRolling = true;
            rollTimer = rollDuration;
            rb.linearVelocity = new Vector2(-rollForce, rb.linearVelocity.y);
            transform.localScale = new Vector3(-1f, 1f, 1f);

            // Shrink collider during roll
            col.size = rollColliderSize;
            col.offset = rollColliderOffset;

            animator.ResetTrigger("Roll");
            animator.SetTrigger("Roll");
        }
    }

    public void Rollright()
    {
        if (coolDownTimer <= 0f && !isRolling)
        {
            coolDownTimer = rollCooldown;
            isRolling = true;
            rollTimer = rollDuration;
            rb.linearVelocity = new Vector2(rollForce, rb.linearVelocity.y);
            transform.localScale = new Vector3(1f, 1f, 1f);

            // Shrink collider during roll
            col.size = rollColliderSize;
            col.offset = rollColliderOffset;

            animator.ResetTrigger("Roll");
            animator.SetTrigger("Roll");
        }
    }

    public void Jump()
    {
        
        if (canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
            animator.SetBool("isRunning", false);
            jumpCooldownTimer = jumpCooldown;
        }
    }

    public void Attack1()
    {
        animator.SetTrigger("Attack1");
    }

    public void Attack2()
    {
        animator.SetTrigger("Attack2");
    }
}