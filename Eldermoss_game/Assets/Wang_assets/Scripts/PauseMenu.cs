using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Dette er GameObjectet som holder alle pausemenyelementene (ditt "Pause_menu"-panel)
    public GameObject container;

    private bool isPaused = false;
    // Hvis du vil ha denne sjekken tilgjengelig overalt, kan du flytte den til GameManager.cs

    void Update()
    {
        // Vi sjekker for ESC-tasten og toggler mellom pause og fortsett
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // Hvis pauset, fortsett
            }
            else
            {
                PauseGame(); // Hvis ikke pauset, pause
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        container.SetActive(true); // 1. Viser menyen
        Time.timeScale = 0f;       // 2. Fryser spillet
        isPaused = true;

        // 3. Viser muspekeren
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 4. DEAKTIVERER spiller-input for å stoppe bevegelse/angrep
        // Dette er avgjørende siden din PlayerBehaviour bruker InputManager.
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.DisablePlayerInput();
        }
    }

    // Kalles av ESC-tast og "Resume" knappen
    public void ResumeGame()
    {
        if (!isPaused) return;

        container.SetActive(false); // 1. Skjuler menyen
        Time.timeScale = 1f;        // 2. Starter spillet igjen
        isPaused = false;

        // 3. Skjuler muspekeren
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. AKTIVERER spiller-input
        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.EnablePlayerInput();
        }
    }

    // Kalles av "Hovedmeny" knappen
    public void QuitGame()
    {
        // 1. VELDIG VIKTIG: Sett timeScale tilbake til 1 før scenebytte
        Time.timeScale = 1f;

        // 2. Last Hovedmeny scenen (basert på din MainMenu.cs)
        SceneManager.LoadScene("Menu_1");

        // Merk: Hvis hovedmenyen din kalles "MainMenu", bytt "Menu_1" til "MainMenu"
    }
}