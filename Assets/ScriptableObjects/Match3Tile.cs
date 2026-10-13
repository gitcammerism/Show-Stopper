using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Match3 Tile", menuName = "Match 3/Scriptable Tile")]
public class Match3Tile : ScriptableObject
{
    public TileType tile; // Tile type/name
    public TileBase visualTile; // Tilemap tile asset
    public int scoreValue = 10;
}
