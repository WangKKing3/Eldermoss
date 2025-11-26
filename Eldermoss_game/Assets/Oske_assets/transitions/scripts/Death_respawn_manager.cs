using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Death_respawn_manager : MonoBehaviour
{
    [Header("Bench System")]
    private string Last_bench_location;
    private Vector3 Last_bench_cordiantes;

    public bool Respawning_at_bench;

    void Start()
    {
        if(string.IsNullOrEmpty(Last_bench_location))
        {
            Last_bench_location = SceneManager.GetActiveScene().name;
            Last_bench_cordiantes = transform.position;
        }
    }

    public void Activate_bench(Vector3 becnh_coridnates)
    {
        Last_bench_location = SceneManager.GetActiveScene().name;
        Last_bench_cordiantes = becnh_coridnates;
        Debug.Log("Bench Activated at: " + Last_bench_location);
    }

    public void Death()
    {
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        

        Level_crossfade fader = FindFirstObjectByType<Level_crossfade>();
        if (fader != null)
        {
            yield return StartCoroutine(fader.FadeOut());
        }
        Respawning_at_bench = true;
        yield return new WaitForSeconds(1.15f);
        yield return SceneManager.LoadSceneAsync(Last_bench_location);
        transform.position = Last_bench_cordiantes;
        var player = GetComponent<PlayerBehaviour>();
        var health = GetComponent<Health>();
        if(health != null)
        {
            health.HealthFull();
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (fader != null)
        {
            yield return StartCoroutine(fader.FadeIn());
        }
        yield return new WaitForSeconds(0.5f);
        Respawning_at_bench = false;
        player.RevivePlayer();

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
