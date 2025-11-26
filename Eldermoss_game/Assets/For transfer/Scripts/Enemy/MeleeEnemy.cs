using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [SerializeField] private Transform detectPosition;
    [SerializeField] private Vector2 detectBoxSize;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float moveSpeed = 3f; // Velocidade de perseguição (exemplo)

    private float cooldownTimer;

    protected override void Awake()
    {
        base.Awake();
        base.health.OnHurt += () => print("Melee Enemy Hurt");
        base.health.OnDead += () => print("Melee Enemy Dead");
        
    }

    protected override void Update()
    {
        if (isRecoiling) return;
        cooldownTimer += Time.deltaTime;
        //print("is in sight? " + PlayerInSight());
        VerifyCanAttack();
    }

    private void VerifyCanAttack()
    {
        if (cooldownTimer < attackCooldown)
            return;
        if (PlayerInSight())
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("attack");
            AttackPlayer();
        }
    }
  
    private void AttackPlayer()
    {   
        if (canAttack == false) return;
        cooldownTimer = 0f;
        if (CheckPlayerInDetectArea().TryGetComponent(out Health playerHealth))
        {
            //print("Making Player Take Damage");
            playerHealth.TakeDamage();
        }
    }

    private Collider2D CheckPlayerInDetectArea()
    {
        if ( detectPosition == null ) return null;
        return Physics2D.OverlapBox(detectPosition.position, detectBoxSize, 0f, playerLayer);
    }

    

    private bool PlayerInSight()
    {
        Collider2D playerCollider = CheckPlayerInDetectArea();
        return playerCollider != null;
    }
    

    private void OnDrawGizmos()
    {
        if (detectPosition == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(detectPosition.position, detectBoxSize);
    }
}