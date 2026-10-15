// using System.Collections.Generic;
// using System.Linq;
// using Mono.Cecil;
// using UnityEditor.Rendering;
// using UnityEngine;
// using UnityEngine.Tilemaps;
//
// public class BoardBuilder : MonoBehaviour
// {
//     [SerializeField] private Tilemap boardMap;
//     [SerializeField] private Tilemap obstacleMap;
//     [SerializeField] private ObstacleDefinition[] obstacleDefs;
//     [SerializeField] private GameObject[] tilePrefabs;
//
//     private Dictionary<TileBase, ObstacleDefinition> obstacleLookup;
//     public Cell[,] Cells { get; private set; }
//     private BoundsInt bounds;
//
//     private void Awake()
//     {
//         obstacleLookup = obstacleDefs.ToDictionary(d => d.paintedTile);
//         boardMap.CompressBounds();
//         bounds = boardMap.cellBounds;
//         Build();
//     }
//
//     private void Build()
//     {
//         Cells = new Cell[bounds.size.x, bounds.size.y];
//
//         foreach (var pos in bounds.allPositionsWithin)
//         {
//             if (!boardMap.HasTile(pos)) continue;  // hole in board
//
//             int x = pos.x - bounds.xMin;
//             int y = pos.y - bounds.yMin;
//             Vector3 world = boardMap.GetCellCenterWorld(pos);
//
//             var cell = new Cell { x = x, y = y, active = true, worldPos = world };
//
//             var obsTile = obstacleMap.GetTile(pos);
//             if (obsTile != null && obstacleLookup.TryGetValue(obsTile, out var def))
//             {
//                 cell.obstacle = Instantiate(def.prefab, world, Quaternion.identity, transform).GetComponent<Obstacle>();
//                 cell.obstacle.Init(def);
//             }
//             else
//             {
//                 SpawnTile(cell);
//             }
//
//             Cells[x, y] = cell;
//         }
//     }
//     
//     int? TypeAt(int x, int y) =>
//         InBounds(x, y) && Cells[x, y] != null && Cells[x, y].tile != null
//             ? Cells[x, y].tile.type : (int?)null;
//
//     private void SpawnTile(Cell cell)
//     {
//         var allowed = Enumerable.Range(0, tilePrefabs.Length).ToList();
//         
//         if (TypeAt(cell.x - 1, cell.y) is int a && a == TypeAt(cell.x - 2, cell.y)) allowed.Remove(a);
//         if (TypeAt(cell.x, cell.y - 1) is int b && b == TypeAt(cell.x, cell.y - 2)) allowed.Remove(b);
//         
//         int type = allowed[Random.Range(0, allowed.Count)];
//         
//         cell.tile = Instantiate(tilePrefabs[type], cell.worldPos, Quaternion.identity, transform).GetComponent<Tile>();
//     }
//
//     private void PlaceTile(Tile tile, Cell target)
//     {
//         if (tile.cell != null) tile.cell.tile = null;
//         target.tile = tile;
//         tile.cell = target;
//         StartCoroutine(tile.MoveTo(target.worldPos));
//     }
// }
