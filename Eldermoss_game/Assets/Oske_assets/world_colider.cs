using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInteraction : MonoBehaviour
{
    // --- SET IN INSPECTOR ---
    // 1. Drag your 'Collision Tilemap' Game Object here!
    public Tilemap collisionTilemap;

    // 2. Set the position the player should respawn to (e.g., the room entrance)
    public Vector3 respawnPoint = new Vector3(0f, 0f, 0f);

    // --- LOGIC ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided with the Tilemap
        if (collision.gameObject == collisionTilemap.gameObject)
        {
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3Int cellPosition = collisionTilemap.WorldToCell(contactPoint);

            // Try to get the tile and cast it to your custom type
            Tile_property propertyTile = collisionTilemap.GetTile(cellPosition) as Tile_property;

            if (propertyTile != null)
            {
                Debug.Log("Touching tile: " + propertyTile.name);
                // Check the Hazard property
                if (propertyTile.Hazard)
                {
                    HandleHazardTouch();
                }
            }
        }
    }

    // --- HAZARD ACTION ---
    void HandleHazardTouch()
    {
        Debug.Log("HAZARD DETECTED! Resetting Player Position.");

        // Stops all current movement and teleports the character to the respawn point
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Stop movement instantly
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Move the player to the specified respawn point
        transform.position = respawnPoint;
    }
}