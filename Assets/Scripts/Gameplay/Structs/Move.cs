using Unity.Mathematics;
using static Unity.Mathematics.math;

// This struct stores the information of the move/input made by the player.

[System.Serializable]
public struct Move
{
    public MoveDirection Direction
    { get; private set; }

    public int2 From 
    { get; private set; }

    public int2 To
    { get; private set; }

    public bool IsValid => Direction != MoveDirection.None;

    // Constructor; determines which direction is being moved and from where
    public Move (int2 coordinates, MoveDirection direction)
    {
        Direction = direction;
        From = coordinates;
        To = coordinates + direction switch
        {
            MoveDirection.Up => int2(0, 1),
            MoveDirection.Down => int2(0, -1),
            MoveDirection.Right => int2(1, 0),
            _ => int2(-1, 0)
        };
    }

    // Returns a move given the current game. (Checks if there are any valid moves).
    public static Move FindMove (Match3Game game)
    {
        int2 s = game.GridSize;

        // Loops through all tiles to find a vald move for any given tile.
        for (int2 c = 0; c.y < s.y; c.y++)
        {
            for (c.x = 0; c.x < s.x; c.x++)
            {
                TileState t = game[c];

                // Checks for left matches.
                if (c.x >= 3 && game[c.x - 2, c.y] == t && game[c.x - 3, c.y] == t)
                {
                    return new Move(c, MoveDirection.Left);
                }

                // Checks for valid right matches.
                if (c.x + 3 < s.x && game[c.x + 2, c.y] == t && game[c.x + 3, c.y] == t)
                {
                    return new Move(c, MoveDirection.Right);
                }

                // Checks for valid down matches.
                if (c.y >= 3 && game[c.x, c.y - 2] == t && game[c.x, c.y - 3] == t)
                {
                    return new Move(c, MoveDirection.Down);
                }

                // Checks for valid up matches.
                if (c.y +3 < s.y && game[c.x, c.y + 2] == t && game[c.x, c.y + 3] == t)
                {
                    return new Move(c, MoveDirection.Up);
                }

                // Checks for diagonal down matches.
                if (c.y > 1)
                {
                    // Checks for down-left matches.
                    if (c.x > 1 && game[c.x - 1, c.y - 1] == t)
                    {
                        if (
                            c.x >= 2 && game[c.x - 2, c.y - 1] == t ||
                            c.x + 1 < s.x && game[c.x + 1, c.y - 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Down);
                        }

                        if (
                            c.y >= 2 && game[c.x - 1, c.y - 2] == t ||
                            c.y + 1 < s.y && game[c.x - 1, c.y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Left);
                        }
                    }

                    // Checks for down-right matches.
                    if (c.x + 1 < s.x && game[c.x + 1, c.y - 1] == t)
                    {
                        if (c.x + 2 < s.x && game[c.x + 2, c.y - 1] == t)
                        {
                            return new Move(c, MoveDirection.Down);
                        }

                        if (
                            c.y >= 2 && game[c.x + 1, c.y - 2] == t ||
                            c.y + 1 < s.y && game[c.x + 1, c.y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Right);
                        }
                    }
                }

                // Checks for diagonal up matches.
                if (c.y + 1 < s.y)
                {
                    // Checks for up-left matches.
                    if (c.x > 1 && game[c.x - 1, c.y + 1] == t)
                    {
                        if (
                            c.x >= 2 && game[c.x - 2, c.y + 1] == t ||
                            c.x + 1 < s.x && game[c.x + 1, c.y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Up);
                        }

                        if (c.y + 2 < s.y && game[c.x - 1, c.y + 2] == t)
                        {
                            return new Move(c, MoveDirection.Left);
                        }
                    }

                    // Checks for up-right matches.
                    if (c.x + 1 < s.x && game[c.x + 1, c.y + 1] == t)
                    {
                        if (c.x + 2 < s.x && game[c.x + 2, c.y + 1] == t)
                        {
                            return new Move(c, MoveDirection.Up);
                        }

                        if (c.y + 2 < s.y && game[c.x + 1, c.y + 2] == t)
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
