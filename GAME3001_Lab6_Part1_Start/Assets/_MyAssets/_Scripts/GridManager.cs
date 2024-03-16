using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NeighbourTile
{
    TOP_TILE,
    RIGHT_TILE,
    BOTTOM_TILE,
    LEFT_TILE,
    NUM_OF_NEIGHBOUR_TILES
}

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private int rows = 12;
    [SerializeField] private int columns = 16;
    [SerializeField] private float baseTileCost = 1f;
    [SerializeField] private bool useManhattanHeuristic = true;
    [SerializeField] private GameObject panelParent;

    private GameObject[,] grid;
    private List<GameObject> mines = new List<GameObject>();
    public static GridManager Instance { get; private set; }
    void Awake()
    {
        Initialize();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of GridManager already exists. Destroying this one.");
            Destroy(gameObject);
        }
    }
   

    private void Initialize()
    {
        BuildGrid();
        ConnectGrid();
    }

    private void BuildGrid()
    {
        grid = new GameObject[rows, columns];
        float rowPos = 5.5f;
        for (int row = 0; row < rows; row++, rowPos--)
        {
            float colPos = -7.5f;
            for (int col = 0; col < columns; col++, colPos++)
            {
                GameObject tileInst = Instantiate(tilePrefab, new Vector3(colPos, rowPos, 0f), Quaternion.identity);
                tileInst.transform.parent = transform;
                grid[row, col] = tileInst;
            }
        }
    }

    public void ConnectGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject currentTile = grid[row, col];
                if (currentTile == null) continue;

                TileScript tileScript = currentTile.GetComponent<TileScript>();
                if (tileScript == null) continue;

                tileScript.ResetNeighbourConnections();
                if (tileScript.status == TileStatus.IMPASSABLE) continue;

                ConnectTiles(tileScript, row - 1, col, NeighbourTile.TOP_TILE);
                ConnectTiles(tileScript, row, col + 1, NeighbourTile.RIGHT_TILE);
                ConnectTiles(tileScript, row + 1, col, NeighbourTile.BOTTOM_TILE);
                ConnectTiles(tileScript, row, col - 1, NeighbourTile.LEFT_TILE);
            }
        }
    }

    private void ConnectTiles(TileScript tileScript, int row, int col, NeighbourTile direction)
    {
        if (row >= 0 && row < rows && col >= 0 && col < columns)
        {
            GameObject neighbor = grid[row, col];
            if (neighbor != null)
            {
                TileScript neighborTileScript = neighbor.GetComponent<TileScript>();
                if (neighborTileScript != null)
                {
                    tileScript.SetNeighbourTile((int)direction, neighbor);
                    if (tileScript.Node != null && neighborTileScript.Node != null)
                    {
                        tileScript.Node.AddConnection(new PathConnection(tileScript.Node, neighborTileScript.Node,
                            Vector3.Distance(tileScript.transform.position, neighbor.transform.position)));
                    }
                }
            }
        }
    }






    public void ToggleGridVisibility()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
        panelParent.gameObject.SetActive(!panelParent.gameObject.activeSelf);
    }

    public void AddMine()
    {
        Vector2 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        GameObject mineInst = Instantiate(minePrefab, new Vector3(gridPosition.x, gridPosition.y, 0f), Quaternion.identity);
        Vector2 mineIndex = mineInst.GetComponent<NavigationObject>().GetGridIndex();
        Destroy(grid[(int)mineIndex.y, (int)mineIndex.x]);
        grid[(int)mineIndex.y, (int)mineIndex.x] = null;
        mines.Add(mineInst);
    }

    public void ClearMines()
    {
        foreach (GameObject mine in mines)
        {
            Vector2 mineIndex = mine.GetComponent<NavigationObject>().GetGridIndex();
            GameObject tileInst = Instantiate(tilePrefab, new Vector3(mine.transform.position.x, mine.transform.position.y, 0f), Quaternion.identity);
            tileInst.transform.parent = transform;
            grid[(int)mineIndex.y, (int)mineIndex.x] = tileInst;
            Destroy(mine);
        }
        mines.Clear();
    }

    public void StartPathfinding()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
        PathNode start = grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().Node;

        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
        PathNode goal = grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().Node;

        PathManager.Instance.GetShortestPath(start, goal);
    }

    public void ResetGrid()
    {
        foreach (GameObject go in grid)
        {
            go.GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
        }

        foreach (GameObject mine in mines)
        {
            Vector2 mineIndex = mine.GetComponent<NavigationObject>().GetGridIndex();
            grid[(int)mineIndex.y, (int)mineIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.IMPASSABLE);
        }

        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.START);

        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
    }

    public void SetTileCosts(Vector2 targetIndices)
    {
        float distance = 0f;
        float dx = 0f;
        float dy = 0f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                TileScript tileScript = grid[row, col].GetComponent<TileScript>();
                if (useManhattanHeuristic)
                {
                    dx = Mathf.Abs(col - targetIndices.x);
                    dy = Mathf.Abs(row - targetIndices.y);
                    distance = dx + dy;
                }
                else // Euclidean.
                {
                    dx = targetIndices.x - col;
                    dy = targetIndices.y - row;
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                }
                float adjustedCost = distance * baseTileCost;
                tileScript.cost = adjustedCost;
                tileScript.tilePanel.costText.text = tileScript.cost.ToString("F1");
            }
        }
    }

    public void SetTileStatuses()
    {
        foreach (GameObject go in grid)
        {
            go.GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
        }
        foreach (GameObject mine in mines)
        {
            Vector2 mineIndex = mine.GetComponent<NavigationObject>().GetGridIndex();
            grid[(int)mineIndex.y, (int)mineIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.IMPASSABLE);
        }
        // Set the tile under the ship to Start.
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.START);
        // Set the tile under the player to Goal.
        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
    }

    public GameObject[,] GetGrid()
    {
        return grid;
    }

    public Vector2 GetGridPosition(Vector2 worldPosition)
    {
        float xPos = Mathf.Floor(worldPosition.x) + 0.5f;
        float yPos = Mathf.Floor(worldPosition.y) + 0.5f;
        return new Vector2(xPos, yPos);
    }
  
}
