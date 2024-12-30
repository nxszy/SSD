using UnityEngine;

public class LayoutUIController : MonoBehaviour
{
    [SerializeField] private LayoutManager layoutManager;
    [SerializeField] private GameObject editPanel;
    [SerializeField] private GameObject popUpPanel;
    public void onRoadPress()
    {
        layoutManager.SetActiveCellType(CellType.Road);
    }

    public void onGrassPress()
    {
        layoutManager.SetActiveCellType(CellType.Grass);
    }

    public void onChargerPress()
    {
        layoutManager.SetActiveCellType(CellType.Charger);
    }

    public void onStartPress()
    {
        if (layoutManager.ValidateLayout())
        {
            layoutManager.SetEditMode(false);
            editPanel.SetActive(false);
        } else {
            popUpPanel.SetActive(true);
        }   
    }

    public void onOkPress()
    {
        popUpPanel.SetActive(false);
    }

    public void onResetPress()
    {
        layoutManager.SetEditMode(true);
        editPanel.SetActive(true);

        layoutManager.ResetLayout();
    }
}
