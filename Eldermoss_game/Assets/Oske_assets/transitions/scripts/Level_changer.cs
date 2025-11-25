
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_changer : MonoBehaviour
{
    [SerializeField]
    private Level_connections _Connections;

    [SerializeField]
    private string _target_scene_name;

    [SerializeField]
    private Transform _spawn_point;

    private void Start()
    {
        if (_Connections == Level_connections.ActiveConnection)
        {
            var player = FindFirstObjectByType<PlayerBehaviour>();

            if (player != null && _spawn_point != null)
            {
                // 1. Move the player
                player.transform.position = _spawn_point.position;

                // 2. UPDATE THE RESPAWN POINT HERE!
                player.SetRespawnPoint(_spawn_point.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerBehaviour>();
        
        // IMPORTANT: Check if it was actually the PLAYER touching the door
        // (Otherwise an enemy or bullet could trigger the level change!)
        if (player != null)
        {
            Level_connections.ActiveConnection = _Connections;
            Level_crossfade crossfader = FindFirstObjectByType<Level_crossfade>();
            if (crossfader != null)
            {
                crossfader.LoadScene(_target_scene_name);
            }
            else
            {
                Debug.LogWarning("WARNING: No Level_crossfade found in the scene! Loading scene without transition.");
                SceneManager.LoadScene(_target_scene_name);
            }
            // SceneManager.LoadScene(_target_scene_name);
        }
    }
}