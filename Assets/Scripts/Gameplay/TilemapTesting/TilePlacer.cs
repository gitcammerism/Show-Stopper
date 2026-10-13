using UnityEngine;
using UnityEngine.Tilemaps;

// Used in-editor to denote the position where a tile will spawn within a level.

[CreateAssetMenu(menuName = "Match 3/Tile/Tile Placer")]
public class TilePlacer : TileBase
{
    public Sprite previewSprite;
    public Tile tile;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // If in-editor, you see the preview sprite. Otherwise, you see nothing.
        tileData.sprite = !Application.isPlaying ? previewSprite : null;
    }
    
    // Runs on start-up.
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
            return false;
        #endif
        
        // Lets the Match3Board know this is a spot where a tile will spawn.
        Match3Board.SpawnTile(position, tile);
        return base.StartUp(position, tilemap, go);
    }
}
