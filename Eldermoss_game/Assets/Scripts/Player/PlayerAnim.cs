using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator;
    private IsGroundedChecker groundedChecker;
    private Health playerHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        groundedChecker = GetComponent<IsGroundedChecker>();
        playerHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        playerHealth.OnHurt += PlayHurtAnim;
        playerHealth.OnDead += PlayDeadAnim;

        GameManager.Instance.InputManager.OnAttack += PlayAttackAnim;
        GameManager.Instance.InputManager.OnAttack2 += PlayAttackAnim2;
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
    private void PlayAttackAnim2()
    {   
        animator.SetTrigger("attack2");


    }
}
