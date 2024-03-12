using UnityEngine;

public class PathManager : MonoBehaviour
{
    private GridManager gridManager;
    private GameObject[,] grid;
    private GameObject player;
    private Vector2 goalIndices;
    private Vector2 currentIndices;

    private void Start()
    {
        gridManager = GridManager.Instance;
        grid = gridManager.GetGrid();

       
        player = GameObject.FindGameObjectWithTag("Ship");

        
        goalIndices = FindTileIndices(TileStatus.GOAL);

        
        currentIndices = GetPlayerGridIndices();

       // MoveToGoal();
    }

    private Vector2 FindTileIndices(TileStatus targetStatus)
    {
        for (int row = 0; row < gridManager.rows; row++)
        {
            for (int col = 0; col < gridManager.columns; col++)
            {
                if (grid[row, col].GetComponent<TileScript>().status == targetStatus)
                {
                    return new Vector2(col, row);
                }
            }
        }
        return Vector2.zero;
    }

    private Vector2 GetPlayerGridIndices()
    {
        // Get the player's current position in world coordinates.
        Vector3 playerPosition = player.transform.position;

        // Convert the world position to grid indices.
        int col = Mathf.FloorToInt(playerPosition.x + gridManager.columns / 2);
        int row = Mathf.FloorToInt(-playerPosition.y + gridManager.rows / 2);

        // Ensure the indices are within the grid boundaries.
        col = Mathf.Clamp(col, 0, gridManager.columns - 1);
        row = Mathf.Clamp(row, 0, gridManager.rows - 1);

        return new Vector2(col, row);
    }

    private void MoveToGoal()
    {
        while (currentIndices != goalIndices)
        {
            Vector2 nextIndices = FindNextTile();
            MovePlayerToTile(nextIndices);
            currentIndices = nextIndices;
        }

        Debug.Log("Reached Goal!");
    }

    private Vector2 FindNextTile()
    {
        
        int dx = (int)Mathf.Sign(goalIndices.x - currentIndices.x);
        int dy = (int)Mathf.Sign(goalIndices.y - currentIndices.y);

        int nextCol = (int)currentIndices.x + dx;
        int nextRow = (int)currentIndices.y + dy;

        
        nextCol = Mathf.Clamp(nextCol, 0, gridManager.columns - 1);
        nextRow = Mathf.Clamp(nextRow, 0, gridManager.rows - 1);

        return new Vector2(nextCol, nextRow);
    }

    private void MovePlayerToTile(Vector2 indices)
    {
        
        Vector3 nextPosition = new Vector3(indices.x, -indices.y, 0f);
        player.transform.position = nextPosition;

        
    }
}
