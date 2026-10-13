using UnityEngine;
using UnityEngine.Tilemaps;

// Used in-editor to denote the position where an obstacle will spawn within a level.

[CreateAssetMenu(menuName = "Match 3/Tiles/Obstacle Placer")]
public class ObstaclePlacer : TileBase
{
    public Sprite previewSprite;
    public Obstacle obstacle;
    
    // Shows the preview sprite in-editor.
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = !Application.isPlaying ? previewSprite : null;
    }

    // Spawns an obstacle in this grid position.
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
            return false;
        #endif

        var newObstacle = Instantiate(obstacle, position, Quaternion.identity);
        return true;
    }
}
