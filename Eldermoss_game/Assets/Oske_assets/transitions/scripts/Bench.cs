using UnityEngine;

public class Bench : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something touched the bench: " + collision.gameObject.name);

        var manager = collision.GetComponentInParent<Death_respawn_manager>();
        if (manager == null)
        {
            Debug.LogError("Found object, but NO SCRIPT found on it!");
        }
        else
        {
            manager.Activate_bench(transform.position);
        }
    }
}