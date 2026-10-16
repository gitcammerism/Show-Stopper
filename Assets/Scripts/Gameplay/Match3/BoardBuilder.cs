using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BoardBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GoalTracker goalTracker;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private BoardInput boardInput;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap boardMap;
    [SerializeField] private Tilemap obstacleMap;

    [Header("Prefabs")]
    [SerializeField] private ObstacleDefinition[] obstacleDefs;
    [SerializeField] private TilePrefab[] tilePrefabs;

    [Header("Customization")]
    [SerializeField] Sprite maskSprite;
    [SerializeField] private float dropStagger = 0.08f; // stagger for ice block tile spawns

    private List<TileType> spawnableTypes;
    private Dictionary<TileBase, ObstacleDefinition> obstacleLookup;
    private Dictionary<TileType, GameObject> prefabLookup;
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
        // handles dictionary and list definitions
        prefabLookup = tilePrefabs.ToDictionary(e => e.type, e => e.prefab);
        spawnableTypes = tilePrefabs.Select(e => e.type).ToList();
        obstacleLookup = obstacleDefs.ToDictionary(d => d.paintedTile);

        // compresses tilemap bounds and then spawns tiles
        boardMap.CompressBounds();
        bounds = boardMap.cellBounds;
        Build();
        // BuildBoardMask();

        // ensures the board starts with valid moves
        if (!HasValidMove())
        {
            if (!TryShuffle(100)) RegenerateTiles();
            SnapTilesToCells();
        }
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
    private TileType GetTypeAt(int x, int y)
    {
        if (!IsPlayable(x, y)) return TileType.None;
        var t = Cells[x, y].tile;
        return t != null ? t.type : TileType.None;
    }

    // length of a "run" (match) along one axis
    private int RunLength(Cell c, int dx, int dy)
    {
        TileType t = GetTypeAt(c.x, c.y);
        if (t == TileType.None) return 0;

        int max = Mathf.Max(Cells.GetLength(0), Cells.GetLength(1));
        int count = 1;
        for (int i = 1; i < max && GetTypeAt(c.x + dx * i, c.y + dy * i) == t; i++) count++;
        for (int i = 1; i < max && GetTypeAt(c.x - dx * i, c.y - dy * i) == t; i++) count++;
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
        var allowed = new List<TileType>(spawnableTypes);

        var l1 = GetTypeAt(cell.x - 1, cell.y);
        if (l1 != TileType.None && l1 == GetTypeAt(cell.x - 2, cell.y)) allowed.Remove(l1);

        var d1 = GetTypeAt(cell.x, cell.y - 1);
        if (d1 != TileType.None && d1 == GetTypeAt(cell.x, cell.y - 2)) allowed.Remove(d1);

        if (allowed.Count == 0) allowed.AddRange(spawnableTypes);

        var type = allowed[Random.Range(0, allowed.Count)];
        CreateTile(cell, type, cell.worldPos);
    }

    // creates tile as given position
    private Tile CreateTile(Cell cell, TileType type, Vector3 pos)
    {
        var go = Instantiate(prefabLookup[type], pos, Quaternion.identity, transform);
        var tile = go.GetComponent<Tile>();
        tile.Init(type, cell);
        cell.tile = tile;

        var sr = go.GetComponentInChildren<SpriteRenderer>();
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

        // if all level goals are met, player wins yay
        if (goalTracker.AllMet())
        {
            gameManager.Win();
            yield break;
        }

        // checks to make sure there are valid moves and, if not, shuffle board
        yield return EnsureMovesAvailable();

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

    // creates a hashset with matches/runs found on the board
    private List<List<Cell>> FindRuns()
    {
        var runs = new List<List<Cell>>();
        int w = Cells.GetLength(0), h = Cells.GetLength(1);

        // horizontal runs/match-checking
        for (int y = 0; y < h; y++)
        {
            int x = 0;
            while (x < w)
            {
                TileType t = GetTypeAt(x, y);
                if (t == TileType.None) { x++; continue; }

                int run = 1;
                while (x + run < w && GetTypeAt(x + run, y) == t) run++;

                if (run >= 3)
                {
                    var list = new List<Cell>(run);
                    for (int i = 0; i < run; i++) list.Add(Cells[x + i, y]);
                    runs.Add(list);
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
                TileType t = GetTypeAt(x, y);
                if (t == TileType.None) { y++; continue; }

                int run = 1;
                while (y + run < h && GetTypeAt(x, y + run) == t) run++;

                if (run >= 3)
                {
                    var list = new List<Cell>(run);
                    for (int i = 0; i < run; i++) list.Add(Cells[x, y + i]);
                    runs.Add(list);
                }

                y += run;
            }
        }

        return runs;
    }

    // converts FindRuns list into a hashset
    private HashSet<Cell> CellsOf(List<List<Cell>> runs)
    {
        var set = new HashSet<Cell>();
        foreach (var run in runs)
        {
            foreach (var c in run) set.Add(c);
        }
        return set;
    }

    // coroutine that loops until the board is stable
    private IEnumerator ResolveBoard()
    {
        int cascade = 0;
        while (true)
        {
            var runs = FindRuns();
            if (runs.Count == 0) break;

            cascade++;
            var matches = CellsOf(runs);

            yield return new WaitForSeconds(0.15f); // brief pause before updating score
            scoreManager.ScoreRuns(runs, cascade);
            yield return ClearMatches(matches);

            yield return new WaitForSeconds(0.1f);
            yield return ApplyGravity();
            yield return Refill();

            yield return new WaitForSeconds(0.2f);
            // yield return null; - not necessary at the moment, BUT if we add a "fast mode" for tile clearing, this needs to replace the "WaitForSeconds" calls
        }
    }

    // clears matches 
    private IEnumerator ClearMatches(HashSet<Cell> matches)
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

                // where new tiles will appear 
                Vector3 origin = Cells[x, end].worldPos + Vector3.up * cellH;

                int order = 0;
                for (int i = start; i <= end; i++)
                {
                    var cell = Cells[x, i];
                    if (cell.tile != null) continue;

                    var type = spawnableTypes[Random.Range(0, spawnableTypes.Count)];
                    var tile = CreateTile(cell, type, origin);
                    moves.Add(StartCoroutine(DropIn(tile, origin, cell.worldPos, order * dropStagger)));
                    order++;
                }
            }
        }

        foreach (var co in moves) yield return co;
    }

    // animates tiles dropping in
    IEnumerator DropIn(Tile tile, Vector3 origin, Vector3 target, float delay)
    {
        float cellH = boardMap.layoutGrid.cellSize.y;
        var renderers = tile.GetComponentsInChildren<SpriteRenderer>();

        SetAlpha(renderers, 0f);
        if (delay > 0f) yield return new WaitForSeconds(delay);

        StartCoroutine(tile.MoveTo(target));

        // fade in over the first cell of travel
        while (tile != null && (tile.transform.position - target).sqrMagnitude > 0.0001f)
        {
            float a = Mathf.Clamp01((origin.y - tile.transform.position.y) / cellH);
            SetAlpha(renderers, a);
            yield return null;
        }

        if (tile != null) SetAlpha(renderers, 1f);
    }

    static void SetAlpha(SpriteRenderer[] renderers, float a)
    {
        foreach (var sr in renderers)
        {
            var c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }

    // swaps tile references (temporary for checking matches)
    private void SwapTileRefs(Cell a, Cell b) => (a.tile, b.tile) = (b.tile, a.tile);

    // checks for valid moves on the board
    private bool FindValidMove(out Cell moveA, out Cell moveB)
    {
        int w = Cells.GetLength(0), h = Cells.GetLength(1);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var c = Cells[x, y];
                if (c == null || c.tile == null) continue;

                for (int k = 0; k < 2; k++)
                {
                    var n = k == 0 ? GetCell(x + 1, y) : GetCell(x, y + 1);
                    if (n == null || n.tile == null) continue;
                    if (n.tile.type == c.tile.type) continue;

                    SwapTileRefs(c, n);
                    bool ok = HasMatchAt(c) || HasMatchAt(n);
                    SwapTileRefs(c, n);

                    if (ok) { moveA = c; moveB = n; return true; }
                }
            }
        }

        moveA = moveB = null;
        return false;
    }

    // checks if there is a valid move on the current board
    private bool HasValidMove() => FindValidMove(out _, out _);

    // shuffles the board using the fisher-yates shuffle algorithm :D
    private bool TryShuffle(int maxAttempts)
    {
        var cells = new List<Cell>();
        var tiles = new List<Tile>();

        foreach (var c in Cells)
        {
            if (c != null && c.tile != null) { cells.Add(c); tiles.Add(c.tile); }
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // fisher-yates shuffle algorithm start here
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
            }

            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].tile = tiles[i];
                tiles[i].cell = cells[i];
            }

            if (FindRuns().Count == 0 && HasValidMove()) return true;
        }

        return false;
    }

    // in case shuffling doesn't work (unlikely) this is a fallback method that re-generates the board
    private void RegenerateTiles()
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            foreach (var c in Cells)
            {
                if (c != null && c.tile != null)
                {
                    Destroy(c.tile.gameObject);
                    c.tile = null;
                }
            }

            // bottom-to-top, left-to-right so SpawnTile's exclusion logic works
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                for (int x = 0; x < Cells.GetLength(0); x++)
                {
                    var c = Cells[x, y];
                    if (c != null && c.CanHoldTile) SpawnTile(c);
                }
            }

            if (HasValidMove()) return;
        }

        Debug.LogWarning("Couldn't generate a board with valid moves after 50 attempts; check layout & tile count.");
    }

    // animates tiles as they shuffle!
    private IEnumerator AnimateTilesToCells()
    {
        var moves = new List<Coroutine>();
        foreach (var c in Cells)
        {
            if (c == null || c.tile == null) continue;
            if ((c.tile.transform.position - c.worldPos).sqrMagnitude < 0.0001f) continue;
            moves.Add(StartCoroutine(c.tile.MoveTo(c.worldPos)));
        }

        foreach (var co in moves) yield return co;
    }

    // snaps tiles to the grid
    private void SnapTilesToCells()
    {
        foreach (var c in Cells)
        {
            if (c != null && c.tile != null) c.tile.transform.position = c.worldPos;
        }
    }

    // initiates game board shuffling
    private IEnumerator EnsureMovesAvailable()
    {
        if (HasValidMove()) yield break;

        // delay before shuffle starts
        yield return new WaitForSeconds(0.5f);

        // tries to shuffle current tiles 100 times before fallback method
        if (TryShuffle(100))
        {
            yield return AnimateTilesToCells();
        }
        else
        {
            RegenerateTiles();
        }
    }

    // builds sprite mask around the game grid (makes spawning tiles not visible)
    private void BuildBoardMask()
    {
        var grid = boardMap.layoutGrid;
        Vector3 size = Vector3.Scale(grid.cellSize, grid.transform.lossyScale);

        var root = new GameObject("BoardMask").transform;

        foreach (var c in Cells)
        {
            if (c == null) continue; // holes/missing tiles get no mask
            if (c.obstacle != null && c.obstacle.definition.indestructible) continue; // ignore ice tiles

            var go = new GameObject("MaskCell");
            go.transform.SetParent(root, false);
            go.transform.position = c.worldPos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);

            var mask = go.AddComponent<SpriteMask>();
            mask.sprite = maskSprite;
        }
    }
}