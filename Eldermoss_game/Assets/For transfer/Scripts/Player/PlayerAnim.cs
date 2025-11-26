using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator;
    private IsGroundedChecker groundedChecker;
    private Health playerHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        groundedChecker = GetComponent<IsGroundedChecker>();
        playerHealth = GetComponent<Health>();

        playerHealth.OnHurt += PlayHurtAnim;
        playerHealth.OnDead += PlayDeadAnim;

        GameManager.Instance.InputManager.OnAttack += PlayAttackAnim;
    }

    private void Update()
    {
        bool isMoving = GameManager.Instance.InputManager.Movement != 0;
        animator.SetBool("isMoving", isMoving);
    }

    private void PlayHurtAnim()
    {
        animator.SetTrigger("hurt");
    }

    private void PlayDeadAnim()
    {
        animator.SetTrigger("dead");
        respawn_anim();
    }

    private void PlayAttackAnim()
    {
        animator.SetTrigger("attack");
    }
    private void respawn_anim()
    {
        animator.SetTrigger("respawn");
    }
    private void OnDisable()
    {
        // 1. Unsubscribe from Health (Local component)
        if (playerHealth != null)
        {
            playerHealth.OnHurt -= PlayHurtAnim;
            playerHealth.OnDead -= PlayDeadAnim;
        }

        // 2. Unsubscribe from Input Manager (Global component)
        // This is the critical one causing your crash!
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.OnAttack -= PlayAttackAnim;
        }
    }

    private void OnDestroy()
    {
        // Double safety check
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.OnAttack -= PlayAttackAnim;
        }
    }
}
