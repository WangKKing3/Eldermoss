using UnityEngine;
using UnityEngine.UI;

public class HealDisplay : MonoBehaviour
{
    [Header("UI References")]
    // Vi endrer disse til arrays ([]) slik at vi kan legge inn mange forskjellige bilder
    public Sprite[] emptyRunes;
    public Sprite[] fullRunes;
    public Image[] hearts;      // Dette er selve Image-objektene i scenen

    [Header("Player Reference")]
    public Health playerHealth;


    void Start()
    {
        // If we forgot to drag the player in, OR if the link broke during scene load:
        if (playerHealth == null)
        {
            // Search the entire scene for the Health script
            playerHealth = FindFirstObjectByType<Health>();
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        int currentHealth = playerHealth.CurrentLives;
        int maxHealth = playerHealth.MaxLives;

        for (int i = 0; i < hearts.Length; i++)
        {
       
            if (hearts[i] == null) continue;
            // -----------------------------------

            // Sjekk om vi har nok bilder i listene våre for å unngå feil
            if (i < fullRunes.Length && i < emptyRunes.Length)
            {
                if (i < currentHealth)
                {
                    hearts[i].sprite = fullRunes[i];
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].sprite = emptyRunes[i];
                    // hearts[i].color = new Color(1, 1, 1, 0.5f); 
                }
            }

            // Håndterer antall beholdere totalt (Max Health)
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