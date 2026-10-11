using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Match3 Tile", menuName = "Match 3/Tile")]
public class Match3Tile : ScriptableObject
{
    public string id;
    public TileBase visualTile; // Tilemap tile asset
    public int scoreValue = 10;
}
