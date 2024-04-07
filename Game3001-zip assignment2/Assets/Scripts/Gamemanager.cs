using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
public class Gamemanager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap obstacleTilemap;
    public Tilemap debugTilemap;
    public Tile startingTile;
    public Tile goalTile;
    public TMP_Text debugText;
    public TMP_Text instructionText;
    public AudioSource backgroundMusic;

    private Vector3Int startingPosition;
    private Vector3Int goalPosition;
    private bool debugMode = false;

    private List<Vector3Int> shortestPath;
    public float moveSpeed = 5f;
    public float moveDelay = 0.5f;

    private void Start()
    {
        // Initialize starting and goal positions
        startingPosition = Vector3Int.zero;
        goalPosition = new Vector3Int(9, 9, 0); // Assuming bottom-right tile as the goal
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleDebugMode();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            FindShortestPath();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MoveActorAlongPath();
        }
    }

    private void ToggleDebugMode()
    {
        debugMode = !debugMode;

        if (debugMode)
        {
            // Display debug view
            debugTilemap.gameObject.SetActive(true);
            instructionText.text = "Debug mode activated. Left-click to set starting tile, right-click to set goal tile.";
        }
        else
        {
            // Hide debug view
            debugTilemap.gameObject.SetActive(false);
            instructionText.text = "Press H for Debug mode, F to find shortest path, M to move actor.";
        }
    }

    private void FindShortestPath()
    {
        // Clear previous path
        ClearPath();

        // A* algorithm implementation
        List<Vector3Int> openList = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, float> gScores = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        openList.Add(startingPosition);
        gScores[startingPosition] = 0f;

        while (openList.Count > 0)
        {
            Vector3Int current = GetLowestFScoreTile(openList, gScores);
            openList.Remove(current);

            if (current == goalPosition)
            {
                ReconstructPath(cameFrom, current);
                break;
            }

            foreach (Vector3Int neighbor in GetNeighborTiles(current))
            {
                if (closedSet.Contains(neighbor) || IsObstacleTile(neighbor))
                    continue;

                float tentativeGScore = gScores[current] + GetDistance(current, neighbor);

                if (!openList.Contains(neighbor) || tentativeGScore < gScores[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScores[neighbor] = tentativeGScore;
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }

            closedSet.Add(current);
        }

        // Update tilemap to display viable paths and highlight the shortest path
        UpdateTilemap();
    }

    private Vector3Int GetLowestFScoreTile(List<Vector3Int> openList, Dictionary<Vector3Int, float> gScores)
    {
        float lowestFScore = float.MaxValue;
        Vector3Int lowestFScoreTile = Vector3Int.zero;

        foreach (Vector3Int tile in openList)
        {
            float fScore = gScores[tile] + GetHeuristicCost(tile, goalPosition);
            if (fScore < lowestFScore)
            {
                lowestFScore = fScore;
                lowestFScoreTile = tile;
            }
        }

        return lowestFScoreTile;
    }

    private float GetHeuristicCost(Vector3Int current, Vector3Int goal)
    {
        // Use Manhattan distance as the heuristic
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    private List<Vector3Int> GetNeighborTiles(Vector3Int tile)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        neighbors.Add(tile + Vector3Int.up);
        neighbors.Add(tile + Vector3Int.down);
        neighbors.Add(tile + Vector3Int.left);
        neighbors.Add(tile + Vector3Int.right);

        return neighbors;
    }

    private float GetDistance(Vector3Int current, Vector3Int neighbor)
    {
        // Assuming uniform cost for all neighboring tiles
        return 1f;
    }

    private bool IsObstacleTile(Vector3Int tile)
    {
        return obstacleTilemap.GetTile(tile) != null;
    }

    private void ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        shortestPath = new List<Vector3Int>();
        while (cameFrom.ContainsKey(current))
        {
            shortestPath.Add(current);
            current = cameFrom[current];
        }
        shortestPath.Reverse();
    }

    private void ClearPath()
    {
        // Clear previous path
        shortestPath = null;

        // Clear any visual representation of the previous path on the tilemap
        // (e.g., reset tile colors or remove path indicators)
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            if (debugTilemap.GetTile(position) != null)
            {
                debugTilemap.SetTile(position, null);
            }
        }
    }

    private void MoveActorAlongPath()
    {
        if (shortestPath == null || shortestPath.Count == 0)
            return;

        StartCoroutine(MoveActorCoroutine());
    }

    private IEnumerator MoveActorCoroutine()
    {
        foreach (Vector3Int tile in shortestPath)
        {
            // Calculate the world position of the center of the tile
            Vector3 targetPosition = tilemap.GetCellCenterWorld(tile);

            // Move actor to the next tile
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                // Smoothly move the actor towards the target position
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                yield return null;
            }

            // Ensure the actor is exactly at the target position
            transform.position = targetPosition;

            // Optionally, add delays for each move to simulate actor movement
            yield return new WaitForSeconds(moveDelay);
        }
    }

    private void UpdateTilemap()
    {
        if (shortestPath == null || shortestPath.Count == 0)
            return;

        // Display viable paths and highlight the shortest path on the debug tilemap
        foreach (Vector3Int position in shortestPath)
        {
            debugTilemap.SetTile(position, goalTile);
        }
    }
}
