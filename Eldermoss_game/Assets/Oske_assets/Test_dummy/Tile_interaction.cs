// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.SceneManagement;

// public class Tile_interaction : MonoBehaviour{
//     [Header("Tilemap refrance")]

    
//     public Tilemap collisionTilemap;

//     private TestPlayerController player;

//     private bool is_touching_wall = false;

//     private Rigidbody2D rb;

//     private void Onenable(){
//         SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
//     }
//     private void Ondisable(){
//         SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
//     }

//     void Awake(){
//         player = GetComponent<TestPlayerController>();
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void Start()
// {
//     // If the slot is empty, let's find it automatically!
//     if (collisionTilemap == null)
//     {
//         // 1. Find the GameObject named "Collider_layer"
//         GameObject gridObject = GameObject.Find("Collider_layer");

//         if (gridObject != null)
//         {
//             // 2. Get the Tilemap component from it
//             collisionTilemap = gridObject.GetComponent<Tilemap>();
//         }
//         else
//         {
//             Debug.LogError("CRITICAL: Could not auto-find 'Collider_layer'! Make sure the object name is exact.");
//         }
//     }
// }

//     private void OnCollisionStay2D(Collision2D collision)
//     {
//         // Safety check to ensure we are actually colliding with the Tilemap
//         if (collision.gameObject == collisionTilemap.gameObject && player != null)
//         {
//             // For minimalism, we are skipping the full ground check here, but your 
//             // main property check still runs.
//             check_tile_properties(collision);
//         }
//     }
    
//     // Required: Resets state when collision ends
//     private void OnCollisionExit2D(Collision2D collision)
//     {
//         if (collision.gameObject == collisionTilemap.gameObject)
//         {
//              // NOTE: You must also ensure you call player.SetGroundedState(false); 
//              // in a separate function where you check collision exit.
//             player.is_touching_wall = false;
//         }
//     }

//     void check_tile_properties(Collision2D collision)
// {
//     is_touching_wall = false; 

//     for (int i = 0; i < collision.contactCount; i++)
//     {
        
//         Vector3 contact_point = collision.GetContact(i).point;
//         Vector3Int cellPosition = collisionTilemap.WorldToCell(contact_point);
//         Tile_property tile_prop = collisionTilemap.GetTile(cellPosition) as Tile_property;   

//         if (tile_prop != null)
//         {
            
//             if (tile_prop.Wall && !player.IsGrounded)
//             {
                
//                 float normal_angle = Vector2.Angle(collision.GetContact(i).normal, Vector2.up);
                
                
//                 if (normal_angle > 45f)
//                 {
//                     is_touching_wall = true;
//                 }
//             }

//             if (tile_prop.Platform)
//             {
//                 // Platform interaction logic (e.g., enable drop-through) goes here
//             }
            

//             if (tile_prop.Hazard)
//             {
//                 // Hazard interaction logic goes here
//             }
//         }
//     } 
//     player.is_touching_wall = is_touching_wall;
// }
// }

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement; // Required for scene switching

public class Tile_interaction : MonoBehaviour
{
    [Header("Tilemap Reference")]
    public Tilemap collisionTilemap;

    private TestPlayerController player;
    private bool is_touching_wall = false;
    private Rigidbody2D rb;

    // --- FIX 1: CAPITALIZATION & NAMESPACE ---
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- FIX 2: THE MISSING FUNCTION ---
    // This runs automatically every time a level loads
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindGrid();
    }

    void Awake()
    {
        player = GetComponent<TestPlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        FindGrid();
    }

    // Helper function so we don't write the code twice
    void FindGrid()
    {
        GameObject gridObject = GameObject.Find("Collider_layer");

        if (gridObject != null)
        {
            collisionTilemap = gridObject.GetComponent<Tilemap>();
            Debug.Log("Tile_interaction: Found Collider_layer successfully.");
        }
        else
        {
            // Only log error if we really needed it and couldn't find it
            if (collisionTilemap == null)
                Debug.LogError("CRITICAL: Could not auto-find 'Collider_layer'! Check name.");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Safety check + Make sure tilemap isn't null/destroyed
        if (collisionTilemap != null && collision.gameObject == collisionTilemap.gameObject && player != null)
        {
            check_tile_properties(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // --- FIX 3: SAFETY CHECK FOR DESTROYED OBJECT ---
        if (collisionTilemap == null) return;

        if (collision.gameObject == collisionTilemap.gameObject)
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

            // --- THE NUDGE FIX ---
            // Push the point slightly inside the block to ensure we hit the tile, not the edge
            Vector3 hitPosition = contact.point - (contact.normal * 0.1f);
            
            Vector3Int cellPosition = collisionTilemap.WorldToCell(hitPosition);
            Tile_property tile_prop = collisionTilemap.GetTile(cellPosition) as Tile_property;

            if (tile_prop != null)
            {
                if (tile_prop.Wall && !player.IsGrounded)
                {
                    float normal_angle = Vector2.Angle(contact.normal, Vector2.up);

                    if (normal_angle > 45f)
                    {
                        is_touching_wall = true;
                    }
                }

                if (tile_prop.Platform)
                {
                    // Platform logic
                }

                if (tile_prop.Hazard)
                {
                    // Hazard logic
                }
            }
        }
        player.is_touching_wall = is_touching_wall;
    }
}
