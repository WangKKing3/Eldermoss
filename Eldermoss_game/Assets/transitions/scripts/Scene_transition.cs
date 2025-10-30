using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Scene_transition : MonoBehaviour
{
    [Header("Target Scene Name")]
    public string next_scene;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return; // Only trigger once
        if (collision.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(LoadSceneAndPlacePlayer(collision.gameObject));
        }
    }

    private IEnumerator LoadSceneAndPlacePlayer(GameObject player)
    {
        // Optional: add fade out effect here

        // Load the next scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next_scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Find the PlayerStart in the new scene
        GameObject startPoint = GameObject.Find("Player_start");
        if (startPoint != null)
        {
            player.transform.position = startPoint.transform.position;
        }

        // Optional: add fade in effect here

        triggered = false;
    }
}