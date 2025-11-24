using UnityEngine;
using System.Collections; // Needed for Coroutine (DisablePlatformCollision)
using UnityEngine.Tilemaps; // Needed for TilemapCollider2D access

public class TestPlayerController : MonoBehaviour
{
    // === REFERENCES AND STATE ===
    private Rigidbody2D rb;
    private Collider2D playerCollider; // Needed for platform drop collision control
    private bool isGrounded = false;

    // Public auto-properties, set by Tile_interaction.cs
    
    public bool IsOnPlatform { get; set; } = false;
    public bool is_touching_wall { get; set; } = false;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 8f;
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 2f;

    [Header("Wall Slide")]
    // Define the speed as a field for Inspector adjustment
    public float wallSlideSpeed = -2f; 

    [Header("Platform Drop")]
    public float disableCollisionTime = 0.3f; // Duration player ignores platform collision

    // Persistent player instance (Singleton logic)
    private static TestPlayerController instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>(); // Get the player's collider
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleWallSlide();
        HandlePlatformDrop(); // New: Check for input to drop through platform
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        // Standard Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Better jump physics (snappy fall/short hop)
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void HandleWallSlide()
    {
        // Only slide if touching wall, not grounded, and falling
        if (is_touching_wall && !isGrounded && rb.linearVelocity.y < 0)
        {
            // Apply the controlled slide speed
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, wallSlideSpeed));
        }
    }

    void HandlePlatformDrop()
    {
        // Input check: On a platform AND pressing Down (Vertical < 0) AND pressing Jump
        if (IsOnPlatform && Input.GetAxisRaw("Vertical") < -0.5f && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(DisablePlatformCollision());
        }
    }

    // Coroutine to temporarily disable collision with the Tilemap
    IEnumerator DisablePlatformCollision()
    {
        Final_checker interactionScript = GetComponent<Final_checker>();
        
        if (interactionScript != null && interactionScript.collisionTilemap != null)
        {
            // Find the TilemapCollider2D of the Tilemap we are interacting with
            TilemapCollider2D platformCollider = interactionScript.collisionTilemap.GetComponent<TilemapCollider2D>();

            if (platformCollider != null && playerCollider != null)
            {
                // Ignore collision (player drops through)
                Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

                // Wait for the duration
                yield return new WaitForSeconds(disableCollisionTime);

                // Re-enable collision
                Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
            }
        }
    }

    // --- METHODS FOR TILE_INTERACTION.CS ---

    // Public method for Tile_interaction.cs to set the private grounded state
    public void SetGroundedState(bool state)
    {
        isGrounded = state;
    }

    // Public property for other scripts to READ the grounded state (avoids CS0102 error)
    public bool IsGrounded => isGrounded; 
}