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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next_scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        GameObject startPoint = GameObject.Find("Player_start");
        if (startPoint != null)
        {
            player.transform.position = startPoint.transform.position;
        }
        triggered = false;
    }
}