using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    private static LayoutManager instance;

    public static LayoutManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<LayoutManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("LayoutManager");
                    instance = obj.AddComponent<LayoutManager>();
                }
            }
            return instance;
        }
    }

    [Header("Layout settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    public Vector2 Size => new(width, height);

    [Header("Layout prefabs")]
    [SerializeField] private GridCell grassPrefab;
    [SerializeField] private GridCell roadPrefab;
    [SerializeField] private GridCell chargerPrefab;
    [SerializeField] private GridCell emptyPrefab;

    private Dictionary<Vector2Int, GridCell> grid;
    private CellType activeCellType;
    private bool isEditMode = true;
    private int rotation = 0;

    public void SetActiveCellType(CellType cellType)
    {
        activeCellType = cellType;
    }

    public CellType GetActiveCellType()
    {
        return activeCellType;
    }

    public GridCell GetPrefabForCellType(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Road:
                return roadPrefab;
            case CellType.Grass:
                return grassPrefab;
            case CellType.Charger:
                return chargerPrefab;
            case CellType.Empty:
                return emptyPrefab;
            default:
                return null;
        }
    }

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Dictionary<Vector2Int, GridCell>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = Instantiate(emptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.name = $"GridCell {x} {y}";
                cell.Initialize(x, y, CellType.Empty);

                grid.Add(new Vector2Int(x, y), cell);
            }
        }
    }

    void ClearGrid()
    {
        foreach (var cell in grid.Values)
        {
            Destroy(cell.gameObject);
        }
        grid.Clear();
    }

    public void ResetLayout()
    {
        ClearGrid();
        GenerateGrid();
    }

    public bool ValidateLayout()
    {
        foreach (var cell in grid.Values)
        {
            if (cell.CellType == CellType.Empty)
            {
                return false;
            }
        }
        return true;
    }
    
    public void SetCell(GridCell cell)
    {
        Vector2Int cellPos = new Vector2Int(cell.X, cell.Y);
        grid[cellPos] = cell;
    }

    public GridCell GetCell(Vector2Int cellPos)
    {
        grid.TryGetValue(cellPos, out GridCell cell);
        return cell;
    }

    public void SetEditMode(bool editMode)
    {
        isEditMode = editMode;
    }

    public bool GetEditMode()
    {
        return isEditMode;
    }

    public int GetCurrentRotation()
    {
        return rotation;
    }

    public void SetCurrentRotation(int newRotation)
    {
        rotation = newRotation;
    }
}
