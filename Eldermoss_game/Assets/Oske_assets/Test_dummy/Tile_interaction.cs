using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile_interaction : MonoBehaviour{
    [Header("Tilemap refrance")]

    
    public Tilemap collisionTilemap;

    private TestPlayerController player;

    private bool is_touching_wall = false;

    private Rigidbody2D rb;


    void Awake(){
        player = GetComponent<TestPlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        // Safety check to ensure we are actually colliding with the Tilemap
        if (collision.gameObject == collisionTilemap.gameObject && player != null)
        {
            // For minimalism, we are skipping the full ground check here, but your 
            // main property check still runs.
            check_tile_properties(collision);
        }
    }
    
    // Required: Resets state when collision ends
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == collisionTilemap.gameObject)
        {
             // NOTE: You must also ensure you call player.SetGroundedState(false); 
             // in a separate function where you check collision exit.
            player.is_touching_wall = false;
        }
    }

    void check_tile_properties(Collision2D collision)
{
    is_touching_wall = false; 

    for (int i = 0; i < collision.contactCount; i++)
    {
        
        Vector3 contact_point = collision.GetContact(i).point;
        Vector3Int cellPosition = collisionTilemap.WorldToCell(contact_point);
        Tile_property tile_prop = collisionTilemap.GetTile(cellPosition) as Tile_property;   

        if (tile_prop != null)
        {
            
            if (tile_prop.Wall && !player.IsGrounded)
            {
                
                float normal_angle = Vector2.Angle(collision.GetContact(i).normal, Vector2.up);
                
                
                if (normal_angle > 45f)
                {
                    is_touching_wall = true;
                }
            }

            if (tile_prop.Platform)
            {
                // Platform interaction logic (e.g., enable drop-through) goes here
            }
            

            if (tile_prop.Hazard)
            {
                // Hazard interaction logic goes here
            }
        }
    } 
    player.is_touching_wall = is_touching_wall;
}
}
