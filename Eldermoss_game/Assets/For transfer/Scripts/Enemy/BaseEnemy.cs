using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BaseEnemy : MonoBehaviour
{
    protected Animator animator;
    protected Health health;
    protected Rigidbody2D rb;

    protected bool canAttack = true;
    protected bool isRecoiling = false;

    

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        health.OnHurt += PlayHurtAnim;
        health.OnDead += HandleDeath;
        
        
    }

    protected abstract void Update();

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (health == null || rb == null) return;

        // Para a lógica de movimento da IA
        isRecoiling = true;
        
        // Zera velocidade atual e aplica impacto
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Retoma controle após um curto tempo
        StopAllCoroutines(); // Reinicia timers se já estiver sofrendo knockback
        StartCoroutine(RecoverFromKnockback());
    }

    private IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(0.2f); // Tempo do atordoamento
        rb.linearVelocity = Vector2.zero; // Para de deslizar
        isRecoiling = false;
    }

    private void PlayHurtAnim() => animator.SetTrigger("hurt");

    private void HandleDeath()
    {   
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        animator.SetTrigger("dead");
        StartCoroutine(DestroyEnemy(1));
    }


    private IEnumerator DestroyEnemy(int time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}