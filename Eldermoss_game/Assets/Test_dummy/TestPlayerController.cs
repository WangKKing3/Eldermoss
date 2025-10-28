using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f; // Horizontal movement speed

    [Header("Jump")]
    public float jumpForce = 8f;          // Max jump velocity (controls height)
    public float fallMultiplier = 4f;     // Makes falling faster
    public float lowJumpMultiplier = 2f;  // Makes short hops faster if player releases jump early

    private Rigidbody2D rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Make sure rotation is locked
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Better jump for snappy feel without changing max height
        if (rb.linearVelocity.y < 0)
        {
            // Falling faster
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Short hop: rising stops faster if player releases jump
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Ground detection using collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
