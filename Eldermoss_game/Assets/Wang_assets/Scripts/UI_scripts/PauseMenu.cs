using UnityEngine;

using UnityEngine.SceneManagement;



public class PauseMenu : MonoBehaviour
{

    public GameObject container;

    private bool isPaused = false;


    void Update()

    {

        // Vi sjekker for ESC-tasten og toggler mellom pause og fortsett

        if (Input.GetKeyDown(KeyCode.Escape))

        {

            if (isPaused)
            {
                ResumeGame();
            }
            else

            {
                PauseGame();
            }
        }
    }


    public void PauseGame()

    {

        if (isPaused) return;



        container.SetActive(true); 
        Time.timeScale = 0f;      
        isPaused = true;


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)
        {
            GameManager.Instance.InputManager.DisablePlayerInput();
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        container.SetActive(false); 
        Time.timeScale = 1f;        
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameManager.Instance != null && GameManager.Instance.InputManager != null)

        {
            GameManager.Instance.InputManager.EnablePlayerInput();
        }
    }

    public void QuitGame()

    {
        Application.Quit();
        Debug.Log("Quit!");
    }

}