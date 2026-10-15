using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GoalTracker goalTracker;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private BoardInput boardInput;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap boardMap;
    [SerializeField] private Tilemap obstacleMap;

    [Header("Prefabs")]
    [SerializeField] private ObstacleDefinition[] obstacleDefs;
    [SerializeField] private GameObject[] tilePrefabs;

    private Dictionary<TileBase, ObstacleDefinition> obstacleLookup;
    private Dictionary<Vector3Int, Cell> cellsByTilePos = new();
    public Cell[,] Cells { get; private set; }
    private BoundsInt bounds;

    // directions (L, R, U, D respectively)
    private static readonly Vector2Int[] Dirs =
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1),
    };

    private void Awake()
    {
        obstacleLookup = obstacleDefs.ToDictionary(d => d.paintedTile);
        boardMap.CompressBounds();
        bounds = boardMap.cellBounds;
        Build();
    }

    // builds the game board at the start of the game
    private void Build()
    {
        Cells = new Cell[bounds.size.x, bounds.size.y];

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!boardMap.HasTile(pos)) continue;  // hole in board

            int x = pos.x - bounds.xMin;
            int y = pos.y - bounds.yMin;
            Vector3 world = boardMap.GetCellCenterWorld(pos);

            var cell = new Cell { x = x, y = y, active = true, worldPos = world };
            Cells[x, y] = cell;
            cellsByTilePos[new Vector3Int(pos.x, pos.y, 0)] = cell;

            var obsTile = obstacleMap.GetTile(pos);
            if (obsTile != null && obstacleLookup.TryGetValue(obsTile, out var def))
            {
                cell.obstacle = Instantiate(def.prefab, world, Quaternion.identity, transform).GetComponent<Obstacle>();
                cell.obstacle.Init(def);
            }
            else
            {
                SpawnTile(cell);
            }
        }
    }

    // checks the type of the given cell
    int? TypeAt(int x, int y) =>
        InBounds(x, y) && Cells[x, y] != null && Cells[x, y].tile != null
            ? Cells[x, y].tile.type : (int?)null;


    // 2nd method for checking the type of given cell/tile
    private int TileType(int x, int y)
    {
        if (!IsPlayable(x, y)) return -1;
        var g = Cells[x, y].tile;
        return g != null ? g.type : -1;
    }

    // length of a "run" (match) along one axis
    private int RunLength(Cell c, int dx, int dy)
    {
        int t = TileType(c.x, c.y);
        if (t < 0) return 0;

        int max = Mathf.Max(Cells.GetLength(0), Cells.GetLength(1));
        int count = 1;
        for (int i = 1; i < max && TileType(c.x + dx * i, c.y + dy * i) == t; i++) count++;
        for (int i = 1; i < max && TileType(c.x - dx * i, c.y - dy * i) == t; i++) count++;
        return count;
    }

    // checks for a match at the given cell
    private bool HasMatchAt(Cell c)
    {
        return RunLength(c, 1, 0) >= 3 || RunLength(c, 0, 1) >= 3;
    }

    // checks if tile is within the bounds of the game board
    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < Cells.GetLength(0) &&
               y >= 0 && y < Cells.GetLength(1);
    }

    // checks if the tile is playable/active
    private bool IsPlayable(int x, int y)
    {
        return InBounds(x, y) && Cells[x, y] != null && Cells[x, y].active;
    }

    // gets the given cell by its coordinates in the array
    public Cell GetCell(int x, int y)
    {
        return InBounds(x, y) ? Cells[x, y] : null;
    }

    // gets the given cell by its world position
    public Cell GetCellAtWorld(Vector3 worldPos)
    {
        Vector3Int pos = boardMap.WorldToCell(worldPos);
        pos.z = 0;

        if (cellsByTilePos.TryGetValue(pos, out var cell)) return cell;
        return null;
    }

    // spawns a tile at the given cell
    private void SpawnTile(Cell cell)
    {
        var allowed = Enumerable.Range(0, tilePrefabs.Length).ToList();

        if (TypeAt(cell.x - 1, cell.y) is int a && a == TypeAt(cell.x - 2, cell.y)) allowed.Remove(a);
        if (TypeAt(cell.x, cell.y - 1) is int b && b == TypeAt(cell.x, cell.y - 2)) allowed.Remove(b);

        int type = allowed[Random.Range(0, allowed.Count)];

        var go = Instantiate(tilePrefabs[type], cell.worldPos, Quaternion.identity, transform);
        var tile = go.GetComponent<Tile>();
        tile.Init(type, cell);
        cell.tile = tile;
    }

    // overloaded SpawnTile that takes a position
    private Tile SpawnTile(Cell cell, int type, Vector3 pos)
    {
        var go = Instantiate(tilePrefabs[type], pos, Quaternion.identity, transform);
        var tile = go.GetComponent<Tile>();
        tile.Init(type, cell);
        cell.tile = tile;
        return tile;
    }

    // tries swapping 2 given cells/tiles
    public void TrySwap(Cell a, Cell b)
    {
        if (a.tile == null || b.tile == null) return;
        if (a.obstacle != null && a.obstacle.BlocksSwap) return;
        if (b.obstacle != null && b.obstacle.BlocksSwap) return;

        StartCoroutine(SwapRoutine(a, b));
    }

    // swaps the data of the given 2 cells
    private void SwapData(Cell a, Cell b)
    {
        (a.tile, b.tile) = (b.tile, a.tile);
        a.tile.cell = a;
        b.tile.cell = b;
    }

    // coroutine for swapping 2 cells/tiles
    private IEnumerator SwapRoutine(Cell a, Cell b)
    {
        boardInput.inputLocked = true;

        SwapData(a, b);
        yield return AnimateSwap(a, b);

        if (!HasMatchAt(a) && !HasMatchAt(b))
        {
            // invalid move, swap back
            SwapData(a, b);
            yield return AnimateSwap(a, b);
        }
        else
        {
            // timer only pauses when a VALID mode is made
            timer.StopTimer();
            yield return ResolveBoard();
        }

        if (goalTracker.AllMet())
        {
            gameManager.Win();
            yield break;
        }

        boardInput.inputLocked = false;
        timer.StartTimer();
    }

    // coroutine for cell/tile swap animation
    private IEnumerator AnimateSwap(Cell a, Cell b)
    {
        var m1 = StartCoroutine(a.tile.MoveTo(a.worldPos));
        var m2 = StartCoroutine(b.tile.MoveTo(b.worldPos));
        yield return m1;
        yield return m2;
    }

    // creates a hashset with matches found on the board
    private HashSet<Cell> FindMatches()
    {
        var result = new HashSet<Cell>();
        int w = Cells.GetLength(0), h = Cells.GetLength(1);

        // horizontal runs/match-checking
        for (int y = 0; y < h; y++)
        {
            int x = 0;
            while (x < w)
            {
                int t = TileType(x, y);
                if (t < 0) { x++; continue; }

                int run = 1;
                while (x + run < w && TileType(x + run, y) == t) run++;

                if (run >= 3)
                {
                    for (int i = 0; i < run; i++) result.Add(Cells[x + i, y]);
                }

                x += run;
            }
        }

        // vertical runs/match-checking
        for (int x = 0; x < w; x++)
        {
            int y = 0;
            while (y < h)
            {
                int t = TileType(x, y);
                if (t < 0) { y++; continue; }

                int run = 1;
                while (y + run < h && TileType(x, y + run) == t) run++;

                if (run >= 3)
                {
                    for (int i = 0; i < run; i++) result.Add(Cells[x, y + i]);
                }

                y += run;
            }
        }

        return result;
    }

    // coroutine that loops until the board is stable
    private IEnumerator ResolveBoard()
    {
        int pass = 0;
        while (true)
        {
            var matches = FindMatches();
            if (matches.Count == 0) break;

            // in case the coroutine runs for too long, force-break
            if (++pass > 50)
            {
                Debug.LogError("ResolveBoard ran 50 passes, bailing out");
                break;
            }

            yield return new WaitForSeconds(0.15f);
            yield return ClearMatches(matches);

            yield return new WaitForSeconds(0.1f);
            yield return ApplyGravity();
            yield return Refill();

            yield return new WaitForSeconds(0.2f);
            // yield return null; - not necessary at the moment, BUT if we add a "fast mode" for tile clearing, this needs to replace the "WaitForSeconds" calls
        }
    }

    // clears matches 
    IEnumerator ClearMatches(HashSet<Cell> matches)
    {
        // damages any damage-able obstacles next to any cleared cell once each
        var hitCells = new HashSet<Cell>();
        foreach (var c in matches)
        {
            foreach (var d in Dirs)
            {
                var n = GetCell(c.x + d.x, c.y + d.y);
                if (n != null && n.obstacle != null) hitCells.Add(n);
            }
        }

        foreach (var n in hitCells)
        {
            if (n.obstacle.TakeHit()) n.obstacle = null;
        }

        // clears tile data and visuals 
        var clears = new List<Coroutine>();
        foreach (var c in matches)
        {
            goalTracker.RecordCleared(c.tile.type);

            var g = c.tile;
            c.tile = null;
            clears.Add(StartCoroutine(g.Clear()));
        }

        foreach (var co in clears) yield return co;

        // score += matches.Count * cascadeMultiplier;
    }

    // applies gravity to tiles that can fall
    private IEnumerator ApplyGravity()
    {
        var moves = new List<Coroutine>();
        int w = Cells.GetLength(0), h = Cells.GetLength(1);

        for (int x = 0; x < w; x++)
        {
            int writeY = -1;
            for (int y = 0; y < h; y++)
            {
                var c = Cells[x, y];
                if (c == null || !c.CanHoldTile) { writeY = -1; continue; }

                if (c.tile == null)
                {
                    if (writeY < 0) writeY = y;
                }
                else if (writeY >= 0)
                {
                    var target = Cells[x, writeY];
                    var tile = c.tile;

                    c.tile = null;
                    target.tile = tile;
                    tile.cell = target;

                    moves.Add(StartCoroutine(tile.MoveTo(target.worldPos)));
                    writeY++;
                }
            }
        }

        foreach (var co in moves) yield return co;
    }

    // refills the grid after matches 
    private IEnumerator Refill()
    {
        var moves = new List<Coroutine>();
        int w = Cells.GetLength(0), h = Cells.GetLength(1);
        float cellH = boardMap.layoutGrid.cellSize.y;

        for (int x = 0; x < w; x++)
        {
            int y = 0;
            while (y < h)
            {
                var c = Cells[x, y];
                if (c == null || !c.CanHoldTile) { y++; continue; }

                // if start of segment is found
                int start = y;
                while (y < h && Cells[x, y] != null && Cells[x, y].CanHoldTile) y++;
                int end = y - 1;

                int empties = 0;
                for (int i = start; i <= end; i++)
                {
                    if (Cells[x, i].tile == null) empties++;
                }

                for (int i = start; i <= end; i++)
                {
                    var cell = Cells[x, i];
                    if (cell.tile != null) continue;

                    int type = Random.Range(0, tilePrefabs.Length);
                    Vector3 spawnPos = cell.worldPos + Vector3.up * (cellH * empties);

                    var tile = SpawnTile(cell, type, spawnPos);
                    moves.Add(StartCoroutine(tile.MoveTo(cell.worldPos)));
                }
            }
        }

        foreach (var co in moves) yield return co;
    }
}