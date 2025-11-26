using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float deceleration = 7f;

    [Header("Jump System")]
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float doubleJumpForce = 18f;
    [SerializeField] private float fallGravityMult = 2.5f;
    [SerializeField] private float jumpCutGravityMult = 5f;
    [SerializeField] private float maxFallSpeed = 25f;

    [Header("Double Jump")]
    [SerializeField] private bool unlockDoubleJump = true;

    [Header("Assists")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Combat & Recoil")]
    [SerializeField] private float selfRecoilForce = 3f; // Força que o player vai para trás
    [SerializeField] private float enemyKnockbackForce = 2f; // Força que o inimigo vai para trás
    [SerializeField] private float recoilDuration = 0.15f; // Tempo que perde o controle

    [Header("Attack properties")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform attackPosition;
    [SerializeField] private LayerMask attackLayer;

    private Rigidbody2D rigidbody;
    private IsGroundedChecker isGroundedChecker;
    private Health health;

    private float moveImput;

    private bool isJumping;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float defaultGravityScale;
    private bool canDoubleJump; //double jump flag
    private float recoilTimer;

    public static PlayerBehaviour instance;
    private Vector3 CurrentRespawnPoint;
    public bool isGrounded;
    public bool is_touching_wall;
    private bool isRespawming = false;
    private bool dead = false;

    [Header("Respawn Point")]
    [SerializeField] private int HazardDamage = 2;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rigidbody = GetComponent<Rigidbody2D>();
        isGroundedChecker = GetComponent<IsGroundedChecker>();
        defaultGravityScale = rigidbody.gravityScale;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        health = GetComponent<Health>();
        health.OnDead += HandlePlayerDeath;
        health.OnHurt += PlayerHurtSound;
        
    }

    private void Start()
    {
        dead = false;   
        CurrentRespawnPoint = transform.position;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InputManager.OnJump += HandleJumpInput;
            GameManager.Instance.InputManager.OnAttack += Attack;
        }
    }

    private void Update()
    {
        // Timers
        if (recoilTimer > 0) recoilTimer -= Time.deltaTime;
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        moveImput = GameManager.Instance.InputManager.Movement;

        // Coyote Time
        if (isGroundedChecker.IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;           
            canDoubleJump = true; // Reseta double jump ao tocar no chão

        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        
        FlipSpriteAccordingTomoveImput();


    }

    private void FixedUpdate()
    {
        if(dead) return; //test, may be removed later

        // Se estiver em recuo de dano/ataque, a física inercial assume
        if (recoilTimer > 0) return;

        // 1. Movimento Horizontal Instantâneo
        float targetSpeed = moveImput * moveSpeed;
        float speedDif = targetSpeed - rigidbody.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDif * accelRate;

        rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);

        if (rigidbody.linearVelocity.y < 0)
        {
            rigidbody.gravityScale = defaultGravityScale * fallGravityMult;
            
            // Limitador de velocidade de queda (Terminal Velocity)
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, Mathf.Max(rigidbody.linearVelocity.y, -maxFallSpeed));
        }

        else if (rigidbody.linearVelocity.y > 0 && !GameManager.Instance.InputManager.IsJumpHeld)
        {
            rigidbody.gravityScale = defaultGravityScale * jumpCutGravityMult;
        }
        // Caso C: Subindo normal ou no chão
        else
        {
            rigidbody.gravityScale = defaultGravityScale;
        }
        
        // Verifica e executa o pulo se houver buffer
        if (jumpBufferCounter > 0)
        {
            TryJump();
        }
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        CurrentRespawnPoint = newRespawnPoint;
    }

    public void RespawnFromHazard()
    {
        if (isRespawming) return;
        isRespawming = true;

        if (health != null)
        {
            health.TakeDamage(HazardDamage);
        }
        if (dead)
        {
            return;
        }

        rigidbody.linearVelocity = Vector2.zero;

        Level_crossfade fader = FindFirstObjectByType<Level_crossfade>();
        if (fader != null)
        {
            fader.RespawnFade(this);
        }
        else
        {
            TeleportToRespawn();
        }
    }
    public void TeleportToRespawn()
    {
        rigidbody.linearVelocity = Vector2.zero;
        transform.position = CurrentRespawnPoint;
        isRespawming = false;
    }
   

    private void HandleJumpInput()
    {
        
        if (this == null || gameObject == null) return;
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerJump);
        jumpBufferCounter = jumpBufferTime;
    }

    private void TryJump()
    {
        // 1. Pulo Normal (No chão ou Coyote Time)
        if (coyoteTimeCounter > 0f)
        {
            PerformJump(jumpForce);
            coyoteTimeCounter = 0f; // Consome o coyote time
            jumpBufferCounter = 0f; // Consome o buffer
        }
        // 2. Pulo Duplo (No ar e permitido)
        else if (unlockDoubleJump && canDoubleJump)
        {
            PerformJump(doubleJumpForce);
            canDoubleJump = false; // Gasta o pulo duplo
            jumpBufferCounter = 0f; // Consome o buffer
            
            // Opcional: Efeito visual ou som de Double Jump aqui
        }
    }

    private void PerformJump(float force)
    {
        // Zera a velocidade Y para o pulo ser consistente
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0f);
        
        // Aplica força instantânea
        rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerJump);
    }
       
    
    private void PlayerHurtSound()
    {
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerHurt);
    }
    

    private void PlayWalkSound()
    {
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerWalk);
    }

