using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level_crossfade : MonoBehaviour
{
    public CanvasGroup Fade_canvas_group;
    public float fade_time = 1f;
    public static Level_crossfade instance;

    private void Awake()
    {
           
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string scene_name) 
    {
        StartCoroutine(TransitionSequence(scene_name));
    }

    IEnumerator TransitionSequence(string scene_name)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(scene_name);
        yield return new WaitForSeconds(0.1f); // Wait one frame for the scene to load
        yield return StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        Fade_canvas_group.blocksRaycasts = true;
        float timer = 0f;
        while (timer <= fade_time)
        {
            timer += Time.deltaTime;
            Fade_canvas_group.alpha = Mathf.Lerp(0f, 1f, timer / fade_time);
            yield return null;
        }
        Fade_canvas_group.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer <= fade_time)
        {
            timer += Time.deltaTime;
            Fade_canvas_group.alpha = Mathf.Lerp(1f, 0f, timer / fade_time);
            yield return null;
        }
        Fade_canvas_group.alpha = 0f;
        Fade_canvas_group.blocksRaycasts = false;
    }

    public void RespawnFade(PlayerBehaviour player)
    {
        StartCoroutine(RespawnSequence(player));
    }

    IEnumerator RespawnSequence(PlayerBehaviour player)
    {
        yield return StartCoroutine(FadeOut());
        player.TeleportToRespawn();
        yield return new WaitForSeconds(0.2f); // Wait one frame for the respawn to complete
        yield return StartCoroutine(FadeIn());
    }
}
