using System.Collections.Generic;
using UnityEngine;

public class RoadCell : MonoBehaviour
{
    [Header("Road sprites")]
    [SerializeField] private Sprite roadSprite;
    [SerializeField] private Sprite roadCrossSprite;

    [Header("References")]
    private LayoutManager layoutManager;

    void Awake()
    {
        layoutManager = LayoutManager.Instance;
    }  

    void FixedUpdate()
    {
        if (layoutManager.GetEditMode())
        {
            DetermineSprite();
        }
    }

    private void DetermineSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        GridCell gridCell = GetComponent<GridCell>();

        var nearby_cells = new List<GridCell>();
        Vector2Int cellPos = new Vector2Int(gridCell.X, gridCell.Y);

        //check all 4 directions
        nearby_cells.Add(layoutManager.GetCell(cellPos + new Vector2Int(0, 1)));
        nearby_cells.Add(layoutManager.GetCell(cellPos + new Vector2Int(0, -1)));
        nearby_cells.Add(layoutManager.GetCell(cellPos + new Vector2Int(1, 0)));
        nearby_cells.Add(layoutManager.GetCell(cellPos + new Vector2Int(-1, 0)));

        //check if there is a road in more than 2 directions
        int roadCount = 0;
        foreach (var cell in nearby_cells)
        {
            if (cell != null && cell.CellType == CellType.Road)
            {
                roadCount++;
            }
        }

        if (roadCount > 3)
        {
            spriteRenderer.sprite = roadCrossSprite;
        }
        else
        {
            spriteRenderer.sprite = roadSprite;
        }
    }
}
