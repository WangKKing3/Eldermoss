using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New PropertyTile", menuName = "Tiles/Property Tile")]
public class Tile_property : Tile
{
    [Header("Tile Properties")]
    public bool Ground;
    public bool Platform;
    public bool Wall;
    public bool Hazard;
    public bool Camera_Boundary;
}
