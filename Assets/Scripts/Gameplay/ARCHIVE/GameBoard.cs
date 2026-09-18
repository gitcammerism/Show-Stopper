using UnityEngine;

/*
 * Last Modified: 09/15/2026 by Chandler Guzman
 * 
 * This script creates the game board and stores a reference to all board tiles.
 *
 * Chandler TO-DO:
 * - Store tiles in _allTiles array; determine how matches will be made and tracked
 */

public class GameBoard : MonoBehaviour
{
    [SerializeField] private int boardWidth;
    [SerializeField] private int boardHeight;
    [SerializeField] private GameObject tilePrefab;
    private BackgroundTile[,] _allTiles;
    
    // Creates the 2D tile array/game board at the start of the game.
    void Start()
    {
        // Width and height are divided by 2 to ensure that the board is centered.
        transform.position = new Vector3((boardWidth / 2 * -1), (boardHeight / 2 * -1), 0);
        _allTiles = new BackgroundTile[boardWidth, boardHeight];
        SetUpTiles();
    }

    // Creates all game board tiles.
    private void SetUpTiles()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                // Adds the board's x and y position as an offset so the board starts where the GameObject is located
                Vector2 tilePosition = new Vector2(i + transform.position.x, j + transform.position.y);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                
                // Organizes tiles in the Hierarchy
                backgroundTile.transform.SetParent(transform);
                backgroundTile.name = "(" + i + ", " + j + ")";
            }
        }
    }
}
