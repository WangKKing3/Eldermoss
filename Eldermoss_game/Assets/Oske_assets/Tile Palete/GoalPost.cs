using UnityEngine;
using UnityEngine.SceneManagement; 

public class GoalPost : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject winScreen; 

    private bool gameEnded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerBehaviour>() != null && !gameEnded)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("YOU WIN!");
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
        Time.timeScale = 0f;
    }
}