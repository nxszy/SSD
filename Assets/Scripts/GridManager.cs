using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    public Vector2 Size => new(width, height);

    [Header("Grid prefabs")]
    [SerializeField] private GridCell grassPrefab;
    [SerializeField] private GridCell roadPrefab;
    [SerializeField] private GridCell chargerPrefab;
    [SerializeField] private GridCell carPrefab;
    [SerializeField] private GridCell ecarPrefab;


    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell prefab = grassPrefab;
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    prefab = roadPrefab;
                }

                GridCell cell = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.name = $"GridCell {x} {y}";
            }
        }
    }

}
