using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

// The class name MUST match the filename "Final_checker"
public class Final_checker : MonoBehaviour
{
    [Header("Tilemap Reference")]
    public Tilemap collisionTilemap;

    // Change this to 'PlayerBehaviour' if your dummy uses the new script!
    private PlayerBehaviour player; 
    
    private bool is_touching_wall = false;
    private Rigidbody2D rb;

    // --- AUTO-FIND LOGIC ---
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindGrid();
    }

    void Awake()
    {
        // Make sure this matches the script on your dummy!
        player = GetComponent<PlayerBehaviour>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        FindGrid();
    }

    void FindGrid()
    {
        // It looks for an object named EXACTLY "Collider_layer"
        GameObject gridObject = GameObject.Find("Colider_layer");

        if (gridObject != null)
        {
            collisionTilemap = gridObject.GetComponent<Tilemap>();
        }
        else
        {
            // Only log if we really needed it
            if (collisionTilemap == null)
                Debug.LogWarning("Final_checker: Could not auto-find 'Collider_layer'. Is the name correct?");
        }
    }

    // --- COLLISION LOGIC ---
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Safety checks
        if (collisionTilemap != null && collision.gameObject == collisionTilemap.gameObject && player != null)
        {
            check_tile_properties(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collisionTilemap == null) return;

        if (collision.gameObject == collisionTilemap.gameObject && player != null)
        {
            player.is_touching_wall = false;
        }
    }

    void check_tile_properties(Collision2D collision)
    {
        is_touching_wall = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            
            // The Nudge Fix
            Vector3 hitPosition = contact.point - (contact.normal * 0.1f);
            Vector3Int cellPosition = collisionTilemap.WorldToCell(hitPosition);
            
            Tile_property tile_prop = collisionTilemap.GetTile(cellPosition) as Tile_property;

            if (tile_prop != null)
            {
                if (tile_prop.Wall && !player.isGrounded)
                {
                    float normal_angle = Vector2.Angle(contact.normal, Vector2.up);
                    if (normal_angle > 45f) is_touching_wall = true;
                }
                
                if (tile_prop.Hazard)
                {
                    player.RespawnFromHazard();
                    // Hazard logic here
                    Debug.Log("Hazard Hit!");
                    return;
                }
            }
        }
        player.is_touching_wall = is_touching_wall;
    }
}