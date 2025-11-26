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

    void Update()
    {
        if (playerHealth == null) return;

        int currentHealth = playerHealth.CurrentLives;
        int maxHealth = playerHealth.MaxLives;

        for (int i = 0; i < hearts.Length; i++)
        {
            // Sjekk om vi har nok bilder i listene våre for å unngå feil
            if (i < fullRunes.Length && i < emptyRunes.Length)
            {
                if (i < currentHealth)
                {
                    // Spilleren har liv her: Vis den spesifikke FULL-runen for denne plassen
                    hearts[i].sprite = fullRunes[i];
                    hearts[i].enabled = true; // Sørg for at den synes
                }
                else
                {
                    // Spilleren har mistet dette livet: Vis den spesifikke TOM-runen
                    hearts[i].sprite = emptyRunes[i];

                    // Hvis du vil at den tomme runen skal være litt gjennomsiktig, kan du fjerne kommentaren under:
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