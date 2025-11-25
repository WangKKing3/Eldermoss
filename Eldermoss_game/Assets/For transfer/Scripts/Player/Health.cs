using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxLives = 6; // Max liv
    [SerializeField] private int lives;        // now liv

    public int CurrentLives => lives;  
    public int MaxLives => maxLives;

    public event Action OnDead;
    public event Action OnHurt;

    void Start()
    {
        lives = maxLives;
    }

    public void TakeDamage()
    {
        lives--;
        HandleDamageTaken();
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
    


}
