using Unity.Mathematics;

/*
 * Last Modified: 09/16/2026 by Chandler Guzman
 * 
 * This is a struct that handles the 2D grid of the match-3 game.
 *
 * Chandler TO-DO:
 * - 
 */

[System.Serializable]
public struct MatchGrid2D<T>
{
    private T[] _cells;
    private int2 _size;

    // Allows other classes to obtain grid information without modifying
    public int2 Size => _size;
    public int SizeX => _size.x;
    public int SizeY => _size.y;

    // Constructor which creates the 2D grid
    public MatchGrid2D (int2 size)
    {
        this._size = size;
        _cells = new T[size.x * size.y];
    }

    // Indexers that allow the X and Y coordinates to be set and gotten 
    public T this[int x, int y]
    {
        get => _cells[y * _size.x + x];
        set => _cells[y * _size.x + x] = value;
    }

    public T this[int2 c]
    {
        get => _cells[c.y * _size.x + c.x];
        set => _cells[c.y * _size.x + c.x] = value;
    }

    // Checks whether the grid is undefined
    public bool IsUndefined => _cells == null || _cells.Length == 0;

    // Checks whether the given coordinates are valid
    public bool AreValidCoordinates (int2 c) => 
        0 <= c.x && c.x < _size.x && 0 <= c.y && c.y < _size.y;

    // Swaps 2 grid elements
    public void Swap (int2 a, int2 b) => (this[a], this[b]) = (this[b], this[a]);
}
