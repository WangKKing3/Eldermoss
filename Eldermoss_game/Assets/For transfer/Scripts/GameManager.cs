using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // VIKTIG: Denne er NØDVENDIG for PlayerBehaviour (som nå kaller den)
    public AudioManager AudioManager;

    public InputManager InputManager { get; private set; }


    private void Awake()
    {
        // 1. Singleton Sjekk for å ødelegge duplikater
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // 2. Persistens: Hinder at manageren ødelegges ved sceneskifte
        DontDestroyOnLoad(this.gameObject);

        InputManager = new InputManager();
    }
}