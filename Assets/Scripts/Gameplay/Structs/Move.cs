using Unity.Mathematics;
using static Unity.Mathematics.math;

/*
 * Last Modified: 09/21/2026 by Chandler Guzman
 * 
 * This struct stores the information of the move/input made by the player.
 */

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
}
