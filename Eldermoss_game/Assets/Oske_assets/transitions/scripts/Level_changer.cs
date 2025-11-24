// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class Level_changer : MonoBehaviour
// {
//     [SerializeField]
//     private Level_connections _Connections;

//     [SerializeField]
//     private string _target_scene_name;

//     [SerializeField]
//     private Transform _spawn_point;

//     private void Start()
//     {
//         if (_Connections == Level_connections.ActiveConnection)
//         {
//             FindFirstObjectByType<PlayerBehaviour>().transform.position = _spawn_point.position;
//         }
//     }
//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         var player = collision.GetComponent<PlayerBehaviour>();
//         Level_connections.ActiveConnection = _Connections;
//         SceneManager.LoadScene(_target_scene_name);
//     }
// }

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
        // Only try to move the player if this is the correct connection
        if (_Connections == Level_connections.ActiveConnection)
        {
            // 1. Find the player safely
            var player = FindFirstObjectByType<TestPlayerController>();

            // 2. Check if we actually found the player
            if (player == null)
            {
                Debug.LogError("CRITICAL ERROR: Level_changer cannot find a 'PlayerBehaviour' object in the scene!");
                return; // Stop here so we don't crash
            }

            // 3. Check if the spawn point is assigned
            if (_spawn_point == null)
            {
                Debug.LogError("CRITICAL ERROR: The 'Spawn Point' slot is empty on " + gameObject.name);
                return; // Stop here so we don't crash
            }

            // 4. If both exist, NOW it is safe to move
            player.transform.position = _spawn_point.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<TestPlayerController>();
        
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