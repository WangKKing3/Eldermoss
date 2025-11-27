using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int lives;
    [SerializeField] private int maxLives = 5;

    public int CurrentLives => lives;
    public int MaxLives => maxLives;


    public event Action OnDead;
    public event Action OnHurt;
    public event Action OnHeal;
    public void TakeDamage(int damage = 1)
    {
        lives -= damage;
        HandleDamageTaken();
    }

    public void StealHealth()
    {
        OnHeal?.Invoke();
        lives ++;
        if(lives > maxLives)
        {
            lives = maxLives;
            
    }
    }
    private void HandleDamageTaken()
    {

        if (lives <= 0)
        {
            OnDead?.Invoke();
            Death_respawn_manager Respawner = GetComponent<Death_respawn_manager>();
            if (Respawner != null)
            {
                Respawner.Respawn();
            }
            else
            {
                Debug.LogWarning("Death_respawn_manager component not found on Player.");
            }
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

   
    public void HealthFull()
    {
        lives = maxLives;
    }

}