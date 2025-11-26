using UnityEngine;
using UnityEngine.SceneManagement; // We need this to restart

public class GoalPost : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject winScreen; // Drag your "WinScreen" panel here

    private bool gameEnded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the Player touched the goal
        if (collision.GetComponent<PlayerBehaviour>() != null && !gameEnded)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("YOU WIN!");

        // 1. Show the UI
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        // 2. Freeze Time (Stop everything)
        Time.timeScale = 0f;
    }
}