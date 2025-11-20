using UnityEngine;

[CreateAssetMenu(menuName = "transitions/Level_connections")]
public class Level_connections : ScriptableObject
{
    public static Level_connections ActiveConnection { get; set; }
}
