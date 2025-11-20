using UnityEngine;
using UnityEngine.Tilemaps;

// This script MUST be attached to your Collision Tilemap Game Object
public class TilePropertyChecker : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        // Get the Tilemap component on this Game Object
        tilemap = GetComponent<Tilemap>();

        // Define a position where you KNOW you painted a HAZARD tile
        // Change these coordinates to a grid cell position where a hazard tile is located in your scene.
        Vector3Int hazardCellPosition = new Vector3Int(2, 3, 0);

        CheckTileProperties(hazardCellPosition);
    }

    void CheckTileProperties(Vector3Int cellPosition)
    {
        // 1. Get the tile asset at the specified position
        Tile_property propertyTile = tilemap.GetTile(cellPosition) as Tile_property;

        if (propertyTile != null)
        {
            // 2. SUCCESS! The tile is a custom property tile.
            Debug.Log($"Tile at {cellPosition} is a custom property tile.");

            // 3. Now check the property you set in the Inspector on the asset
            if (propertyTile.Hazard)
            {
                Debug.LogError("✅ SUCCESS! The tile is registered as a HAZARD! Your replacement worked.");
            }
            else
            {
                Debug.LogWarning("❌ Tile is the correct custom type, but the Hazard box is NOT checked on the asset.");
            }
        }
        else
        {
            // ❌ FAILURE. The tile is still a simple tile or null.
            Debug.LogError("❌ FAILURE: Tile at this position is NOT your custom Tile_property type. You need to repaint the tile.");
        }
    }
}