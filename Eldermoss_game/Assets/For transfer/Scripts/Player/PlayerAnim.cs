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
    }

    private void PlayAttackAnim()
    {
        animator.SetTrigger("attack");
    }
    private void Respawn_anim()
    {
        animator.SetTrigger("respawn");
    }
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHurt -= PlayHurtAnim;
            playerHealth.OnDead -= PlayDeadAnim;
        }
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.OnAttack -= PlayAttackAnim;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.OnAttack -= PlayAttackAnim;
        }
    }
}
