using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int lives;

    public event Action OnDead;
    public event Action OnHurt;
    public event Action OnHeal;
    public void TakeDamage()
    {
        lives--;
        HandleDamageTaken();
    }

    public void StealHealth()
    {
       
        OnHeal?.Invoke();
        lives ++;
        
            
    }

    private void HandleDamageTaken()
    {

        if (lives <= 0)
        {
            OnDead?.Invoke();
            
        }
        else
        {
            OnHurt?.Invoke();
        }
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
