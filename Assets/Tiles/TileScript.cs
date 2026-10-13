using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Match 3/Tile")]
public class TileScript : TileBase
{
    public Sprite tileSprite;
    public TileType tileType;
    public Color tileColor = Color.white;
    public UnityEngine.Tilemaps.Tile.ColliderType collider = UnityEngine.Tilemaps.Tile.ColliderType.Sprite;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = tileSprite;
        tileData.color = tileColor;
        tileData.colliderType = collider;
    }
}