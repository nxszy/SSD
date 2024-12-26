using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject groundPrefab;
    
    [Header("Layout settings")]
    [SerializeField] private int width; // grid width
    [SerializeField] private int height; // grid height
    [SerializeField] private float cellSize; // Rozmiar pojedynczej komórki
    [SerializeField] private int[] verticalRoads = new int[] {2, 4, 10};
    [SerializeField] private int[] horizontalRoads = new int[] {5, 9};
    [SerializeField] private GridCell[] spawnPoints = new GridCell[] {};

    public GridCell[,] grid;

    public Vector2 GridSize{
        get{return new Vector2(width, height);}
    }

    void Start()
    {
        grid = new GridCell[width, height];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isRoad = horizontalRoads.Contains(x) || verticalRoads.Contains(y);

                GameObject cellPrefab = isRoad ? roadPrefab : groundPrefab;
                GameObject cellObject = Instantiate(cellPrefab, new Vector3(x * cellSize, y * cellSize, 0), Quaternion.identity);

                cellObject.transform.parent = transform;

                GridCell cell = new()
                {
                    coords = new Vector2(x, y),
                    isRoad = isRoad,
                };
                grid[x, y] = cell;

                cellObject.name = $"Cell ({x}, {y})";
            }
        }
    }
}

public class GridCell
{
    public Vector2 coords;
    public bool isRoad;
    public Car occupiedBy;
}
