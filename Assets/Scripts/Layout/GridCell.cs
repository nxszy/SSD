using System;
using Unity.VisualScripting;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("References")]
    private LayoutManager layoutManager;
    [SerializeField] private GameObject highlight;

    [Header("Preview")]
    private Vector3 previewOffset = new Vector3(0, 0, -1f);
    private GridCell previewObject;
    private int rotation = 0;
    public int Rotation => rotation;

    [Header("Cell properties")]
    private int x, y;
    public int X => x;
    public int Y => y;
    private CellType cellType;
    public CellType CellType => cellType;

    void Awake()
    {
        layoutManager = LayoutManager.Instance;
    }

    public void Initialize(int x, int y, CellType cellType) {
        this.x = x;
        this.y = y;
        this.cellType = cellType;
    }

    void Update() {
        if (layoutManager.GetEditMode() && Input.GetKeyDown(KeyCode.R)) {
            RotatePreview();
        }
    }
 
    void OnMouseEnter() {
        if (layoutManager.GetEditMode() && layoutManager != null && previewObject == null) {
            ShowPreview(layoutManager.GetActiveCellType());
        }
        if (!layoutManager.GetEditMode() && highlight != null) {
            highlight.SetActive(true);
        }
    }

    void OnMouseExit() {
        if (layoutManager.GetEditMode() && previewObject != null) {
            HidePreview();
        }
        if (!layoutManager.GetEditMode() && highlight != null) {
            highlight.SetActive(false);
        }
    }

    void OnMouseDown() {
        if (layoutManager.GetEditMode() && layoutManager != null) {
            SetCellType(layoutManager.GetActiveCellType());
        }
        if (layoutManager.GetEditMode() && previewObject != null) {
            HidePreview();
        }
    }

    public void SetCellType(CellType newCellType) {
        cellType = newCellType;
        ReplaceWithNewPrefab(newCellType);
    }

    private void ReplaceWithNewPrefab(CellType newCellType) {
        GridCell newPrefab = layoutManager.GetPrefabForCellType(newCellType);
        if (newPrefab != null) {
            GridCell newCell = Instantiate(newPrefab, transform.position, Quaternion.identity);
            newCell.name = $"GridCell {x} {y}";
            newCell.Initialize(x, y, newCellType);
            newCell.transform.Rotate(0, 0, rotation);
            layoutManager.SetCell(newCell);

            Destroy(gameObject);
        }
    }

    private void RotatePreview() {
        if (previewObject != null) {
            previewObject.transform.Rotate(0, 0, 90);

            rotation += 90;
            rotation %= 360;
            layoutManager.SetCurrentRotation(rotation);
        }
    }

    private void ShowPreview(CellType previewCellType) {
        GridCell previewPrefab = layoutManager.GetPrefabForCellType(previewCellType);
        rotation = layoutManager.GetCurrentRotation();
        if (previewPrefab != null) {
            previewObject = Instantiate(previewPrefab, transform.position + previewOffset, Quaternion.identity);
            Color previewColor = previewObject.GetComponent<SpriteRenderer>().color.WithAlpha(0.5f);
            previewObject.transform.Rotate(0, 0, rotation);
            previewObject.GetComponent<SpriteRenderer>().color = previewColor;
            previewObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void HidePreview() {
        if (previewObject != null) {
            Destroy(previewObject.gameObject);
            previewObject = null;
        }
    }
}

public enum CellType
{
    Road,
    Grass,
    Empty,
    Charger
}
