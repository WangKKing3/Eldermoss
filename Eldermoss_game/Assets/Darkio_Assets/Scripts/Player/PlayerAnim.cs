using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator;
    private IsGroundedChecker groundedChecker;
    private Health playerHealth;
    private Rigidbody2D rb;

    [Header("Combo System")]
    [SerializeField] private float maxComboDelay = 0.2f; // Tempo máximo entre cliques para manter o combo
    private int comboStep = 0; // Passo atual do combo (0 = nenhum, 1 = ataque 1, 2 = ataque 2)
    private float lastAttackTime = 0f; // Momento do último ataque

    private void Awake()
    {
        animator = GetComponent<Animator>();
        groundedChecker = GetComponent<IsGroundedChecker>();
        playerHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        playerHealth.OnHurt += PlayHurtAnim;
        playerHealth.OnDead += PlayDeadAnim;

        //GameManager.Instance.InputManager.OnAttack += PlayAttackAnim;
        //GameManager.Instance.InputManager.OnAttack2 += PlayAttackAnim2;
        GameManager.Instance.InputManager.OnAttack += HandleAttackInput;
    }

    private void Update()
    {
        bool isMoving = GameManager.Instance.InputManager.Movement != 0;
        animator.SetBool("isMoving", isMoving);

        if (Time.time - lastAttackTime > maxComboDelay)
        {
            comboStep = 0;
        }     
    }

        private void HandleAttackInput()
    {
        // Opcional: Impedir ataque se estiver morto ou tomando dano
        // if (playerHealth.IsDead) return;

        // Atualiza o tempo do último clique
        lastAttackTime = Time.time;

        // Avança o passo do combo
        comboStep++;

        // Limita o combo a 2 passos (pode aumentar se tiver Attack3)
        if (comboStep > 2) 
        {
            comboStep = 1; // Reinicia o loop ou define como 1 para começar novo combo imediatamente
        }

        // Executa a animação baseada no passo atual
        if (comboStep == 1)
        {
            animator.SetTrigger("attack"); // Nome do Trigger no Animator
            GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerAttack); // Som do ataque 1
        }
        else if (comboStep == 2)
        {
            animator.SetTrigger("attack2"); // Nome do Trigger no Animator
             // Opcional: Tocar um som diferente para o segundo ataque se tiver
             GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerAttack); 
        }
    }

    private void PlayHurtAnim()
    {
        animator.SetTrigger("hurt");
        comboStep = 0;
    }

    private void PlayDeadAnim()
    {
        animator.SetTrigger("dead");
        comboStep = 0;
    }

    /*private void PlayAttackAnim()
    {
        animator.SetTrigger("attack");
    }*/


    private void OnDestroy()
    {
        // Boa prática: Remover listeners ao destruir o objeto para evitar erros de memória
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.OnAttack -= HandleAttackInput;
        }
    }
}