private void ApplyRecoil()
    {
        recoilTimer = recoilDuration;
        int direction = transform.localScale.x > 0 ? -1 : 1;
        
        rigidbody.linearVelocity = Vector2.zero; // Para o movimento atual
        rigidbody.AddForce(new Vector2(direction * selfRecoilForce, 0), ForceMode2D.Impulse);
    }

    private void Attack()
    {
        if (this == null || gameObject == null) return;
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerAttack);

        Collider2D[] hittedEnemies = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, attackLayer);
        print("Hitted Enemies: " + hittedEnemies.Length);

        if(hittedEnemies.Length > 0)
        {
            // Aplica Recoil no Player
            recoilTimer = recoilDuration;
            isJumping = false; // Cancela pulo se atacar (opcional, mas bom para pogo)

            int direction = transform.localScale.x > 0 ? -1 : 1;
            // Zera velocidade atual para o impacto ser limpo
            rigidbody.linearVelocity = Vector2.zero; 
            // Aplica velocidade instantânea
            rigidbody.linearVelocity = new Vector2(direction * selfRecoilForce, 0);


            foreach (Collider2D hittedEnemy in hittedEnemies)
            {
                // Tenta pegar o script BaseEnemy para aplicar knockback nele
                if (hittedEnemy.TryGetComponent(out BaseEnemy enemyScript))
                {
                    // Calcula direção do Player para o Inimigo
                    Vector2 knockbackDir = (hittedEnemy.transform.position - transform.position).normalized;
                    enemyScript.ApplyKnockback(knockbackDir, enemyKnockbackForce);
                }
                print("CHECKING ENEMY TO TAKE DAMAGE");
                if (hittedEnemy.TryGetComponent(out Health enemyHealth))
                {
                    print("ENEMY TAKING DAMAGE");
                    enemyHealth.TakeDamage();
                    health.StealHealth();
                }
                
            }
        }
    }
    public void RevivePlayer()
    {
        dead = false;
        isRespawming = false;
        //GetComponent<Collider2D>().enabled = true;
        //rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        print("Player Revived");
        //if (Instance != null) Destroy(this.gameObject);
        //Instance = this;

        //InputManager = new InputManager();
        //if (GameManager.Instance != null)
        //{
        //    GameManager.Instance.InputManager.EnablePlayerInput();
        //}
    }


    private void HandlePlayerDeath()
    {
        dead = true;
        //GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerDeath);
        //rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        //GameManager.Instance.InputManager.DisablePlayerInput();
        Death_respawn_manager Respawner = GetComponent<Death_respawn_manager>();
        
        if (Respawner != null)
        {
            Debug.LogWarning("Calling Respawn from Death_respawn_manager.");
            Respawner.Death();
        }
        else
        {
            Debug.LogWarning("Death_respawn_manager component not found on Player.");
        }
    }

    private void FlipSpriteAccordingTomoveImput()
    {  
        if (moveImput < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveImput > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }

    private void OnDisable()
    {
        // We check if GameManager exists first to avoid errors when quitting the game
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.OnJumpUp -= HandleJumpInput;
            GameManager.Instance.InputManager.OnAttack -= Attack;
        }
    }
    private void OnDestroy()
    {
        // Double-check: If the object is deleted, FORCE unsubscribe
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {

            GameManager.Instance.InputManager.OnJumpUp -= HandleJumpInput;
            GameManager.Instance.InputManager.OnAttack -= Attack;
        }
    }

}
