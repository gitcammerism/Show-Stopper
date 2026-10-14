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
    public Move (Vector3 from, MoveDirection direction)
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
    //public static Move FindMove (Match3Board board)
    //{
    //    BoundsInt bounds = board.boardTilemap.cellBounds;

    //    // Loops through all tiles to find a vald move for any given tile.
    //    for (int y = bounds.yMin; y < bounds.yMax; y++)
    //    {
    //        for (int x = bounds.xMin; x < bounds.xMax; x++)
    //        {
    //            Vector3 pos = board.ScreenToTileSpace(new Vector3(x, y, 0));
    //            Debug.Log(pos);
    //            if (board._boardTiles[pos] == null || board._boardTiles[pos].tileType == TileType.None) continue;
    //            TileType t = board._boardTiles[pos].tileType;

    //            // Checks for left matches.
    //            if (x >= 3 && board._boardTiles[new Vector3(pos.x - 2, pos.y, 0)].tileType == t &&
    //                board._boardTiles[new Vector3(pos.x - 3, pos.y, 0)].tileType == t)
    //            {
    //                return new Move(pos, MoveDirection.Left);
    //            }

    //            // Checks for valid right matches.
    //            if (x >= 3 && board._boardTiles[new Vector3(pos.x + 2, pos.y, 0)].tileType == t &&
    //                board._boardTiles[new Vector3(pos.x + 3, pos.y, 0)].tileType == t)
    //            {
    //                return new Move(pos, MoveDirection.Right);
    //            }

    //            // Checks for valid down matches.
    //            if (x >= 3 && board._boardTiles[new Vector3(pos.x, pos.y - 2, 0)].tileType == t &&
    //                board._boardTiles[new Vector3(pos.x, pos.y - 3, 0)].tileType == t)
    //            {
    //                return new Move(pos, MoveDirection.Down);
    //            }

    //            // Checks for valid up matches.
    //            if (x >= 3 && board._boardTiles[new Vector3(pos.x, pos.y + 2, 0)].tileType == t &&
    //                board._boardTiles[new Vector3(pos.x, pos.y + 3, 0)].tileType == t)
    //            {
    //                return new Move(pos, MoveDirection.Up);
    //            }

    //            // Checks for diagonal down matches.
    //            if (y > 1)
    //            {
    //                // Checks for down-left matches.
    //                if (x > 1 && board._boardTiles[new Vector3(pos.x - 1, pos.y - 1, 0)].tileType == t)
    //                {
    //                    if (
    //                        x >= 2 && board._boardTiles[new Vector3(pos.x - 2, pos.y - 1, 0)].tileType == t ||
    //                        x + 1 < x && board._boardTiles[new Vector3(pos.x + 1, pos.y - 1, 0)].tileType == t
    //                    )
    //                    {
    //                        return new Move(pos, MoveDirection.Down);
    //                    }

    //                    if (
    //                        y >= 2 && board._boardTiles[new Vector3(pos.x - 1, pos.y - 2, 0)].tileType == t ||
    //                        y + 1 < y && board._boardTiles[new Vector3(pos.x - 1, pos.y + 1, 0)].tileType == t
    //                    )
    //                    {
    //                        return new Move(pos, MoveDirection.Left);
    //                    }
    //                }

    //                // Checks for down-right matches.                   
    //                if (x + 1 < x && board._boardTiles[new Vector3(pos.x + 1, pos.y - 1, 0)].tileType == t)
    //                {                        
    //                    if (x + 2 < x && board._boardTiles[new Vector3(pos.x + 2, pos.y - 1, 0)].tileType == t)
    //                    {
    //                        return new Move(pos, MoveDirection.Down);
    //                    }
                        
    //                    if (
    //                        y >= 2 && board._boardTiles[new Vector3(pos.x + 1, pos.y - 2, 0)].tileType == t ||
    //                        y + 1 < y && board._boardTiles[new Vector3(pos.x + 1, pos.y + 1, 0)].tileType == t
    //                    )
    //                    {
    //                        return new Move(pos, MoveDirection.Right);
    //                    }
    //                }
    //            }

    //            // Checks for diagonal up matches.
    //            if (y + 1 < y)
    //            {                    
    //                // Checks for up-left matches.
    //                if (x > 1 && board._boardTiles[new Vector3(pos.x - 1, pos.y + 1, 0)].tileType == t)
    //                {
                        
    //                    if (
    //                        x >= 2 && board._boardTiles[new Vector3(pos.x - 2, pos.y + 1, 0)].tileType == t ||
    //                        x + 1 < x && board._boardTiles[new Vector3(pos.x + 1, pos.y + 1, 0)].tileType == t
    //                    )
    //                    {
    //                        return new Move(pos, MoveDirection.Up);
    //                    }
                        
    //                    if (y + 2 < y && board._boardTiles[new Vector3(pos.x - 1, pos.y + 2, 0)].tileType == t)
    //                    {
    //                        return new Move(pos, MoveDirection.Left);
    //                    }
    //                }

    //                // Checks for up-right matches.

    //                if (x + 1 < x && board._boardTiles[new Vector3(pos.x + 1, pos.y + 1, 0)].tileType == t)
    //                {                        
    //                    if (x + 2 < x && board._boardTiles[new Vector3(pos.x + 2, pos.y + 1, 0)].tileType == t)
    //                    {
    //                        return new Move(pos, MoveDirection.Up);
    //                    }
                      
    //                    if (y + 2 < y && board._boardTiles[new Vector3(pos.x + 1, pos.y + 2, 0)].tileType == t)
    //                    {
    //                        return new Move(pos, MoveDirection.Right);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    // If no move is found, return default/invalid.
    //    return default;
    //}
}
