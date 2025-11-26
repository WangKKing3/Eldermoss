using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public AudioManager AudioManager;
    public InputManager InputManager { get; private set; }



    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;

        InputManager = new InputManager();

        // STARTER BAKGRUNNSMUSIKKEN MED DET NYE NAVNET
        if (MusicManager.Instance != null)
        {
            // ENDRE "ForestTheme" HVIS DU VALGTE ET ANNET NAVN I BIBLIOTEKET
            MusicManager.Instance.PlayMusic("ForestTheme");
        }
    }
    
}
