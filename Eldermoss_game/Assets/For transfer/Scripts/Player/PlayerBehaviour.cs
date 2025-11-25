using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 10f;
    //[SerializeField] private float jumpForce = 16f;
    //[SerializeField] private float acceleration = 50f;

    [Header("Jump System")]
    [SerializeField] private float jumpSpeed = 12f;         // Velocidade de subida constante
    [SerializeField] private int jumpStepsMax = 12;         // Quantos frames dura o impulso se segurar
    [SerializeField] private float minJumpSpeed = 5f;       // Velocidade mínima se soltar o botão rápido
    [SerializeField] private float maxFallSpeed = 25f;      // Velocidade terminal de queda

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

    private float moveDirection;
    
    


    private bool isJumping;
    private int jumpSteps;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float recoilTimer;
    public static PlayerBehaviour instance;
    private Vector3 CurrentRespawnPoint;
    public bool isGrounded;
    public bool is_touching_wall;
    private bool isRespawming = false;

    [Header("Respawn Point")]
    [SerializeField] private int HazardDamage = 2;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        isGroundedChecker = GetComponent<IsGroundedChecker>();
        rigidbody.gravityScale = 3f;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        health = GetComponent<Health>();
        health.OnDead += HandlePlayerDeath;
        health.OnHurt += PlayerHurtSound;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CurrentRespawnPoint = transform.position;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InputManager.OnJump += HandleJump;
            GameManager.Instance.InputManager.OnJumpUp += HandleJumpUpInput; // Pulo variável
            GameManager.Instance.InputManager.OnAttack += Attack;
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

    private void Update()
    {
        // Timers
        if (recoilTimer > 0) recoilTimer -= Time.deltaTime;
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Coyote Time
        if (isGroundedChecker.IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false; // Reseta estado ao tocar no chão
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Tenta iniciar o pulo (Buffer + Coyote)
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && recoilTimer <= 0)
        {
            StartJump();
        }

        // Input de Movimento
        if (recoilTimer <= 0)
        {
            moveDirection = GameManager.Instance.InputManager.Movement;
            FlipSpriteAccordingToMoveDirection();
        }
        else
        {
            moveDirection = 0;
        } 

    }

        private void FixedUpdate()
    {
        // Se estiver em recuo de dano/ataque, a física inercial assume
        if (recoilTimer > 0) return;

        // 1. Movimento Horizontal Instantâneo
        float targetSpeed = moveDirection * moveSpeed;
        float currentVerticalSpeed = rigidbody.linearVelocity.y;

        // 2. Lógica de Pulo por Passos (Frame a Frame)
        if (isJumping)
        {
            // Enquanto tiver "combustível" (steps) e o botão estiver apertado
            if (jumpSteps > 0 && GameManager.Instance.InputManager.IsJumpHeld)
            {
                // Força a velocidade para cima, ignorando gravidade temporariamente
                currentVerticalSpeed = jumpSpeed;
                jumpSteps--;
            }
            else
            {
                // Acabaram os passos ou soltou o botão -> Deixa a gravidade agir
                isJumping = false;
            }
        }

        // 3. Limite de Velocidade de Queda (Terminal Velocity)
        if (currentVerticalSpeed < -maxFallSpeed)
        {
            currentVerticalSpeed = -maxFallSpeed;
        }

        // Aplica a velocidade final
        rigidbody.linearVelocity = new Vector2(targetSpeed, currentVerticalSpeed);
    }

    private void StartJump()
    {   
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerJump);
        
        if (isGroundedChecker.IsGrounded() == false) return;

        isJumping = true;
        jumpSteps = jumpStepsMax; // Recarrega os passos
        
        // Impulso inicial
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, jumpSpeed);

        // Reseta timers para não pular duas vezes
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    private void HandleJump()
    {
        
        //GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerJump);
        //rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //rigidbody.linearVelocity += Vector2.up * jumpForce;
        jumpBufferCounter = jumpBufferTime;
    }

    private void HandleJumpUpInput()
    {
        // Soltou o botão: Cancela a subida forçada
        isJumping = false;
        
        // Se ainda estiver subindo muito rápido, corta a velocidade (Pulo Curto)
        if (rigidbody.linearVelocity.y > minJumpSpeed)
        {
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, minJumpSpeed);
        }
    }



    private void MovePlayer()
    {
        
        /*Vector2 vectorMoveDirection = new Vector2(moveDirection, rigidbody.linearVelocity.y);
        rigidbody.linearVelocity = vectorMoveDirection * moveSpeed;*/
        float targetSpeed = moveDirection * moveSpeed;
        //float speedDif = targetSpeed - rigidbody.linearVelocity.x;
        //float force = speedDif * acceleration * Time.fixedDeltaTime;
        //rigidbody.AddForce(Vector2.right * force, ForceMode2D.Force);
        rigidbody.linearVelocity = new Vector2(targetSpeed, rigidbody.linearVelocity.y);
        
    }
        
    





    
    private void PlayerHurtSound()
    {
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerHurt);
    }
    

    

    

    private void PlayWalkSound()
    {
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerWalk);
    }

    private void RecoilTimer()
    {
       // Timers
        if (recoilTimer > 0) recoilTimer -= Time.deltaTime;
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Coyote Time
        if (isGroundedChecker.IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false; // Reseta estado ao tocar no chão
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Tenta iniciar o pulo (Buffer + Coyote)
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && recoilTimer <= 0)
        {
            StartJump();
        }

        // Input de Movimento
        if (recoilTimer <= 0)
        {
            moveDirection = GameManager.Instance.InputManager.Movement;
            FlipSpriteAccordingToMoveDirection();
        }
        else
        {
            moveDirection = 0;
        }
    }

    private void Attack()
    {
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
                }
                
            }
        }
    }

    private void HandlePlayerDeath()
    {
        GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerDeath);
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.Instance.InputManager.DisablePlayerInput();
    }

    private void FlipSpriteAccordingToMoveDirection()
    {  
        if (moveDirection < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDirection > 0) transform.localScale = new Vector3(1, 1, 1);
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
        GameManager.Instance.InputManager.OnJump -= HandleJump;       // Note the minus sign (-=)
        GameManager.Instance.InputManager.OnJumpUp -= HandleJumpUpInput;
        GameManager.Instance.InputManager.OnAttack -= Attack;
    }
}

}
