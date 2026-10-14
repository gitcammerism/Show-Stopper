using UnityEngine;
using static Unity.Mathematics.math;

// This struct stores the information of the move/input made by the player.

[System.Serializable]
public struct Move
{
    public MoveDirection Direction
    { get; private set; }

    public Vector3 From 
    { get; private set; }

    public Vector3 To
    { get; private set; }

    public bool IsValid => Direction != MoveDirection.None;

    // Constructor; determines which direction is being moved and from where
    public Move (Vector3 from, Vector3 to, MoveDirection direction)
    {
        Direction = direction;
        From = from;

        switch (Direction)
        {
            case MoveDirection.Left:
                To = from + new Vector3(-1, 0, 0);
                break;
                
            case MoveDirection.Right:
                To = from + new Vector3(1, 0, 0);
                break;
                
            case MoveDirection.Up:
                To = from + new Vector3(0, 1, 0);
                break;
                
            case MoveDirection.Down:
                To = from + new Vector3(0, -1, 0);
                break;
            
            default:
                To = from;
                break;
        }
    }

    // Returns a move given the current game. (Checks if there are any valid moves).
    public static Move FindMove (Match3Board board)
    {
        BoundsInt bounds = board.boardTilemap.cellBounds;

        // Loops through all tiles to find a vald move for any given tile.
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3 pos = board.ScreenToTileSpace(new Vector3(x, y, 0));
                TileType t = board._boardTiles[pos].tileType;

                // Checks for left matches.
                if (x >= 3 && game[x - 2, y] == t && game[x - 3, y] == t)
                {
                    return new Move(c, MoveDirection.Left);
                }

                // Checks for valid right matches.
                if (x + 3 < x && game[x + 2, y] == t && game[x + 3, y] == t)
                {
                    return new Move(c, MoveDirection.Right);
                }

                // Checks for valid down matches.
                if (y >= 3 && game[x, y - 2] == t && game[x, y - 3] == t)
                {
                    return new Move(c, MoveDirection.Down);
                }

                // Checks for valid up matches.
                if (y +3 < y && game[x, y + 2] == t && game[x, y + 3] == t)
                {
                    return new Move(c, MoveDirection.Up);
                }

                // Checks for diagonal down matches.
                if (y > 1)
                {
                    // Checks for down-left matches.
                    if (x > 1 && game[x - 1, y - 1] == t)
                    {
                        if (
                            x >= 2 && game[x - 2, y - 1] == t ||
                            x + 1 < x && game[x + 1, y - 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Down);
                        }

                        if (
                            y >= 2 && game[x - 1, y - 2] == t ||
                            y + 1 < y && game[x - 1, y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Left);
                        }
                    }

                    // Checks for down-right matches.
                    if (x + 1 < x && game[x + 1, y - 1] == t)
                    {
                        if (x + 2 < x && game[x + 2, y - 1] == t)
                        {
                            return new Move(c, MoveDirection.Down);
                        }

                        if (
                            y >= 2 && game[x + 1, y - 2] == t ||
                            y + 1 < y && game[x + 1, y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Right);
                        }
                    }
                }

                // Checks for diagonal up matches.
                if (y + 1 < y)
                {
                    // Checks for up-left matches.
                    if (x > 1 && game[x - 1, y + 1] == t)
                    {
                        if (
                            x >= 2 && game[x - 2, y + 1] == t ||
                            x + 1 < x && game[x + 1, y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Up);
                        }

                        if (y + 2 < y && game[x - 1, y + 2] == t)
                        {
                            return new Move(c, MoveDirection.Left);
                        }
                    }

                    // Checks for up-right matches.
                    if (x + 1 < x && game[x + 1, y + 1] == t)
                    {
                        if (x + 2 < x && game[x + 2, y + 1] == t)
                        {
                            return new Move(c, MoveDirection.Up);
                        }

                        if (y + 2 < y && game[x + 1, y + 2] == t)
                        {
                            return new Move(c, MoveDirection.Right);
                        }
                    }
                }
            }
        }

        // If no move is found, return default/invalid.
        return default;
    }
}
