using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Match 3/Obstacle Definition")]
public class ObstacleDefinition : ScriptableObject
{
    public TileBase paintedTile;            // tilemap tile
    public GameObject prefab;               // obstacle prefab

    public int hitPoints;                   // only applicable if obstacle can be broken
    public bool blocksSwap = false;         // if it can be swapped with another tile
    public bool fallsWithGravity = false;   // if obstacle falls down or stays in place
    public bool tilesFallPast = false;      // if tiles drop past/through this tile
    public bool indestructible = false;     // whether it can be broken at all
}
