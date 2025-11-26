using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    // --- 1. Definimos os Estados Possíveis ---
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Detection")]
    [SerializeField] private Transform detectPosition;
    [SerializeField] private Transform chasePosition;
    [SerializeField] private Vector2 detectBoxSize;
    [SerializeField] private Vector2 chaseBoxSize;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Combat Stats")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float moveSpeed = 2f;

    // --- Variáveis de Controle ---
    private float cooldownTimer;
    private EnemyState currentState = EnemyState.Idle; // Estado inicial

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        if (isRecoiling) return;

        cooldownTimer += Time.deltaTime;

        // O Update agora apenas gere qual estado deve estar ativo
        ManageStateTransitions();

        // Executa o comportamento do estado atual
        ExecuteCurrentState();
    }

    // --- 2. Cérebro de Decisões (Mudança de Estado) ---
    private void ManageStateTransitions()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // Se viu o jogador, começa a perseguir
                if (PlayerInChaseRange()) currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                // Se o jogador fugiu para muito longe, volta a ficar parado
                if (!PlayerInChaseRange()) 
                {
                    currentState = EnemyState.Idle;
                }
                // Se o jogador está perto o suficiente, ataca
                else if (PlayerInAttackRange()) 
                {
                    currentState = EnemyState.Attack;
                }
                break;

            case EnemyState.Attack:
                // Se o jogador se afastou durante o ataque, volta a perseguir (após o ataque)
                if (!PlayerInAttackRange() && !IsAttackAnimationPlaying())
                {
                    currentState = EnemyState.Chase;
                }
                break;
        }
    }

    // --- 3. Execução Física (O que fazer em cada estado) ---
    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero; // Garante que está parado
                break;

            case EnemyState.Chase:
                ChasePlayerBehavior();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
    }

    // --- Comportamentos Específicos ---

    private void ChasePlayerBehavior()
    {
        Collider2D playerCollider = CheckPlayerInChaseArea();
        if (playerCollider != null)
        {
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            
            // Move apenas no eixo X
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            animator.SetBool("chase", true);

            // Vira o sprite
            if (direction.x < 0) transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        }
        else animator.SetBool("chase", false);
    }

    private void AttackBehavior()
    {
        // Para o inimigo para bater
        rb.linearVelocity = Vector2.zero;

        if (cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            animator.SetTrigger("attack");
            
            // Lógica de dano (pode ser melhorada com Animation Events depois)
            PerformDamageCheck(); 
        }
    }

    // --- Funções Auxiliares ---
    
    private void PerformDamageCheck()
    {
        // Verifica se o player ainda está ali antes de dar dano
        Collider2D player = CheckPlayerInDetectArea();
        if (player != null && player.TryGetComponent(out Health playerHealth))
        {
            playerHealth.TakeDamage();
        }
    }

    // Verifica se a animação de ataque ainda está a rodar (opcional, para polimento)
    private bool IsAttackAnimationPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("attack_gob"); // Use o nome exato da animação no Animator
    }

    private bool PlayerInAttackRange() => CheckPlayerInDetectArea() != null;
    private bool PlayerInChaseRange() => CheckPlayerInChaseArea() != null;

    private Collider2D CheckPlayerInDetectArea()
    {
        if (detectPosition == null) return null;
        return Physics2D.OverlapBox(detectPosition.position, detectBoxSize, 0f, playerLayer);
    }
    
    private Collider2D CheckPlayerInChaseArea()
    {
        if (chasePosition == null) return null;
        
        return Physics2D.OverlapBox(chasePosition.position, chaseBoxSize, 0f, playerLayer);
    }

    private void OnDrawGizmos()
    {
        if (detectPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(detectPosition.position, detectBoxSize);
        }
        if (chasePosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(chasePosition.position, chaseBoxSize);
        }
    }
}