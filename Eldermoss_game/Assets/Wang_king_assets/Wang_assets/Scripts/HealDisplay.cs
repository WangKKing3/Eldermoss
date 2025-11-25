using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HealDisplay : MonoBehaviour
{


    [Header("UI References")]
    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] hearts;


    [Header("Player Reference")]
    public Health playerHealth;


    // Update is called once per frame
    void Update()
    {


        if (playerHealth == null) return;

      
        int currentHealth = playerHealth.CurrentLives;
        int maxHealth = playerHealth.MaxLives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
